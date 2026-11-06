"""Python twin of the C# PDF writer: base PDF + JSON + values -> filled PDF (incremental update)."""
import re, sys, os, json, math

HELV = [int(x) for x in open(os.path.join(os.path.dirname(__file__), "helv_widths.txt")).read().split(",")]
CHECK = "√"
CP1252 = {0x20AC: 0x80, 0x201A: 0x82, 0x0192: 0x83, 0x201E: 0x84, 0x2026: 0x85, 0x2020: 0x86, 0x2021: 0x87, 0x02C6: 0x88,
          0x2030: 0x89, 0x0160: 0x8A, 0x2039: 0x8B, 0x0152: 0x8C, 0x017D: 0x8E, 0x2018: 0x91, 0x2019: 0x92, 0x201C: 0x93,
          0x201D: 0x94, 0x2022: 0x95, 0x2013: 0x96, 0x2014: 0x97, 0x02DC: 0x98, 0x2122: 0x99, 0x0161: 0x9A, 0x203A: 0x9B,
          0x0153: 0x9C, 0x017E: 0x9E, 0x0178: 0x9F}
SIG_W, SIG_H = 93.6, 27.4


def code(ch):
    o = ord(ch)
    if 32 <= o < 127 or 160 <= o <= 255:
        return o
    return CP1252.get(o, 63)   # '?'


def tw(text, size):
    return sum(HELV[code(c) - 32] if code(c) >= 32 else 278 for c in text) * size / 1000


def pdfstr(text):
    out = bytearray(b"(")
    for c in text:
        b = code(c)
        if b in (0x28, 0x29, 0x5C):
            out += b"\\" + bytes([b])
        else:
            out.append(b)
    return bytes(out + b")")


def num(v):
    s = ("%.2f" % v).rstrip("0").rstrip(".")
    return s if s not in ("-0", "") else "0"


class Page:
    def __init__(self):
        self.ops = []

    def text(self, x, y, size, t):
        self.ops.append(b"BT /FsiHelv " + num(size).encode() + b" Tf " + num(x).encode() + b" " + num(y).encode() +
                        b" Td " + pdfstr(t) + b" Tj ET")

    def check(self, cx, cy, size):
        # check mark drawn with lines (not a font), so it looks the same in every PDF viewer
        pts = [(-0.45, 0.02), (-0.12, -0.40), (0.48, 0.47)]
        w = max(0.6, 0.15 * size)
        p = ["%s %s" % (num(cx + x * size), num(cy + y * size - 0.03 * size)) for x, y in pts]
        self.ops.append(("q %s w 1 J 1 j %s m %s l %s l S Q" % (num(w), p[0], p[1], p[2])).encode())

    def line(self, x0, x1, y, w):
        self.ops.append(("%s w %s %s m %s %s l S" % (num(w), num(x0), num(y), num(x1), num(y))).encode())


def slots_of(m, center):
    out = []
    for s in m["segs"]:
        f = m.get("rs") or s["s"]
        x0, x1, xl, xr = s["x0"], s["x1"], s["xl"], s["xr"]
        t, a = m["t"], m["a"]
        if center or t == "t":
            c = (x0 + x1) / 2
            room = 2 * min(c - xl, xr - c) - 4
            w = (x1 - x0 - 2) if t == "u" else room
            out.append(dict(seg=s, f=f, al="c", x=c, w=max(w, 1), wmax=max(room, w, 1)))
        elif t == "u":
            out.append(dict(seg=s, f=f, al="l", x=x0 + 2, w=x1 - x0 - 3, wmax=max(x1 - x0 - 3, xr - x0 - 4)))
        elif a == "c":
            c = (x0 + x1) / 2
            w = 2 * min(c - xl, xr - c) - 4
            out.append(dict(seg=s, f=f, al="c", x=c, w=w, wmax=w))
        elif a == "r":
            out.append(dict(seg=s, f=f, al="r", x=x1, w=x1 - xl - 3, wmax=x1 - xl - 3))
        else:
            # may move to the left if there is room (pananglitan "1.2 (m)" sa gamay nga cell)
            out.append(dict(seg=s, f=f, al="l", x=x0, w=xr - x0 - 3, wmax=max(xr - x0 - 3, xr - xl - 4), lo=xl + 2, hi=xr - 1.5))
    return out


def draw_line(pages, sl, text, scale):
    s = sl["seg"]
    f = sl["f"] * scale
    w = tw(text, f)
    if sl["al"] == "c":
        x = sl["x"] - w / 2
    elif sl["al"] == "r":
        x = sl["x"] - w
    else:
        x = sl["x"]
        if "hi" in sl and x + w > sl["hi"]:
            x = max(sl["lo"], sl["hi"] - w)
    pg = pages[s["p"]]
    pg.text(x, s["y"], f, text)
    # on a line "____": if the answer is longer than the line, continue the line under it
    if s.get("_u") and x + w > s["x1"]:
        pg.line(s["x1"] - 0.5, x + w + 1, s["y"] - 0.17 * s["s"], 0.05 * s["s"])


def paragraphs(text):
    return [" ".join(p.split()) for p in text.replace("\r", "").split("\n") if p.strip()]


def flow(pages, slots, paras):
    """Write the text on the lines (slots). Every paragraph starts on a new line.
    If it does not fit, the letters get smaller (down to 62%)."""
    if not paras or not slots:
        return
    if len(slots) == 1:
        text = " ".join(paras)
        sl = slots[0]
        w = tw(text, sl["f"])
        scale = 1.0
        if w > sl["w"] and w > sl["wmax"]:
            scale = max(0.6, sl["wmax"] / w)
        draw_line(pages, sl, text, scale)
        return
    for scale in (1.0, 0.92, 0.85, 0.78, 0.7, 0.62):
        lines = [""] * len(slots)
        k, ok = 0, True
        for para in paras:
            words = para.split(" ")
            i = 0
            while i < len(words) and k < len(slots):
                sl = slots[k]
                nxt = (lines[k] + " " + words[i]) if lines[k] else words[i]
                if tw(nxt, sl["f"] * scale) <= sl["w"] or not lines[k]:
                    lines[k] = nxt; i += 1
                else:
                    k += 1
            if i < len(words):
                ok = False
                lines[-1] = (lines[-1] + " " + " ".join(words[i:])).strip()
            k += 1
        if ok:
            break
    for sl, ln in zip(slots, lines):
        if ln:
            draw_line(pages, sl, ln, scale)


ROMAN = re.compile(r"^\s*(?:ITEM\s+)?([IVX]{1,5})\s*[.:)\-\u2013]\s*", re.I)


def flow_long(pages, parts, text):
    """Long answer on many lines (defects#1, defects#2 ...).
    If the lines belong to ITEM I, ITEM II ... and a paragraph starts with "IV." / "ITEM IV:" ..., it goes to that ITEM."""
    paras = paragraphs(text)
    groups = []
    for m in parts:
        if m.get("g") and m["g"] not in groups:
            groups.append(m["g"])
    tagged = [ROMAN.match(p) for p in paras]
    if groups and any(t and t.group(1).upper() in groups for t in tagged):
        buckets = {g: [] for g in groups}
        cur = -1
        for p, t in zip(paras, tagged):
            if t and t.group(1).upper() in groups:
                cur = groups.index(t.group(1).upper())
                p = p[t.end():]
            else:
                cur = max(cur, 0)      # walay "IV." sa sugod: padayon sa samang ITEM
            buckets[groups[cur]].append(p)
        for g in groups:
            slots = []
            for m in parts:
                if m.get("g") == g:
                    slots += slots_of(m, False)
            flow(pages, slots, buckets[g])
        return
    slots = []
    for m in parts:
        slots += slots_of(m, False)
    flow(pages, slots, paras)


def draw_check(pages, m):
    s = m["segs"][0]
    pg = pages[s["p"]]
    if m["t"] == "s":
        # [   ]: in the middle between the brackets
        cx, cy = (s["x0"] + s["x1"]) / 2, s["y"] + 0.26 * s["s"]
        size = max(0.5 * s["s"], min(0.6 * s["s"], (s["x1"] - s["x0"]) + 0.05 * s["s"]))
    elif "box" in s:
        # a small box or table cell: in the middle of the box
        bx0, by0, bx1, by1 = s["box"]
        cx, cy = (bx0 + bx1) / 2, (by0 + by1) / 2
        size = min(0.75 * min(bx1 - bx0, by1 - by0), 0.9 * s["s"])
    else:
        cx, cy = (s["x0"] + s["x1"]) / 2, s["y"] + 0.3 * s["s"]
        size = 0.6 * s["s"]
    pg.check(cx, cy, size)


def draw_signature(pages, seg, strokes, aspect):
    cx = (seg["x0"] + seg["x1"]) / 2
    bottom = seg["y"] + 0.1 * seg["s"]
    # the box of the signature: 1.3 x 0.38 inch, smaller if there is little room above the name
    bh = max(12, min(SIG_H, seg.get("up", 60) - 0.1 * seg["s"] - 1.5))
    bw = SIG_W * bh / SIG_H
    sc = min(bw, bh / max(aspect, 0.01))
    ox = cx - sc / 2
    oy = bottom + (bh - sc * aspect) / 2
    ops = ["q 0.04 0.08 0.24 RG 0.9 w 1 J 1 j"]
    for st in strokes:
        if not st:
            continue
        pts = [(ox + p[0] * sc, oy + (aspect - p[1]) * sc) for p in st]
        ops.append("%s %s m" % (num(pts[0][0]), num(pts[0][1])))
        if len(pts) == 1:
            ops.append("%s %s l" % (num(pts[0][0] + 0.1), num(pts[0][1])))
        for x, y in pts[1:]:
            ops.append("%s %s l" % (num(x), num(y)))
        ops.append("S")
    ops.append("Q")
    pages[seg["p"]].ops.append(" ".join(ops).encode())


def fill(base_pdf, meta, values, signatures, out_pdf):
    pages = [Page() for _ in meta["pages"]]
    hide = []
    longs = {}
    for m in meta["markers"]:
        k = m["k"]
        center = k.startswith("^")
        key = re.sub(r"~\d+$", "", k.lstrip("^"))
        lm = re.match(r"^(.+)#(\d+)$", key)
        for s in m["segs"]:
            s["_u"] = m["t"] == "u"
        if lm:
            longs.setdefault(lm.group(1), []).append((int(lm.group(2)), m))
            continue
        v = values.get(key, "")
        if not v:
            continue
        hide += m.get("hide", [])
        if v == CHECK:
            draw_check(pages, m)
        else:
            flow(pages, slots_of(m, center), paragraphs(v))
    for key, parts in longs.items():
        v = values.get(key, "")
        if v:
            flow_long(pages, [m for _, m in sorted(parts, key=lambda t: t[0])], v)
    for role, (strokes, aspect) in signatures.items():
        for m in meta["markers"]:
            if m["k"] == "^" + role + "_NAME":
                draw_signature(pages, m["segs"][0], strokes, aspect)
    # ---- incremental update
    data = bytearray(open(base_pdf, "rb").read())
    if not data.endswith(b"\n"):
        data += b"\n"
    objs = {}
    for i, pg in enumerate(pages):
        if pg.ops:
            objs[meta["overlay"][i]] = b"q 0 g\n" + b"\n".join(pg.ops) + b"\nQ\n"
    for n in hide:
        objs[n] = b"3 Tr\n"
    offs = {}
    for n in sorted(objs):
        offs[n] = len(data)
        body = objs[n]
        data += b"%d 0 obj\n<< /Length %d >>\nstream\n" % (n, len(body)) + body + b"\nendstream\nendobj\n"
    xref = len(data)
    data += b"xref\n"
    for n in sorted(objs):
        data += b"%d 1\n%010d 00000 n \n" % (n, offs[n])
    tr = b"<< /Size %d /Root %d 0 R /Prev %d" % (meta["size"], meta["root"], meta["prev"])
    if meta.get("info"):
        tr += b" /Info %d 0 R" % meta["info"]
    if meta.get("id"):
        tr += b" /ID [<" + meta["id"][0].encode() + b"> <" + meta["id"][1].encode() + b">]"
    data += b"trailer\n" + tr + b" >>\nstartxref\n%d\n%%%%EOF\n" % xref
    open(out_pdf, "wb").write(data)


def sample_values(tid, review=False):
    sys.path.insert(0, os.path.dirname(__file__))
    from engine import targets_for
    v = {"STATION_NAME": "Tangub City Fire Station", "STATION_ADDRESS": "Tangub City, Misamis Occidental",
         "DATE_INSPECTED": "October 30, 2026", "REFERENCE_NO": "IO-2026-0142", "SUBJECT_NAME": "Mercado Bakery",
         "OWNER_NAME": "Juan Dela Cruz", "ADDRESS": "Brgy. Maloro, Tangub City", "CONTACT_NO": "0917 123 4567",
         "SIGNED_AT": "Oct 30, 2026 9:42 AM", "OWNER_REP_NAME": "JUAN DELA CRUZ", "INSPECTOR_NAME": "FO1 PRINCE DATOY",
         "TEAM_LEADER_NAME": "SFO2 MARIA SANTOS", "CHIEF_FSES_NAME": "SFO4 JOSE REYES", "FIRE_MARSHAL_NAME": "FSINSP ANA LIM",
         "BUILDING_PLAN_EVALUATOR_NAME": "FO3 CARLO DIAZ"}
    seen = set()
    for t in targets_for(tid):
        if t.kind == "T" and review:
            lab = (t.q.split(":")[-1].strip() if not t.opt else t.opt)[:22]
            v.setdefault(t.marker, "«" + lab + "»")
        if t.kind == "T" and not t.opt:
            v.setdefault(t.marker, "Sample")
        if t.kind == "C":
            base = t.marker.split("=")[0]
            if "=" in t.marker and base not in seen:
                v[t.marker] = CHECK; seen.add(base)
            elif "=" not in t.marker:
                v[t.marker] = CHECK
    for k in list(v):
        if k in ("defects", "recommendations", "rec.clearances", "rec.requirements", "sectional_occupancy") or k.startswith("sectional."):
            v[k] = ("Fire extinguisher at the kitchen is not charged and has no tag. Exit sign at the rear door is not "
                    "illuminated. Electrical wiring near the oven is exposed and must be enclosed in conduit.")
    return v


def fake_signature():
    pts = [[0.05 + i * 0.022, 0.15 + 0.1 * math.sin(i / 2.2) * (1 if i % 7 else 0.4)] for i in range(42)]
    loop = [[0.3 + 0.08 * math.cos(a / 6), 0.14 + 0.09 * math.sin(a / 6)] for a in range(40)]
    return [pts, loop], 0.3


if __name__ == "__main__":
    n, d, tid = sys.argv[1], sys.argv[2], sys.argv[3]
    meta = json.load(open(f"{d}/{n}.json"))
    vals = sample_values(tid, os.environ.get("REVIEW") == "1")
    flt = os.environ.get("ONLY")
    if flt:
        vals = {k: v for k, v in vals.items() if re.search(flt, k)}
    sig = fake_signature()
    sigs = {r: sig for r in ("OWNER_REP", "INSPECTOR", "TEAM_LEADER", "CHIEF_FSES", "FIRE_MARSHAL", "BUILDING_PLAN_EVALUATOR")}
    out = f"{d}/{n}_filled.pdf"
    fill(f"{d}/{n}.pdf", meta, vals, sigs, out)
    print(out)
