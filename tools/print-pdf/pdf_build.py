"""Build the PDF print templates for the app.

For each form (print/NNN.docx or .xlsx with {{markers}}):
  1. probe : every marker drawn in its own colour (placeholders keep the same width as the original text)
             -> LibreOffice -> PDF.   (also a 'defaults' render, used only to check that nothing moved)
  2. read the probe PDF (pdfplumber): the coloured characters tell exactly where each answer goes
  3. turn the probe PDF into the background PDF:
       - marker colours back to black
       - placeholders made invisible (text render mode 3)
       - text defaults like "(Station)" get a small switch stream: the app writes "3 Tr" into it to hide them
       - fonts Helvetica + ZapfDingbats added, one empty "overlay" stream per page (the app fills it)
       - saved with a classic xref table, so the app can append an incremental update
  4. NNN.json: positions + the object numbers the app needs
"""
import re, sys, os, json, zipfile, subprocess, copy
from lxml import etree
import pdfplumber, pikepdf

MARK = re.compile(r"\{\{([^{}<|]+)(?:\|([^{}<]*))?\}\}")
W = "{http://schemas.openxmlformats.org/wordprocessingml/2006/main}"
XS = "{http://www.w3.org/XML/1998/namespace}space"


def color_of(i):
    return (40 + (i // 50) * 4, 20 + (i % 50) * 4, 201)


def id_of(rgb):
    try:
        r, g, b = [round(float(v) * 255) for v in rgb]
    except Exception:
        return None
    if b != 201 or r < 40 or (r - 40) % 4 or g < 20 or (g - 20) % 4 or g > 216:
        return None
    return (r - 40) // 4 * 50 + (g - 20) // 4


def kind_of(key, dflt):
    if key.startswith("SIG:"):
        return "sig"
    if not dflt:
        return "e"
    if set(dflt) == {"_"}:
        return "u"
    if set(dflt) == {" "}:
        return "s"
    return "t"


def probe_text(kind, dflt):
    if kind == "sig":
        return ""
    if kind == "s":
        return dflt                 # the spaces themselves (coloured): LibreOffice writes them in the PDF too
    if kind == "e":
        return "."
    return dflt


def render(path, outdir):
    subprocess.run(["soffice", "--headless", "--convert-to", "pdf", "--outdir", outdir, path], capture_output=True)
    out = os.path.join(outdir, os.path.splitext(os.path.basename(path))[0] + ".pdf")
    assert os.path.exists(out), "render failed: " + path
    return out


# ---------------------------------------------------------------- xlsx page setup for the render
def sheet_print_setup(xml, paper=None, fit=False):
    """one page wide (like Excel 'Fit all columns on one page'), page breaks kept."""
    if not fit and not paper:
        return xml
    if not fit:
        return re.sub(r'(<pageSetup\b[^>]*?)\spaperSize="[^"]*"', r'\1 paperSize="%s"' % paper, xml, 1)
    if "fitToPage" not in xml:
        if re.search(r"<sheetPr\b[^>]*/>", xml):
            xml = re.sub(r"<sheetPr\b([^>]*)/>", r'<sheetPr\1><pageSetUpPr fitToPage="1"/></sheetPr>', xml, 1)
        elif "<sheetPr" in xml:
            xml = re.sub(r"(<sheetPr\b[^>]*>)", r'\1<pageSetUpPr fitToPage="1"/>', xml, 1)
        else:
            xml = re.sub(r"(<worksheet\b[^>]*>)", r'\1<sheetPr><pageSetUpPr fitToPage="1"/></sheetPr>', xml, 1)
    def ps(m):
        a = re.sub(r'\s(fitToWidth|fitToHeight|scale)="[^"]*"', "", m.group(1))
        if paper:
            a = re.sub(r'\spaperSize="[^"]*"', "", a) + f' paperSize="{paper}"'
        return f'<pageSetup{a} fitToWidth="1" fitToHeight="0"'
    xml = re.sub(r"<pageSetup\b([^>]*?)(?=\s*/?>)", ps, xml, 1)
    return xml


def workbook_print_setup(xml):
    """hidden sheets: no print area (LibreOffice would print them)."""
    sheets = re.findall(r"<sheet\b[^>]*>", xml)
    hidden = [i for i, s in enumerate(sheets) if 'state="hidden"' in s or "state=\"veryHidden\"" in s]
    for i in hidden:
        xml = re.sub(r'<definedName name="_xlnm\.Print_Area" localSheetId="%d"[^>]*>[^<]*</definedName>' % i, "", xml)
    return xml


def squeeze(data, k):
    """LibreOffice lays out a little longer than Word: remove k empty paragraphs (one from each of the
    longest runs of 3+ empty lines, starting at the end) so the page count is the same as in Word."""
    root = etree.fromstring(data)
    body = root.find(W + "body")
    def empty(p):
        return (p.tag == W + "p" and not "".join(t.text or "" for t in p.iter(W + "t")).strip()
                and p.find(".//" + W + "drawing") is None and p.find(".//" + W + "pict") is None
                and p.find(".//" + W + "sectPr") is None and p.find(".//{*}AlternateContent") is None)
    for _ in range(k):
        kids = list(body)
        runs, cur = [], []
        for c in kids:
            if empty(c):
                cur.append(c)
            else:
                if len(cur) >= 3:
                    runs.append(cur)
                cur = []
        if not runs:
            break
        run = runs[-1]
        body.remove(run[len(run) // 2])
    return etree.tostring(root, xml_declaration=True, encoding="UTF-8", standalone=True)


RPR_AFTER_COLOR = re.compile(r"<w:(spacing|w|kern|position|sz|szCs|highlight|u|effect|bdr|shd|fitText|vertAlign|rtl|cs|em|lang|eastAsianLayout|specVanish|oMath)\b")
TEXT_RUN = re.compile(r"(<w:r(?:\s[^>]*)?>)(?:<w:rPr>((?:(?!</w:rPr>).)*)</w:rPr>)?(<w:t[^>]*>[^<]*\{\{([^{}<|]+)\|([^{}<]*)\}\}[^<]*</w:t></w:r>)", re.S)


def sentinel(j):
    return "0203%02X" % j


def mark_text_defaults(xml, sent):
    """Word: runs with a text default like "(Station)" -> colour 02 03 jj (looks black)."""
    def rep(m):
        key, dflt = m.group(4).strip(), m.group(5)
        if kind_of(key, dflt) != "t":
            return m.group(0)
        j = next((k for k, v in sent.items() if v == key), None) or len(sent) + 1
        sent[j] = key
        rpr = re.sub(r"<w:color\b[^>]*/>", "", m.group(2) or "")
        a = RPR_AFTER_COLOR.search(rpr)
        col = '<w:color w:val="%s"/>' % sentinel(j)
        rpr = rpr[:a.start()] + col + rpr[a.start():] if a else rpr + col
        return m.group(1) + "<w:rPr>" + rpr + "</w:rPr>" + m.group(3)
    return TEXT_RUN.sub(rep, xml)


# ---------------------------------------------------------------- probe document
def make_probe(src, probe_out, dflt_out, opts):
    markers = []
    zin = zipfile.ZipFile(src)
    zp = zipfile.ZipFile(probe_out, "w", zipfile.ZIP_DEFLATED)
    zd = zipfile.ZipFile(dflt_out, "w", zipfile.ZIP_DEFLATED)
    is_docx = src.endswith(".docx")
    fonts, xff, xal = [], [], []
    if not is_docx:
        styles = zin.read("xl/styles.xml").decode()
        fonts = [re.sub(r"<name\b", "<rFont", f.group(1) or "") for f in
                 re.finditer(r"<font\b[^>]*?(?:/>|>(.*?)</font>)", re.search(r"<fonts[^>]*>(.*?)</fonts>", styles, re.S).group(1), re.S)]
        for x in re.finditer(r"<xf\b[^>]*?(?:/>|>.*?</xf>)", re.search(r"<cellXfs[^>]*>(.*?)</cellXfs>", styles, re.S).group(1), re.S):
            f = re.search(r'fontId="(\d+)"', x.group(0)); xff.append(int(f.group(1)) if f else 0)
            h = re.search(r'horizontal="(\w+)"', x.group(0)); xal.append(h.group(1) if h else "general")

    for it in zin.infolist():
        data = zin.read(it.filename)
        name = it.filename
        is_sheet = name.startswith("xl/worksheets/sheet") and name.endswith(".xml")
        if name == "xl/workbook.xml":
            data = workbook_print_setup(data.decode()).encode()
        if not (name.endswith(".xml") and name.startswith(("word/", "xl/")) and b"{{" in data):
            if is_sheet:
                data = sheet_print_setup(data.decode(), opts.get("paper"), opts.get("fit")).encode()
            zp.writestr(copy.copy(it), data); zd.writestr(copy.copy(it), data)
            continue
        if name == "word/document.xml" and opts.get("squeeze"):
            data = squeeze(data, opts["squeeze"])
        s = data.decode()
        # ---- defaults: markers -> their original text (= the original form); only for checking
        d = re.sub(r"<w:r(?:\s[^>]*)?>(?:(?!</w:r>).)*?\{\{SIG:[^}]*\}\}(?:(?!</w:r>).)*?</w:r>", "", s, flags=re.S)
        d = re.sub(r"<w:t>([^<]*\{\{)", r'<w:t xml:space="preserve">\1', d)
        if is_docx:
            d = mark_text_defaults(d, opts.setdefault("_sent", {}))
        d = MARK.sub(lambda m: m.group(2) or "", d)
        d = re.sub(r'<c ([^>]*?)\s*t="inlineStr"([^>]*)><is><t[^>]*></t></is></c>', r"<c \1\2/>", d)
        # ---- probe: each marker in its own colour
        if is_docx:
            root = etree.fromstring(data)
            for r in list(root.iter(W + "r")):
                ts = r.findall(W + "t")
                txt = "".join(t.text or "" for t in ts)
                ms = list(MARK.finditer(txt))
                if not ms:
                    continue
                assert len(ms) == 1, (name, txt)
                m = ms[0]
                key, dflt = m.group(1).strip(), m.group(2) or ""
                kind = kind_of(key, dflt)
                i = len(markers)
                rpr0 = r.find(W + "rPr")
                sz = rpr0.find(W + "sz") if rpr0 is not None else None
                col0 = rpr0.find(W + "color") if rpr0 is not None else None
                par = r.getparent()
                while par is not None and par.tag != W + "p":
                    par = par.getparent()
                alone = False
                if par is not None:
                    other = "".join(t.text or "" for t in par.iter(W + "t") if t not in ts)
                    alone = not other.strip() and not txt.replace(m.group(0), "").strip()
                jc = par.find(W + "pPr/" + W + "jc") if par is not None else None
                jcv = jc.get(W + "val") if jc is not None else "left"
                al = {"center": "c", "right": "r", "end": "r"}.get(jcv, "l") if alone else "l"
                markers.append({"id": i, "key": key, "dflt": dflt, "kind": kind, "part": name, "al": al, "alone": alone,
                                "rs": int(sz.get(W + "val")) / 2 if sz is not None else None,
                                "col": col0.get(W + "val") if col0 is not None and col0.get(W + "val") not in (None, "auto") else None})
                new = txt[:m.start()] + probe_text(kind, dflt) + txt[m.end():]
                for t in ts[1:]:
                    r.remove(t)
                ts[0].text = new
                ts[0].set(XS, "preserve")
                # an empty marker alone in its paragraph: the line height comes from the paragraph mark,
                # so draw the probe dot with the mark's font (keeps the layout the same as the original)
                if kind == "e" and alone and par is not None:
                    mark = par.find(W + "pPr/" + W + "rPr")
                    if mark is not None:
                        if rpr0 is not None:
                            r.remove(rpr0)
                        r.insert(0, copy.deepcopy(mark))
                rpr = r.find(W + "rPr")
                if rpr is None:
                    rpr = etree.Element(W + "rPr"); r.insert(0, rpr)
                for c in rpr.findall(W + "color"):
                    rpr.remove(c)
                col = etree.Element(W + "color")
                col.set(W + "val", "%02X%02X%02X" % color_of(i))
                after = [c for c in rpr if etree.QName(c).localname in ("rStyle", "rFonts", "b", "bCs", "i", "iCs", "caps", "smallCaps",
                         "strike", "dstrike", "outline", "shadow", "emboss", "imprint", "noProof", "snapToGrid", "vanish", "webHidden")]
                (after[-1].addnext(col) if after else rpr.insert(0, col))
            p = etree.tostring(root, xml_declaration=True, encoding="UTF-8", standalone=True).decode()
        else:
            def cell(m):
                inner = m.group(2)
                if "{{" not in inner:
                    return m.group(0)
                s_ = re.search(r'\ss="(\d+)"', m.group(1)); xf = int(s_.group(1)) if s_ else 0
                cf = fonts[xff[xf]] if xf < len(xff) and xff[xf] < len(fonts) else ""
                cal = xal[xf] if xf < len(xal) else "general"
                rich = list(re.finditer(r"<r>(?:<rPr>(.*?)</rPr>)?<t[^>]*>(.*?)</t></r>", inner, re.S))
                runs = [(r.group(1) if r.group(1) is not None else cf, r.group(2)) for r in rich] if rich else \
                       [(cf, re.search(r"<t[^>]*>(.*?)</t>", inner, re.S).group(1))]
                alltext = "".join(t for _, t in runs)
                out = "<is>"
                def run(rpr, t):
                    return "<r>" + ("<rPr>" + rpr + "</rPr>" if rpr else "") + '<t xml:space="preserve">' + t + "</t></r>"
                for rpr, t in runs:
                    pos = 0
                    for mk in MARK.finditer(t):
                        if mk.start() > pos:
                            out += run(rpr, t[pos:mk.start()])
                        key, dflt = mk.group(1).strip(), mk.group(2) or ""
                        kind = kind_of(key, dflt)
                        i = len(markers)
                        alone = alltext.strip() == mk.group(0).strip()
                        al = {"center": "c", "centerContinuous": "c", "right": "r"}.get(cal, "l") if alone else "l"
                        markers.append({"id": i, "key": key, "dflt": dflt, "kind": kind, "part": name, "al": al, "rs": None, "col": None,
                                        "cell": re.search(r'r="([A-Z]+\d+)"', m.group(1)).group(1)})
                        rc = re.sub(r"<color\b[^>]*/>", "", rpr) + '<color rgb="FF%02X%02X%02X"/>' % color_of(i)
                        out += run(rc, probe_text(kind, dflt))
                        pos = mk.end()
                    if pos < len(t):
                        out += run(rpr, t[pos:])
                return m.group(1) + out + "</is>" + m.group(3)
            p = re.sub(r'(<c [^>]*t="inlineStr"[^>]*>)<is>(.*?)</is>(</c>)', cell, s, flags=re.S)
        if is_sheet:
            d = sheet_print_setup(d, opts.get("paper"), opts.get("fit")); p = sheet_print_setup(p, opts.get("paper"), opts.get("fit"))
        zp.writestr(copy.copy(it), p.encode()); zd.writestr(copy.copy(it), d.encode())
    zp.close(); zd.close()
    return markers


# ---------------------------------------------------------------- positions
def extract(probe_pdf, markers):
    byid = {m["id"]: m for m in markers}
    pages = []
    with pdfplumber.open(probe_pdf) as pdf:
        for pi, page in enumerate(pdf.pages):
            H = page.height
            pages.append((round(page.width, 2), round(H, 2)))
            edges = page.edges
            vedges = [e for e in edges if e["orientation"] == "v"]
            hedges = [e for e in edges if e["orientation"] == "h"]
            # long lines drawn in pieces (underlined tabs): join them
            hlines = []
            for e in sorted(hedges, key=lambda e: (round(e["top"], 1), e["x0"])):
                if hlines and abs(hlines[-1]["top"] - e["top"]) < 0.3 and e["x0"] <= hlines[-1]["x1"] + 1.5:
                    hlines[-1]["x1"] = max(hlines[-1]["x1"], e["x1"])
                else:
                    hlines.append({"x0": e["x0"], "x1": e["x1"], "top": e["top"]})
            chars = [c for c in page.chars if c["text"].strip() and c["text"] != "\u200b"]
            spaces = [c for c in page.chars if c["text"] == " " and id_of(c.get("non_stroking_color") or ()) is not None]
            mine = {}
            for ch in chars + spaces:
                i = id_of(ch.get("non_stroking_color") or ())
                ch["_id"] = i
                if i is not None and i in byid:
                    mine.setdefault(i, []).append(ch)
            for i, chs in mine.items():
                m = byid[i]
                segs = {}
                for ch in chs:
                    k = round(ch["matrix"][5], 1)
                    sg = segs.setdefault(k, {"x0": ch["x0"], "x1": ch["x1"], "y": ch["matrix"][5], "s": ch["size"],
                                             "top": ch["top"], "bottom": ch["bottom"]})
                    sg["x0"] = min(sg["x0"], ch["x0"]); sg["x1"] = max(sg["x1"], ch["x1"])
                    sg["top"] = min(sg["top"], ch["top"]); sg["bottom"] = max(sg["bottom"], ch["bottom"])
                for k, sg in sorted(segs.items(), key=lambda kv: -kv[0]):
                    top, bot = sg["top"], sg["bottom"]
                    def vcross(e):
                        return e["top"] <= bot - 1 and e["bottom"] >= top + 1
                    def band(c):
                        return c["top"] < bot - 1 and c["bottom"] > top + 1 and c.get("_id") != i
                    rights = [e["x0"] for e in vedges if vcross(e) and e["x0"] >= sg["x1"] - 0.5]
                    rights += [c["x0"] for c in chars if band(c) and c["x0"] >= sg["x1"] - 0.5]
                    lefts = [e["x0"] for e in vedges if vcross(e) and e["x0"] <= sg["x0"] + 0.5]
                    lefts += [c["x1"] for c in chars if band(c) and c["x1"] <= sg["x0"] + 0.5]
                    xr = min(rights + [page.width - 20])
                    xl = max(lefts + [20])
                    seg = {"p": pi, "x0": round(sg["x0"], 2), "x1": round(sg["x1"], 2), "y": round(sg["y"], 2),
                           "s": round(sg["s"], 2), "xl": round(xl, 2), "xr": round(xr, 2)}
                    cx = (sg["x0"] + sg["x1"]) / 2
                    if m["key"].startswith("^"):
                        # printed name: how much free room is above it (for the signature)
                        base = H - sg["y"]
                        above = [c["bottom"] for c in chars if c.get("_id") != i and c["bottom"] <= base - 0.5 * sg["s"]
                                 and c["x1"] >= cx - 50 and c["x0"] <= cx + 50 and c["text"] not in "_"]
                        above += [e["top"] for e in hedges if e["top"] <= base - 0.5 * sg["s"] and e["x1"] >= cx - 50 and e["x0"] <= cx + 50]
                        seg["up"] = round(min(base - max(above), 60) if above else 60, 1)
                    # a small box (table cell) around it: for checks
                    vl = [e["x0"] for e in vedges if vcross(e) and e["x0"] <= cx]
                    vr = [e["x0"] for e in vedges if vcross(e) and e["x0"] >= cx]
                    ht = [e["top"] for e in hedges if e["x0"] <= cx <= e["x1"] and e["top"] <= (top + bot) / 2]
                    hb = [e["top"] for e in hedges if e["x0"] <= cx <= e["x1"] and e["top"] >= (top + bot) / 2]
                    if vl and vr and ht and hb:
                        bl, br, bt, bb = max(vl), min(vr), max(ht), min(hb)
                        if br - bl < 60 and bb - bt < 120:
                            seg["box"] = [round(bl, 2), round(H - bb, 2), round(br, 2), round(H - bt, 2)]
                    # Word: an empty answer alone in its paragraph, floating above a line (not in a table):
                    # write it ON the line (names in the middle of the line)
                    if m["kind"] == "e" and m.get("alone") and m["part"].startswith("word/"):
                        def table_line(h):   # a cell border: vertical lines meet its ends
                            return any(min(abs(v["x0"] - h["x0"]), abs(v["x0"] - h["x1"])) < 1.5 and
                                       v["top"] - 1.5 <= h["top"] <= v["bottom"] + 1.5 for v in vedges)
                        under = [e for e in hlines if e["x0"] <= sg["x0"] + 3 and e["x1"] >= sg["x0"] + 40
                                 and 3 < e["top"] - (H - sg["y"]) <= 22 and not table_line(e)]
                        if under:
                            h = min(under, key=lambda e: e["top"])
                            between = [c for c in chars if c.get("_id") != i and h["x0"] <= c["x0"] <= h["x1"]
                                       and H - sg["y"] < c["bottom"] < h["top"]]
                            if not between:
                                seg.update(x0=round(h["x0"], 2), x1=round(h["x1"], 2), y=round(H - h["top"] + 2.5, 2),
                                           xl=round(h["x0"], 2), xr=round(h["x1"], 2), snap=1)
                    m.setdefault("segs", []).append(seg)
    return pages


# ---------------------------------------------------------------- background PDF
SHOW = {"Tj", "TJ", "'", '"'}


def make_base(dflt_pdf, out_pdf, sent, drop_pages=()):
    """Background = the render of the original form. Only the text defaults ("(Station)") get a switch."""
    pdf = pikepdf.open(dflt_pdf)
    for n in sorted(drop_pages, reverse=True):
        del pdf.pages[n]
    helv = pdf.make_indirect(pikepdf.Dictionary(Type=pikepdf.Name.Font, Subtype=pikepdf.Name.Type1,
                                                 BaseFont=pikepdf.Name.Helvetica, Encoding=pikepdf.Name.WinAnsiEncoding))
    reset = pdf.make_stream(b"0 Tr\n")
    toggles = {}     # key -> [switch streams]
    def sent_of(operands):
        try:
            r, g, b = [round(float(v) * 255) for v in operands]
        except Exception:
            return None
        return sent.get(b) if (r, g) == (2, 3) else None
    for n, page in enumerate(pdf.pages):
        res = page.obj.get("/Resources")
        assert res is not None, "page without own resources"
        fonts = res.get("/Font")
        if fonts is None:
            fonts = pikepdf.Dictionary(); res.Font = fonts
        fonts.FsiHelv = helv
        ops = pikepdf.parse_content_stream(page)
        pieces, cur = [], []
        stack, fill = [], None
        for operands, op in ops:
            o = str(op)
            if o == "q":
                stack.append(fill)
            elif o == "Q":
                fill = stack.pop() if stack else None
            elif o in ("rg", "RG"):
                k = sent_of(operands)
                if o == "rg":
                    fill = k
                if k is not None:
                    operands = [0, 0, 0]
            elif o in ("g", "k", "sc", "scn", "cs"):
                fill = None
            if o in SHOW and fill is not None:
                pieces.append(cur); cur = []
                sw = pdf.make_stream(b"0 Tr\n")
                toggles.setdefault(fill, []).append(sw)
                pieces += [sw, [(operands, op)], reset]
                continue
            cur.append((operands, op))
        pieces.append(cur)
        streams = [pdf.make_stream(b"q\n")]
        for pc in pieces:
            if isinstance(pc, list):
                if pc:
                    streams.append(pdf.make_stream(pikepdf.unparse_content_stream(pc)))
            else:
                streams.append(pc)
        streams.append(pdf.make_stream(b"\nQ\n"))
        ov = pdf.make_stream(b" ")
        ov.FsiOverlay = n
        streams.append(ov)
        page.obj.Contents = pikepdf.Array(streams)
    names = sorted(toggles)
    for key, sws in toggles.items():
        for k, sw in enumerate(sws):
            sw.FsiToggle = names.index(key) * 1000 + k
    pdf.save(out_pdf, object_stream_mode=pikepdf.ObjectStreamMode.disable, compress_streams=True)
    pdf.close()
    pdf = pikepdf.open(out_pdf)
    overlays, tog = {}, {}
    for obj in pdf.objects:
        if isinstance(obj, pikepdf.Stream):
            if "/FsiOverlay" in obj:
                overlays[int(obj.FsiOverlay)] = obj.objgen[0]
            if "/FsiToggle" in obj:
                tog.setdefault(names[int(obj.FsiToggle) // 1000], []).append(obj.objgen[0])
    root = pdf.trailer.Root.objgen[0]
    size = int(pdf.trailer.Size)
    info = pdf.trailer.Info.objgen[0] if "/Info" in pdf.trailer else 0
    ids = [bytes(x).hex().upper() for x in pdf.trailer.ID] if "/ID" in pdf.trailer else []
    pdf.close()
    raw = open(out_pdf, "rb").read()
    prev = int(re.findall(rb"startxref\s+(\d+)", raw)[-1])
    assert raw[prev:prev + 4] == b"xref", "classic xref table expected"
    return {"overlay": [overlays[i] for i in sorted(overlays)], "root": root, "info": info, "id": ids,
            "size": size, "prev": prev}, tog


def correct(probe_pdf, dflt_pdf, markers, drop):
    """The probe can be a hair different from the real form (other colours = other text pieces).
    Move every position by how much the text around it moved between the probe and the real form."""
    def load(path, skip_marker):
        out = []
        with pdfplumber.open(path) as pdf:
            for pi, pg in enumerate(pdf.pages):
                for c in pg.chars:
                    if not c["text"].strip() or c["text"] in "._":
                        continue
                    if skip_marker and id_of(c.get("non_stroking_color") or ()) is not None:
                        continue
                    out.append((pi, c["text"], c["x0"], c["top"]))
        return out
    a, b = load(probe_pdf, True), load(dflt_pdf, False)
    idx = {}
    for pi, t, x, y in b:
        idx.setdefault((pi, t), []).append((x, y))
    pairs = {}
    for pi, t, x, y in a:
        best = None
        for x2, y2 in idx.get((pi, t), []):
            if abs(x2 - x) < 3 and abs(y2 - y) < 6:
                d = abs(x2 - x) + abs(y2 - y)
                if best is None or d < best[0]:
                    best = (d, x2 - x, y2 - y)
        if best:
            pairs.setdefault(pi, []).append((y, x, best[1], best[2]))
    def med(v):
        v = sorted(v); return v[len(v) // 2] if v else 0
    moved = 0
    for m in markers:
        for s in m.get("segs", []):
            orig = s["p"]
            for d in sorted(drop):
                if orig >= d:
                    orig += 1
            pp = pairs.get(orig, [])
            if not pp:
                continue
            H = None
            top = s["_H"] - s["y"] - 0.7 * s["s"] if "_H" in s else None
            if top is None:
                continue
            line = [p for p in pp if abs(p[0] - top) < 3]
            if line:
                near = sorted(line, key=lambda p: abs(p[1] - s["x0"]))[:6]
                dy, dx = med([p[3] for p in near]), med([p[2] for p in near])
            else:
                near = sorted(pp, key=lambda p: abs(p[0] - top))[:10]
                dy, dx = med([p[3] for p in near]), 0
            if abs(dy) > 0.05 or abs(dx) > 0.05:
                moved += 1
                s["y"] = round(s["y"] - dy, 2)
                for k in ("x0", "x1", "xl", "xr"):
                    s[k] = round(s[k] + dx, 2)
                if "box" in s:
                    bx = s["box"]
                    s["box"] = [round(bx[0] + dx, 2), round(bx[1] - dy, 2), round(bx[2] + dx, 2), round(bx[3] - dy, 2)]
    return moved


def build(num, template, workdir, opts=None):
    opts = opts or {}
    os.makedirs(workdir, exist_ok=True)
    ext = os.path.splitext(template)[1]
    probe_doc = os.path.join(workdir, f"{num}_probe{ext}")
    dflt_doc = os.path.join(workdir, f"{num}_dflt{ext}")
    markers = make_probe(template, probe_doc, dflt_doc, opts)
    probe_pdf = render(probe_doc, workdir)
    dflt_pdf = render(dflt_doc, workdir)
    pages = extract(probe_pdf, markers)
    drop = opts.get("drop", [])
    final_pdf = os.path.join(workdir, f"{num}.pdf")
    meta, tog = make_base(dflt_pdf, final_pdf, opts.get("_sent", {}), drop)
    keep = []
    for m in markers:
        if m["kind"] == "sig":
            continue
        segs = [dict(s) for s in m.get("segs", []) if s["p"] not in drop]
        for s in segs:
            s["p"] -= sum(1 for d in drop if d < s["p"])
        if not segs:
            continue
        a = m["al"]
        if any(s.pop("snap", 0) for s in segs):
            a = "c" if m["key"].lstrip("^").endswith("_NAME") else "l"
        e = {"k": m["key"], "t": m["kind"], "a": a, "segs": segs}
        if m["rs"]:
            e["rs"] = m["rs"]
        if m["kind"] == "t" and m["key"] in tog:
            e["hide"] = tog[m["key"]]
        keep.append(e)
    for key, caption in opts.get("extra", []):
        keep += caption_lines(probe_pdf, key, caption, drop)
    for key, start, stop, labels in opts.get("lines", []):
        keep = [e for e in keep if not re.match(re.escape(key) + r"#\d+", e["k"].lstrip("^"))]   # the lines replace them
        keep += lines_between(probe_pdf, key, start, stop, labels, drop)
    H = {i: p[1] for i, p in enumerate([p for i, p in enumerate(pages) if i not in drop])}
    for e in keep:
        for sg in e["segs"]:
            sg["_H"] = H[sg["p"]]
    moved = correct(probe_pdf, dflt_pdf, keep, drop)
    for e in keep:
        for sg in e["segs"]:
            sg.pop("_H", None)
    item_groups(final_pdf, keep)
    meta["pages"] = [p for i, p in enumerate(pages) if i not in drop]
    meta["markers"] = keep
    json.dump(meta, open(os.path.join(workdir, f"{num}.json"), "w"), separators=(",", ":"))
    missing = [m["key"] for m in markers if not m.get("segs") and m["kind"] != "sig"]
    print(num, "pages", len(meta["pages"]), "markers", len(markers), "placed", len(keep), "moved", moved, "missing", len(missing), missing[:8])
    return meta


def caption_lines(probe_pdf, key, caption, drop):
    """The line just above a caption like "(Name of Establishment)": the answer goes on it, in the middle."""
    out = []
    with pdfplumber.open(probe_pdf) as pdf:
        for pi, page in enumerate(pdf.pages):
            if pi in drop:
                continue
            text = page.extract_text_lines()
            for ln in text:
                if caption not in ln["text"]:
                    continue
                k = ln["text"].index(caption)
                chs = ln["chars"][k:k + len(caption)]
                cx = (chs[0]["x0"] + chs[-1]["x1"]) / 2
                hs = [e for e in page.edges if e["orientation"] == "h" and e["top"] <= chs[0]["top"] + 1
                      and chs[0]["top"] - e["top"] < 16]
                hs = [e for e in hs if e["x0"] - 40 <= cx <= e["x1"] + 40]
                if not hs:
                    continue
                top = max(e["top"] for e in hs)
                row = [e for e in hs if abs(e["top"] - top) < 0.3]
                x0, x1 = min(e["x0"] for e in row), max(e["x1"] for e in row)
                H = page.height
                out.append({"k": key, "t": "u", "a": "c", "segs": [{"p": pi - sum(1 for d in drop if d < pi),
                            "x0": round(x0, 2), "x1": round(x1, 2), "y": round(H - top + 2.5, 2), "s": round(chs[0]["size"], 2),
                            "xl": round(x0, 2), "xr": round(x1, 2)}]})
    return out


def text_at(page, text, after=None):
    for ln in page.extract_text_lines():
        if text in ln["text"] and (after is None or ln["top"] > after):
            k = ln["text"].index(text)
            chs = ln["chars"][k:k + len(text)]
            return {"x0": chs[0]["x0"], "x1": chs[-1]["x1"], "top": min(c["top"] for c in chs),
                    "bottom": max(c["bottom"] for c in chs), "s": chs[0]["size"]}
    return None


def lines_between(probe_pdf, key, start, stop, labels, drop):
    """Empty lines of the form (no marker) between two texts, e.g. ITEM I: ... ITEM V: of the defects.
    A line is a drawn line or a row of "____". Every line becomes key#1, key#2 ... ; "g" = which ITEM it belongs to."""
    out = []
    with pdfplumber.open(probe_pdf) as pdf:
        for pi, page in enumerate(pdf.pages):
            a = text_at(page, start)
            if a is None or pi in drop:
                continue
            H = page.height
            b = text_at(page, stop, a["top"])
            bottom = (b["top"] + b["bottom"]) / 2 - 4 if b else H - 40
            topline = (a["top"] + a["bottom"]) / 2 - 4
            labs = [lb for lb in (text_at(page, t, a["top"] - 3) for t in labels) if lb]
            size = min(9, (labs[0] if labs else a)["s"])
            hl = []
            for e in sorted([e for e in page.edges if e["orientation"] == "h"], key=lambda e: (round(e["top"], 1), e["x0"])):
                if hl and abs(hl[-1]["top"] - e["top"]) < 0.3 and e["x0"] <= hl[-1]["x1"] + 1.5:
                    hl[-1]["x1"] = max(hl[-1]["x1"], e["x1"])
                else:
                    hl.append({"x0": e["x0"], "x1": e["x1"], "top": e["top"], "y": H - e["top"] + 2, "s": size})
            for ln in page.extract_text_lines():
                run = []
                for c in ln["chars"] + [None]:      # rows of "____" (also after a label: "ITEM IV: ____")
                    if c is not None and c["text"] == "_" and (not run or abs(c["x0"] - run[-1]["x1"]) < 1):
                        run.append(c)
                        continue
                    if len(run) >= 15:
                        c0 = run[0]
                        hl.append({"x0": c0["x0"], "x1": run[-1]["x1"], "top": H - c0["matrix"][5] + 0.1 * c0["size"],
                                   "y": c0["matrix"][5], "s": min(9, c0["size"])})
                    run = [c] if c is not None and c["text"] == "_" else []
            hl = [h for h in hl if topline <= h["top"] < bottom and h["x1"] - h["x0"] > 100 and h["x1"] > a["x0"] + 100]
            n = 0
            for h in sorted(hl, key=lambda h: h["top"]):
                g = sum(1 for lb in labs if (lb["top"] + lb["bottom"]) / 2 - 4 <= h["top"])
                x0 = max(h["x0"], labs[g - 1]["x1"] + 4) if g else h["x0"]
                n += 1
                e = {"k": f"{key}#{n}", "t": "u", "a": "l", "segs": [{"p": pi - sum(1 for d in drop if d < pi),
                     "x0": round(x0, 2), "x1": round(h["x1"], 2), "y": round(h["y"], 2), "s": round(h["s"], 2),
                     "xl": round(x0, 2), "xr": round(h["x1"], 2)}]}
                out.append(e)
            if out:
                break
    return out


def item_groups(pdf_path, markers):
    """Long answers (key#1, key#2 ...) next to labels "ITEM I:", "ITEM II:" ...: remember which ITEM each line is."""
    rx = re.compile(r"^ITEM\s+([IVX]+)\s*:?$")
    labels = {}
    with pdfplumber.open(pdf_path) as pdf:
        for pi, page in enumerate(pdf.pages):
            for ln in page.extract_text_lines():
                words = ln["text"].split()
                for k in range(len(words) - 1):
                    if words[k] == "ITEM" and rx.match(words[k] + " " + words[k + 1]):
                        t = text_at(page, words[k] + " " + words[k + 1], ln["top"] - 1)
                        if t:
                            labels.setdefault(pi, []).append((rx.match(words[k] + " " + words[k + 1]).group(1), t, page.height))
    for m in markers:
        if not re.match(r"^defects#\d+$", m["k"]):    # ITEM I, II ... are only for the defects
            continue
        s = m["segs"][0]
        cands = [(name, t, H) for name, t, H in labels.get(s["p"], []) if t["x1"] <= s["x0"] + 6
                 and (t["top"] + t["bottom"]) / 2 - 4 <= H - s["y"] + 2]
        if cands:
            m["g"] = max(cands, key=lambda c: c[1]["top"])[0]


OPTS = {
    "006": {"fit": 1, "lines": [("rec.requirements", "State below the identified", "Fire Safety Evaluation Clearance", [])]},
    "061": {"lines": [("defects", "DEFECTS/DEFICIENCIES", "REMARKS:", ["ITEM I:", "ITEM II:", "ITEM III:", "ITEM IV:", "ITEM V:"])]},
    "068": {"fit": 1, "paper": 14, "lines": [
        ("defects", "DEFECTS/DEFICIENCIES", "REMARKS:", ["ITEM I:", "ITEM II:", "ITEM III:", "ITEM IV:", "ITEM V:", "ITEM VI:", "ITEM VII:"]),
        ("remarks", "REMARKS:", "RECOMMENDATIONS", [])]},
    "021": {"extra": [("SUBJECT_NAME", "(Name of Establishment)")]},
    "026": {"extra": [("SUBJECT_NAME", "(Name of Establishment)")], "squeeze": 2},
    "064": {"drop": [1]},
    "024": {"lines": [("defects", "DEFECTS/DEFICIENCIES", "RECOMMENDATIONS", ["ITEM IV:", "ITEM V:", "ITEM VI:", "ITEM VII:", "ITEM VIII:"])]},
}

if __name__ == "__main__":
    num, template, workdir = sys.argv[1:4]
    o = dict(OPTS.get(num, {}))
    if len(sys.argv) > 4 and sys.argv[4]:
        o["drop"] = [int(x) for x in sys.argv[4].split(",")]
    build(num, template, workdir, o)
