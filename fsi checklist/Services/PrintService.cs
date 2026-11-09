// =====================================================================
// PRINT: himoon nga PDF ang tinuod nga BFP form, puno sa mga tubag, check, printed name ug pirma.
//
// Para sa matag form naay duha ka file sa Resources/Raw/print/:
//   NNN.pdf   = ang orihinal nga form nga BLANGKO (pareho gyud sa Word/Excel nga gihatag sa BFP)
//   NNN.json  = asa ibutang ang matag tubag: page, x, y, gidak-on sa letra, kahon sa check, linya ...
//
// Ang app DILI mo-usab sa orihinal nga form. Magdugang lang kini og "overlay" (mga sulat ibabaw sa form)
// gamit ang "incremental update" sa PDF (idugang sa katapusan sa file). Busa dili gyud mausab o mabungkag
// ang layout sa form, ug walay dugang nga library.
//
// Ang NNN.docx / NNN.xlsx (nga naay {{marker}}) sa print folder mao ang gigikanan sa NNN.pdf ug NNN.json.
// Ang mga key sa JSON pareho sa mga marker:
//   plate_no            tubag sa "plate_no"
//   ^OWNER_REP_NAME     printed name (sa tunga sa linya); ang pirma ibutang ibabaw niini
//   cond.01.result=YES  check (√) kung YES ang tubag
//   a05.particulars=*   check (√) kung naay tubag (bisan unsa)
//   rec.issue.any       check (√) kung naay naka-check sa rec.issue.*
//   defects#1, #2 ...   taas nga tubag nga gibahin sa daghang linya
// =====================================================================
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using fsi_checklist.Controls;
using fsi_checklist.Data.Models;

namespace fsi_checklist.Services;

public sealed class PrintService
{
    private readonly InspectionService _inspections;
    private readonly AppSettingsService _settings;

    // Ang bili sa tubag nga "naka-check" (BOOL nga true, o {{key=VALUE}} nga mitugma).
    private const string Check = "√";

    public PrintService(InspectionService inspections, AppSettingsService settings)
    {
        _inspections = inspections;
        _settings = settings;
    }

    /// <summary>Himoon ang napuno nga PDF ug ablihan para i-print. Mobalik og error message, o null kung OK.</summary>
    public async Task<string?> PrintAsync(string inspectionId)
    {
        var form = await _inspections.LoadFormAsync(inspectionId);
        if (form == null)
            return "This inspection no longer exists.";

        // "BFP-QSF-FSED-064" -> "print/064.pdf" ug "print/064.json"
        string number = form.Inspection.Template.Code.Split('-').Last().Trim();
        string pdfFile = $"print/{number}.pdf";
        string mapFile = $"print/{number}.json";
        if (!await FileSystem.AppPackageFileExistsAsync(pdfFile) || !await FileSystem.AppPackageFileExistsAsync(mapFile))
            return $"No print template yet for {form.Inspection.Template.Code}.";

        var signoff = await _inspections.LoadSignoffAsync(inspectionId);
        var values = await BuildValuesAsync(form, signoff);
        var signatures = await LoadSignaturesAsync(signoff);   // role -> mga linya sa pirma

        // Basaha ang blangko nga form (PDF) ug ang mapa (JSON).
        byte[] blank;
        await using (var s = await FileSystem.OpenAppPackageFileAsync(pdfFile))
        using (var ms = new MemoryStream())
        {
            await s.CopyToAsync(ms);
            blank = ms.ToArray();
        }
        string mapJson;
        await using (var s = await FileSystem.OpenAppPackageFileAsync(mapFile))
        using (var r = new StreamReader(s))
            mapJson = await r.ReadToEndAsync();

        var map = JsonSerializer.Deserialize<PrintMap>(mapJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (map == null || map.Overlay.Count == 0)
            return "The print template is damaged.";

        // Isulat ang mga tubag ibabaw sa form.
        // LITRATO: mga litrato sa item nga naa sa map.Photos (pananglitan "defects")
        var photos = new List<byte[]>();
        if (map.Photos is { Items.Count: > 0 } ph)
        {
            var ids = form.Items.Where(x => ph.Items.Contains(x.Key)).Select(x => x.Id).ToHashSet();
            foreach (var a in form.Photos.Where(x => x.ItemId != null && ids.Contains(x.ItemId)).OrderBy(x => x.TakenAt))
            {
                string path = InspectionService.PhotoFullPath(a.FilePath);
                if (File.Exists(path))
                    photos.Add(await File.ReadAllBytesAsync(path));
            }
        }

        byte[] pdf = PdfFormWriter.Fill(blank, map, values, signatures, photos);

        // NGALAN SA FILE: Owner_Establishment_Oras_Petsa.pdf (pananglitan "kurt_kurtstore_11-28AM_11-06-2026.pdf").
        // Ang mga simbolo nga dili pwede sa ngalan sa file (\ / : * ? " < > |) gipulihan og "_".
        static string Safe(string? t) =>
            Regex.Replace(Regex.Replace((t ?? "").Trim(), @"[^\w\-\. ]", "_"), @"\s+", " ").Trim().Replace(' ', '-');
        var now = DateTime.Now;
        var subject = form.Inspection.Subject;
        // Max 40 ka letra matag bahin: aron dili molapas sa limit sa taas sa path sa Windows/Android.
        static string Cut(string t) => t.Length > 40 ? t[..40].TrimEnd('-', '_', '.') : t;
        string owner = Cut(Safe(subject?.OwnerName)), place = Cut(Safe(subject?.Name));
        if (owner.Length == 0)
            owner = "Owner";
        if (place.Length == 0)
            place = Cut(Safe(form.Inspection.ReferenceNo)) is { Length: > 0 } r ? r : $"FSED-{number}";
        string fileName = $"{owner}_{place}_{now.ToString("hh-mmtt", CultureInfo.InvariantCulture)}_{now:MM-dd-yyyy}.pdf";
        string outPath = Path.Combine(FileSystem.CacheDirectory, fileName);
        await File.WriteAllBytesAsync(outPath, pdf);

        AppLog.Info("Print file: " + outPath);
        await Launcher.OpenAsync(new OpenFileRequest("Print", new ReadOnlyFile(outPath, "application/pdf")));
        return null;
    }

    // ---------------------------------------------------------------- values

    // Tanang tubag ug ang ilang bili (key = pareho sa marker sa template).
    private async Task<Dictionary<string, string>> BuildValuesAsync(FormData form, SignoffData? signoff)
    {
        var v = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var i = form.Inspection;
        var en = CultureInfo.InvariantCulture;

        // ----- ulohan -----
        v["STATION_NAME"] = await _settings.GetAsync(AppSettingsService.StationName);
        v["STATION_ADDRESS"] = await _settings.GetAsync(AppSettingsService.StationAddress);
        v["DATE_INSPECTED"] = i.DateInspected?.ToString("MMMM d, yyyy", en) ?? "";
        v["REFERENCE_NO"] = i.ReferenceNo ?? "";
        v["SUBJECT_NAME"] = i.Subject.Name;
        v["OWNER_NAME"] = i.Subject.OwnerName ?? "";
        v["ADDRESS"] = i.Subject.Address ?? "";
        v["CONTACT_NO"] = i.Subject.ContactNo ?? "";

        // ----- mga tubag -----
        var items = form.Items.ToDictionary(x => x.Id);

        // Ang tubag sa pangutana nga NAKATAGO (show_if) dili i-print.
        // Pananglitan: gipili ang Re-inspection + NTC, dayon giusab ngadto sa Others: ang NTC dili na mogawas.
        // (item, slot) -> tubag. Ang slot mahimo nga "value" o column (pananglitan change_status).
        var mainAnswers = form.Answers.Where(x => x.RepeatNo == 0)
            .GroupBy(x => (x.ItemId, x.SlotKey))
            .ToDictionary(g => g.Key, g => g.First().ValueText);
        bool IsShown(FormItem it, int depth)
        {
            if (depth > 20)
                return true;
            if (!string.IsNullOrEmpty(it.ShowIfItemId))
            {
                var (slot, values) = ShowIfRule.Parse(it.ShowIfValues);
                mainAnswers.TryGetValue((it.ShowIfItemId, slot), out var current);
                bool match = ShowIfRule.Matches(current, values);
                if (!match)
                    return false;
            }
            return it.ParentItemId == null || !items.TryGetValue(it.ParentItemId, out var up) || IsShown(up, depth + 1);
        }

        foreach (var a in form.Answers.Where(x => x.RepeatNo == 0))
        {
            if (!items.TryGetValue(a.ItemId, out var item) || !IsShown(item, 0))
                continue;

            // {{key}} para sa main nga tubag, {{key.column}} para sa column (pananglitan cond.01.result)
            string key = a.SlotKey == "value" ? item.Key : item.Key + "." + a.SlotKey;
            string raw = a.ValueText ?? a.ValueNum?.ToString("G", en) ?? "";

            string dataType = item.DataType;
            string ownerId = item.Id;
            if (a.SlotKey != "value")
            {
                var col = form.Columns.FirstOrDefault(c => c.ColumnSetId == item.ColumnSetId && c.Key == a.SlotKey);
                dataType = col?.DataType ?? "TEXT";
                ownerId = col?.Id ?? "";
            }

            v[key] = dataType switch
            {
                "BOOL" => raw is "1" or "True" or "true" ? Check : "",
                "DATE" => DateTime.TryParse(raw, en, DateTimeStyles.None, out var d) ? d.ToString("MMMM d, yyyy", en) : raw,
                "YES_NO" or "YES_NO_NA" or "PASS_FAIL" => raw switch
                {
                    "YES" => "Yes", "NO" => "No", "NA" => "N/A", "PASS" => "Passed", "FAIL" => "Failed", _ => raw,
                },
                "CHOICE" => form.Choices.FirstOrDefault(c => (c.ItemId ?? c.ColumnId) == ownerId && c.Value == raw)?.Label ?? raw,
                _ => raw,
            };

            // {{key=VALUE}} = √ (para sa Yes/No, Passed/Failed, ug pilian)
            if (!string.IsNullOrEmpty(raw))
                v[key + "=" + raw] = Check;

            // CHOICE nga gikan sa daan nga mga checkbox (pananglitan "For Issuance of": rec.issue = closure_order)
            // -> √ sa daan nga marker rec.issue.closure_order (ug rec.issue.any sa ubos)
            if (dataType == "CHOICE" && !string.IsNullOrEmpty(raw) && raw == raw.ToLowerInvariant())
                v[key + "." + raw] = Check;
        }

        // SPECIFY: kung naay "xxx.specify" (pananglitan Special Structure -> Please specify),
        // i-print uban sa pilian: "Special Structure (Tower)"
        foreach (var (k, spec) in v.ToList())
        {
            if (!k.EndsWith(".specify") || string.IsNullOrWhiteSpace(spec))
                continue;
            string main = k[..^".specify".Length];
            if (v.TryGetValue(main, out var mv) && !string.IsNullOrWhiteSpace(mv))
                v[main] = $"{mv} ({spec.Trim()})";
        }

        // N/A: kung "N/A" ang numero (Fire Drill, Fire Insurance, FSEC, Permit, FSIC...), "N/A" pud ang iyang date issued
        foreach (var (no, date) in new[]
        {
            ("fire_drill_no", "fire_drill_date"), ("insurance_policy_no", "insurance_policy_date"),
            ("fsec_no", "fsec_date"), ("renovation_permit_no", "renovation_permit_date"),
            ("fsic_no", "fsic_date"), ("business_permit_no", "business_permit_date"),
        })
        {
            if (v.TryGetValue(no, out var nv) && string.Equals(nv.Trim(), "N/A", StringComparison.OrdinalIgnoreCase))
                v[date] = "N/A";
        }

        // ----- mga pirma: {{OWNER_REP_NAME}}, {{INSPECTOR_NAME}}, {{SIGNED_AT}} ... -----
        if (signoff != null)
        {
            foreach (var s in signoff.Signatories.Where(x => x.Status == "SIGNED"))
            {
                v[s.RoleCode + "_NAME"] = s.PrintedName;
                if (s.SignedAt is { } at)
                    v[s.RoleCode + "_SIGNED_AT"] = DateTime.SpecifyKind(at, DateTimeKind.Utc).ToLocalTime()
                        .ToString("MMM d, yyyy h:mm tt", en);
            }
            v["SIGNED_AT"] = v.GetValueOrDefault("OWNER_REP_SIGNED_AT", "");
        }

        // ----- mga kahon nga awtomatikong ma-check -----
        //   {{key=*}}   = √ kung naay tubag ang key (pananglitan [√] State Particular Violation kung naay gisulat)
        //   {{xxx.any}} = √ kung naay naka-check sa xxx.* (pananglitan [√] For Issuance of, kung naay rec.issue.* nga naka-check)
        foreach (var (k, val) in v.ToList())
        {
            if (string.IsNullOrEmpty(val) || k.Contains('='))
                continue;
            v[k + "=*"] = Check;
            int dot = k.LastIndexOf('.');
            if (val == Check && dot > 0)
                v[k[..dot] + ".any"] = Check;
        }

        return v;
    }

    // ---------------------------------------------------------------- signatures

    // Basahon ang pirma (JSON nga mga linya, gikan sa SignaturePath).
    private static async Task<Dictionary<string, SignatureJson.Dto>> LoadSignaturesAsync(SignoffData? signoff)
    {
        var result = new Dictionary<string, SignatureJson.Dto>();
        if (signoff == null)
            return result;
        foreach (var s in signoff.Signatories.Where(x => x.Status == "SIGNED" && !string.IsNullOrEmpty(x.SignaturePath)))
        {
            if (result.ContainsKey(s.RoleCode))
                continue;
            string path = Path.Combine(FileSystem.AppDataDirectory, s.SignaturePath!);
            if (!File.Exists(path))
                continue;
            try
            {
                var dto = JsonSerializer.Deserialize<SignatureJson.Dto>(await File.ReadAllTextAsync(path));
                if (dto != null && dto.Strokes.Count > 0)
                    result[s.RoleCode] = dto;
            }
            catch (Exception ex)
            {
                AppLog.Error(ex, "Signature");
            }
        }
        return result;
    }
}


// ===================================================================== the map (print/NNN.json)

/// <summary>Ang print/NNN.json: unsa nga mga object sa PDF ang usbon, ug asa ibutang ang matag tubag.</summary>
internal sealed class PrintMap
{
    public List<int> Overlay { get; set; } = new();     // matag page: ang numero sa blangko nga "overlay" stream
    public int Root { get; set; }
    public int Info { get; set; }
    public List<string> Id { get; set; } = new();
    public int Size { get; set; }
    public long Prev { get; set; }                       // asa sugod ang xref sa blangko nga PDF
    public List<double[]> Pages { get; set; } = new();
    public List<PrintMarker> Markers { get; set; } = new();

    // LISTAHAN (pananglitan rec.clearances): matag tubag = usa ka linya. Kung kulang ang linya sa form,
    // ang uban isulat sa DUGANG NGA PAGE sa katapusan (key -> ulohan sa dugang nga page).
    public Dictionary<string, PrintList> Lists { get; set; } = new();
    public int Tree { get; set; }                        // numero sa /Pages object (para sa dugang nga page)
    public int InsertAt { get; set; } = -1;              // asa isulod ang dugang nga page (-1 = sa katapusan)
    public PrintPhotos? Photos { get; set; }             // mga litrato nga i-print sa kaugalingon nga page
    public PrintFooter? Footer { get; set; }             // "Page N of M" (usbon kung naay dugang nga page)
    public List<int> Kids { get; set; } = new();         // mga page sa form (para sa dugang nga page)
}

// LISTAHAN (pananglitan "Secure the following Fire Safety Clearance/s"):
// kung molapas sa linya sa form, MODUGANG OG LINYA sa ubos (ang sulod sa ubos sa page ipaubos),
// hangtod moabot sa Floor. Kung puno na gyud, ang nahabilin -> dugang nga page.
internal sealed class PrintList
{
    public string Title { get; set; } = "";     // ulohan sa dugang nga page
    public int Page { get; set; }                // asa nga page (0 = una)
    public double Row { get; set; }              // gilay-on sa matag linya (pt)
    public double Cut { get; set; }              // y: ubos niini ang ipaubos (ibabaw sa "Pay the corresponding ...")
    public double Floor { get; set; }            // y: ibabaw sa footer (dili lapason)
    public double Bottom { get; set; }           // y: pinaka-ubos nga sulat sa page (ibabaw sa footer)
    public int Obj { get; set; }                 // numero sa page object
    public string Dict { get; set; } = "";       // ang page object, %CONTENTS% = asa ibutang ang /Contents
    public List<int> Contents { get; set; } = new();   // mga content stream sa page (walay overlay)
}

// LITRATO: ang mga litrato sa mga item (pananglitan "defects") i-print sa dugang nga page (6 matag page).
internal sealed class PrintPhotos
{
    public List<string> Items { get; set; } = new();   // item key (pananglitan "defects")
    public string Title { get; set; } = "";
}

// FOOTER: ang "Page N of M" sa matag page sa form, ug ang footer sa dugang nga page.
internal sealed class PrintFooter
{
    public string Text { get; set; } = "";             // pananglitan "BFP-QSF-FSED-069 Rev. Ø1 (11.08.24)"
    public double X { get; set; }                      // sugod sa footer sa dugang nga page
    public double Y { get; set; }                      // baseline sa footer sa dugang nga page
    public double S { get; set; } = 10;
    public List<double[]> Pages { get; set; } = new(); // matag page: tabunan x0,y0,x1,y1, unya x ug baseline sa "Page"
}

// Usa ka dugang nga page (litrato o sobra nga listahan).
internal sealed class ExtraPg
{
    public StringBuilder Content { get; } = new();
    public List<byte[]> Images { get; } = new();       // JPEG (DCTDecode), /Im0 /Im1 ...
}

internal sealed class PrintMarker
{
    public string K { get; set; } = "";     // key (pareho sa marker)
    public string T { get; set; } = "";     // u = linya "____", s = [   ], e = blangko, t = naay sulat nga default
    public string A { get; set; } = "l";    // l / c / r = wala / tunga / tuo
    public double? Rs { get; set; }         // gidak-on sa letra sa form (pt)
    public List<int>? Hide { get; set; }    // mga sulat nga tagoan kung naay tubag, pananglitan "(Station)"
    public string? G { get; set; }          // ITEM I, II ... (para sa defects)
    public PrintCont? Cont { get; set; }    // kung taas kaayo ang tubag: ipadayon sa laing lugar (pananglitan page 2)
    public List<PrintSpot> Segs { get; set; } = new();
}

// PADAYON: kung dili mohaum ang tubag sa iyang mubo nga linya (pananglitan "Special Structure (water pump)"),
// ang pilian ra ang isulat sa linya + " *" (pananglitan "Special Structure *"), ug ang tibuok tubag isulat diri
// nga naay "*" sa sugod (pananglitan ibabaw sa page 2).
internal sealed class PrintCont
{
    public int P { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double W { get; set; }               // pinaka-lapad
    public string Label { get; set; } = "";     // pananglitan "3. Type of occupancy (cont.)"
    public double X2 { get; set; }              // kung > 0: ang tubag sa sunod nga column (parehas nga linya sa Label)

    // BAG-ONG ROW sa table: ipaubos ang sulod ubos sa Row.Cut og D, dayon i-drawing ang row (mga linya sa Cols).
    public PrintList? Row { get; set; }
    public double D { get; set; }               // gitas-on sa bag-ong row
    public List<double> Cols { get; set; } = new();   // x sa mga vertical nga linya sa table
}

internal sealed class PrintSpot
{
    public int P { get; set; }               // page (0 = una)
    public double X0 { get; set; }           // sugod sa linya/kahon
    public double X1 { get; set; }           // katapusan
    public double Y { get; set; }            // baseline sa sulat (gikan sa ubos sa page)
    public double S { get; set; }            // gidak-on sa letra
    public double Xl { get; set; }           // hangtod asa pwede moabot sa wala
    public double Xr { get; set; }           // hangtod asa pwede moabot sa tuo
    public double[]? Box { get; set; }       // kahon sa check: x0, y0, x1, y1
    public double? Up { get; set; }          // luwag ibabaw sa printed name (para sa pirma)
}


// ===================================================================== PDF writer

/// <summary>
/// Isulat ang mga tubag ibabaw sa blangko nga PDF form. Ang blangko nga PDF wala gi-usab:
/// ang bag-ong mga sulat gidugang sa katapusan sa file (PDF "incremental update").
/// </summary>
internal static class PdfFormWriter
{
    private const string Check = "√";
    private const double SigW = 93.6, SigH = 27.4;   // gidak-on sa pirma: 1.3 x 0.38 inch (pt)

    // Gilapdon sa matag letra sa Helvetica (WinAnsi 32..255), sa 1/1000 sa gidak-on sa letra.
    private static readonly short[] Helv =
    {
        278, 278, 355, 556, 556, 889, 667, 191, 333, 333, 389, 584, 278, 333, 278, 278,
        556, 556, 556, 556, 556, 556, 556, 556, 556, 556, 278, 278, 584, 584, 584, 556,
        1015, 667, 667, 722, 722, 667, 611, 778, 722, 278, 500, 667, 556, 833, 722, 778,
        667, 778, 722, 667, 611, 722, 667, 944, 667, 667, 611, 278, 278, 278, 469, 556,
        333, 556, 556, 500, 556, 556, 278, 556, 556, 222, 222, 500, 222, 833, 556, 556,
        556, 556, 333, 500, 278, 556, 500, 722, 500, 500, 500, 334, 260, 334, 584, 278,
        556, 0, 222, 556, 333, 1000, 556, 556, 333, 1000, 667, 333, 1000, 0, 611, 0,
        0, 222, 222, 333, 333, 350, 556, 1000, 333, 1000, 500, 333, 944, 0, 500, 667,
        278, 333, 556, 556, 556, 556, 260, 556, 333, 737, 370, 556, 584, 278, 737, 333,
        400, 584, 278, 278, 333, 556, 537, 278, 333, 278, 365, 556, 834, 834, 834, 611,
        667, 667, 667, 667, 667, 667, 1000, 722, 667, 667, 667, 667, 278, 278, 278, 278,
        722, 722, 778, 778, 778, 778, 778, 584, 778, 722, 722, 722, 722, 667, 667, 611,
        556, 556, 556, 556, 556, 556, 889, 500, 556, 556, 556, 556, 278, 278, 278, 278,
        556, 556, 556, 556, 556, 556, 556, 584, 611, 556, 556, 556, 556, 500, 556, 500
    };

    // Mga letra nga wala sa Latin-1 pero naa sa WinAnsi (pananglitan ang ’ “ ” – —).
    private static readonly Dictionary<char, byte> Win = new()
    {
        ['€'] = 0x80, ['‚'] = 0x82, ['ƒ'] = 0x83, ['„'] = 0x84, ['…'] = 0x85, ['†'] = 0x86,
        ['‡'] = 0x87, ['ˆ'] = 0x88, ['‰'] = 0x89, ['Š'] = 0x8A, ['‹'] = 0x8B, ['Œ'] = 0x8C,
        ['Ž'] = 0x8E, ['‘'] = 0x91, ['’'] = 0x92, ['“'] = 0x93, ['”'] = 0x94, ['•'] = 0x95,
        ['–'] = 0x96, ['—'] = 0x97, ['˜'] = 0x98, ['™'] = 0x99, ['š'] = 0x9A, ['›'] = 0x9B,
        ['œ'] = 0x9C, ['ž'] = 0x9E, ['Ÿ'] = 0x9F,
    };

    // "IV." / "ITEM IV:" / "iv -" sa sugod sa paragraph: para asa nga ITEM sa defects
    private static readonly Regex Roman = new(@"^\s*(?:ITEM\s+)?([IVX]{1,5})\s*[.:)\-–]\s*", RegexOptions.IgnoreCase);

    // Usa ka linya nga masulatan.
    private sealed class Slot
    {
        public PrintSpot Seg = null!;
        public double F;            // gidak-on sa letra
        public char Al = 'l';       // l / c / r
        public double X;            // sugod (l), tunga (c) o katapusan (r)
        public double W;            // gilapdon sa linya
        public double WMax;         // pinaka-lapad kung kulang ang linya
        public double Lo = double.NaN, Hi = double.NaN;   // pwede mobalhin sa wala hangtod Lo (kung lapas sa Hi)
        public bool Line;           // linya nga "____" (padayonon ang linya kung taas ang tubag)
    }

    public static byte[] Fill(byte[] blank, PrintMap map, Dictionary<string, string> values,
        Dictionary<string, SignatureJson.Dto> signatures, List<byte[]>? photos = null)
    {
        var pages = map.Pages.Select(_ => new StringBuilder()).ToList();
        var hide = new List<int>();
        var longs = new Dictionary<string, List<(int N, PrintMarker M)>>();

        // DUGANG NGA LINYA sa listahan: pila ka linya ang idugang, ug ipaubos ang sulod sa ubos niini.
        var shifts = new List<(PrintList L, double D)>();
        foreach (var (key, list) in map.Lists)
        {
            if (!values.TryGetValue(key, out var lv) || string.IsNullOrEmpty(lv))
                continue;
            var parts = map.Markers.Where(m => m.Segs.Count > 0 && Regex.IsMatch(m.K, "^" + Regex.Escape(key) + @"#\d+$"))
                .OrderBy(m => int.Parse(m.K[(m.K.LastIndexOf('#') + 1)..])).ToList();
            int over = Paragraphs(lv).Count - parts.Count;
            if (over <= 0 || parts.Count < 2 || list.Row <= 0)
                continue;
            // GAP: dugang nga luwag ubos sa bag-ong mga linya aron dili dasok ang "Pay the corresponding ..."
            double gap = list.Row * 0.6;
            int maxRows = (int)Math.Floor((list.Bottom - list.Floor - gap) / list.Row);
            int rows = Math.Min((over + 1) / 2, Math.Max(maxRows, 0));
            if (rows == 0)
                continue;
            double d = rows * list.Row + gap;
            // ipaubos ang tanang marker sa page nga ubos sa Cut (Pay the corresponding ..., pirma ...)
            foreach (var m in map.Markers)
                foreach (var sg in m.Segs.Where(x => x.P == list.Page && x.Y < list.Cut && x.Y > list.Floor))
                {
                    sg.Y -= d;
                    if (sg.Box is { Length: 4 } bx)
                        sg.Box = new[] { bx[0], bx[1] - d, bx[2], bx[3] - d };
                }
            // bag-ong mga linya: kopya sa katapusan nga row (2 ka column), ubos-ubos
            var last = parts.Skip(parts.Count - 2).ToList();
            int n = parts.Count;
            for (int r = 1; r <= rows; r++)
                foreach (var pm in last)
                {
                    var o = pm.Segs[0];
                    var seg = new PrintSpot { P = o.P, X0 = o.X0, X1 = o.X1, Y = o.Y - r * list.Row, S = o.S, Xl = o.Xl, Xr = o.Xr };
                    map.Markers.Add(new PrintMarker { K = $"{key}#{++n}", T = pm.T, A = pm.A, Rs = pm.Rs, Segs = { seg } });
                    pages[seg.P].Append("q 0.6 w ").Append(N(seg.X0)).Append(' ').Append(N(seg.Y - 1.5)).Append(" m ")
                        .Append(N(seg.X1)).Append(' ').Append(N(seg.Y - 1.5)).Append(" l S Q\n");
                }
            shifts.Add((list, d));
        }

        // BAG-ONG ROW para sa taas nga tubag (PrintCont.Row): pananglitan 069 "3. Type of occupancy (cont.)"
        // ibabaw sa "4. Means of Egress" sa page 2. Ang sulod sa ubos ipaubos og D.
        foreach (var cm in map.Markers.Where(x => x.Cont?.Row != null && x.Segs.Count == 1).ToList())
        {
            var ct = cm.Cont!;
            var row = ct.Row!;
            if (!values.TryGetValue(cm.K, out var cv) || string.IsNullOrWhiteSpace(cv) || row.Page >= pages.Count)
                continue;
            if (TextWidth(string.Join(" ", Paragraphs(cv)), Slots(cm, false)[0].F) * 0.85 <= Slots(cm, false)[0].W)
                continue;   // mohaum ra sa iyang linya
            double d = ct.D;
            if (row.Bottom - d < row.Floor)
                continue;   // walay luwag sa ubos sa page
            foreach (var m in map.Markers)
                foreach (var sg in m.Segs.Where(x => x.P == row.Page && x.Y < row.Cut && x.Y > row.Floor))
                {
                    sg.Y -= d;
                    if (sg.Box is { Length: 4 } bx)
                        sg.Box = new[] { bx[0], bx[1] - d, bx[2], bx[3] - d };
                }
            var pg = pages[row.Page];
            double top = row.Cut + 0.3, bot = row.Cut - d;
            pg.Append("q 0 G 0.48 w ");
            foreach (var x in ct.Cols)
                pg.Append(N(x)).Append(' ').Append(N(top)).Append(" m ").Append(N(x)).Append(' ').Append(N(bot)).Append(" l ");
            if (ct.Cols.Count > 1)
                pg.Append(N(ct.Cols[0])).Append(' ').Append(N(bot)).Append(" m ").Append(N(ct.Cols[^1])).Append(' ').Append(N(bot)).Append(" l ");
            pg.Append("S Q\n");
            shifts.Add((row, d));
        }

        // Taas nga "State particular violation": ibalhin sa VI. Defects aron klaro ug walay mawala.
        if (map.Markers.Any(x => x.K.StartsWith("defects#")))
            values = MoveLongParticularsToDefects(map, values);

        foreach (var m in map.Markers)
        {
            if (m.Segs.Count == 0)
                continue;
            bool center = m.K.StartsWith('^');
            string key = Regex.Replace(m.K.TrimStart('^'), @"~\d+$", "");

            // defects#1, defects#2 ...: tigumon una, isulat sa ubos
            var lm = Regex.Match(key, @"^(.+)#(\d+)$");
            if (lm.Success)
            {
                if (!longs.TryGetValue(lm.Groups[1].Value, out var list))
                    longs[lm.Groups[1].Value] = list = new List<(int, PrintMarker)>();
                list.Add((int.Parse(lm.Groups[2].Value), m));
                continue;
            }

            if (!values.TryGetValue(key, out var v) || string.IsNullOrEmpty(v))
                continue;   // walay tubag: ang form gihapon (linya o [  ])
            if (m.Hide != null)
                hide.AddRange(m.Hide);
            if (v == Check)
                DrawCheck(pages, m);
            else if (m.Cont is { } ct && m.Segs.Count == 1 && !center && Continue(pages, m, ct, string.Join(" ", Paragraphs(v))))
                continue;
            else
                Flow(pages, Slots(m, center), Paragraphs(v));
        }

        // PAREHO NGA GIDAK-ON SA LETRA: ang Defects (Item I, II ...), Remarks ug Recommendations
        // sa usa ka page kay usa ra ang gidak-on sa letra (ang pinaka-gamay nga gikinahanglan aron mohaum tanan).
        // Ang uban (pananglitan Sectional occupancy, Clearances) kaugalingon ra nga gidak-on.
        var jobs = new List<(bool Same, List<Slot> Slots, List<string> Paras)>();
        var extra = new List<(string Title, List<string> Lines)>();
        foreach (var (key, parts) in longs)
        {
            if (!values.TryGetValue(key, out var v) || string.IsNullOrEmpty(v))
                continue;
            if (map.Lists.TryGetValue(key, out var plist))
            {
                string title = plist.Title;
                // LISTAHAN: matag chip sa iyang linya; ang sobra -> dugang nga page
                var sl = parts.OrderBy(p => p.N).SelectMany(p => Slots(p.M, false)).ToList();
                var items = Paragraphs(v);
                OneEach(pages, sl, items.Take(sl.Count).ToList());
                if (items.Count > sl.Count)
                    extra.Add((title, items.Skip(sl.Count).ToList()));
                continue;
            }
            bool same = key.StartsWith("defects") || key == "remarks" || key == "recommendations";
            foreach (var (sl, ps) in FlowLong(parts.OrderBy(p => p.N).Select(p => p.M).ToList(), v))
                if (sl.Count > 0 && ps.Count > 0)
                    jobs.Add((same, sl, ps));
        }
        var size = jobs.Where(j => j.Same && j.Slots.Count > 1)
            .GroupBy(j => j.Slots[0].Seg.P)
            .ToDictionary(g => g.Key, g => g.Min(j => j.Slots[0].F * FitScale(j.Slots, j.Paras)));
        foreach (var j in jobs)
        {
            if (j.Same && j.Slots.Count > 1)
                Flow(pages, j.Slots, j.Paras, size[j.Slots[0].Seg.P] / j.Slots[0].F);
            else
                Flow(pages, j.Slots, j.Paras);
        }

        // Pirma: ibabaw sa printed name sa iyang role.
        foreach (var (role, sig) in signatures)
        {
            foreach (var m in map.Markers.Where(x => x.K == "^" + role + "_NAME" && x.Segs.Count > 0))
                DrawSignature(pages, m.Segs[0], sig);
        }

        // DUGANG NGA PAGE: (1) ang sobra sa listahan (pananglitan ika-15 nga clearance), (2) ang mga litrato
        var extras = new List<ExtraPg>();
        var pageSize = map.Pages.Count > 0 ? map.Pages[^1] : new double[] { 612, 936 };
        if (map.Tree > 0 && map.Kids.Count > 0)
        {
            if (extra.Count > 0)
            {
                var pg = new ExtraPg();
                ExtraPage(pg.Content, pageSize, extra);
                extras.Add(pg);
            }
            if (photos is { Count: > 0 } && map.Photos != null)
                extras.AddRange(PhotoPages(pageSize, map.Photos.Title, photos));
        }

        // PAGE NUMBER: kung naay dugang nga page, usbon ang "Page N of M" sa tanang page
        if (extras.Count > 0 && map.Footer is { } ft)
        {
            int total = map.Kids.Count + extras.Count;
            int at = map.InsertAt >= 0 && map.InsertAt <= map.Kids.Count ? map.InsertAt : map.Kids.Count;
            for (int i = 0; i < pages.Count && i < ft.Pages.Count; i++)
            {
                var c = ft.Pages[i];
                pages[i].Append("q 1 g ").Append(N(c[0])).Append(' ').Append(N(c[1])).Append(' ')
                    .Append(N(c[2] - c[0])).Append(' ').Append(N(c[3] - c[1])).Append(" re f Q\n");
                PageNumber(pages[i], c[4], c[5], ft.S, i < at ? i + 1 : i + 1 + extras.Count, total);
            }
            for (int j = 0; j < extras.Count; j++)
            {
                var sb = extras[j].Content;
                sb.Append("BT /FsiHelv ").Append(N(ft.S)).Append(" Tf ").Append(N(ft.X)).Append(' ').Append(N(ft.Y))
                  .Append(" Td ").Append(PdfString(ft.Text)).Append(" Tj ET\n");
                PageNumber(sb, ft.X + TextWidth(ft.Text + " ", ft.S), ft.Y, ft.S, at + j + 1, total);
            }
        }

        return Write(blank, map, pages, hide, extras, shifts);
    }

    // ------------------------------------------------------------ text

    private const string SeeDefects = "See VI (Defects)";

    // TAAS NGA VIOLATION (pananglitan 069 Item 4-12 "State particular violation"):
    // kung dili mohaum sa iyang linya bisan gamayon ang letra (80%), isulat sa linya ang "See VI (Defects)"
    // ug ang tibuok text ibutang sa VI. Defects/Deficiencies nga naay label ("Item 7: ...").
    // Mao ni aron dili mogamay kaayo o mosobra sa column ang tubag. Walay mawala.
    private static Dictionary<string, string> MoveLongParticularsToDefects(PrintMap map, Dictionary<string, string> values)
    {
        var v = new Dictionary<string, string>(values, StringComparer.OrdinalIgnoreCase);
        var extra = new List<string>();

        foreach (var m in map.Markers)
        {
            if (m.T != "u" || m.Segs.Count != 1 || m.K.StartsWith('^') || m.K.Contains('#') || m.K.Contains('='))
                continue;
            if (!m.K.Contains("particular") && !m.K.EndsWith(".location"))
                continue;
            if (!v.TryGetValue(m.K, out var text) || string.IsNullOrWhiteSpace(text))
                continue;

            // Ang mga numero sa assessment ra (a01 ... a13 sa 069), aron klaro ang label.
            var num = Regex.Match(m.K, @"^a0?(\d+)([ab]?)\.");
            if (!num.Success)
                continue;

            var slot = Slots(m, false)[0];
            string oneLine = string.Join(" ", Paragraphs(text));
            if (TextWidth(oneLine, slot.F * 0.8) <= slot.WMax)
                continue;   // mohaum pa (bisan gamay-gamay ang letra)

            string label = $"Item {num.Groups[1].Value}{num.Groups[2].Value}";
            extra.Add($"{label}: {oneLine}");
            v[m.K] = SeeDefects;
        }

        if (extra.Count > 0)
        {
            v.TryGetValue("defects", out var existing);
            v["defects"] = string.IsNullOrWhiteSpace(existing)
                ? string.Join("\n", extra)
                : existing.TrimEnd() + "\n" + string.Join("\n", extra);
        }
        return v;
    }

    // Ang mga linya nga masulatan sa usa ka marker.
    private static List<Slot> Slots(PrintMarker m, bool center)
    {
        var list = new List<Slot>();
        foreach (var s in m.Segs)
        {
            double f = m.Rs ?? s.S;
            bool line = m.T == "u";
            if (center || m.T == "t")
            {
                // sa tunga (printed name, ngalan sa station)
                double c = (s.X0 + s.X1) / 2;
                double room = 2 * Math.Min(c - s.Xl, s.Xr - c) - 4;
                double w = line ? s.X1 - s.X0 - 2 : room;
                list.Add(new Slot { Seg = s, F = f, Al = 'c', X = c, W = Math.Max(w, 1), WMax = Math.Max(Math.Max(room, w), 1), Line = line });
            }
            else if (line)
            {
                // ibabaw sa linya "____", sugod sa wala
                list.Add(new Slot { Seg = s, F = f, Al = 'l', X = s.X0 + 2, W = s.X1 - s.X0 - 3,
                    WMax = Math.Max(s.X1 - s.X0 - 3, s.Xr - s.X0 - 4), Line = true });
            }
            else if (m.A == "c")
            {
                double c = (s.X0 + s.X1) / 2;
                double w = 2 * Math.Min(c - s.Xl, s.Xr - c) - 4;
                list.Add(new Slot { Seg = s, F = f, Al = 'c', X = c, W = w, WMax = w });
            }
            else if (m.A == "r")
            {
                list.Add(new Slot { Seg = s, F = f, Al = 'r', X = s.X1, W = s.X1 - s.Xl - 3, WMax = s.X1 - s.Xl - 3 });
            }
            else
            {
                // pwede mobalhin sa wala kung naay luwag (pananglitan "1.2 (m)" sa gamay nga cell)
                list.Add(new Slot { Seg = s, F = f, Al = 'l', X = s.X0, W = s.Xr - s.X0 - 3,
                    WMax = Math.Max(s.Xr - s.X0 - 3, s.Xr - s.Xl - 4), Lo = s.Xl + 2, Hi = s.Xr - 1.5 });
            }
        }
        return list;
    }

    // Isulat ang text sa mga linya. Ang matag paragraph magsugod sa bag-ong linya.
    // Kung dili mohaum, gamay-gamayon ang letra (hangtod 62%).
    private static void Flow(List<StringBuilder> pages, List<Slot> slots, List<string> paras, double? fixedScale = null)
    {
        if (paras.Count == 0 || slots.Count == 0)
            return;

        if (slots.Count == 1)
        {
            string text = string.Join(" ", paras);
            var sl = slots[0];
            double w = TextWidth(text, sl.F), scale = 1;

            // PETSA sa gamay nga linya (pananglitan 069 Item 1 "Dated"): kung ang "November 30, 2025"
            // mogamay kaayo (ubos sa 85%), gamiton ang "11/30/2025" aron klaro basahon.
            // PAREHO KATAAS ANG LINYA: kung mas taas ang petsa kaysa sa linya "____" (pananglitan "November 6, 2026"),
            // "11/06/2026" aron dili mo-lapas ug dili motaas ang linya (uniform sa uban).
            bool tooLong = sl.Line ? w > sl.W : w > sl.WMax && sl.WMax / w < 0.85;
            if (tooLong
                && DateTime.TryParseExact(text, "MMMM d, yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                text = date.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture);
                w = TextWidth(text, sl.F);
            }
            if (w > sl.W && w > sl.WMax)
                scale = Math.Max(0.6, sl.WMax / w);
            // Kung taas pa gihapon bisan 60% na: gamayon pa aron DILI molapas sa border (walay mawala).
            if (w * scale > sl.WMax)
                scale = sl.WMax / w;
            DrawLine(pages, sl, text, scale);
            return;
        }

        double used = fixedScale ?? FitScale(slots, paras);
        var (lines, _, wrap) = Layout(slots, paras, used);

        for (int k = 0; k < slots.Count; k++)
        {
            if (lines[k].Length == 0)
                continue;
            // Dili gyud molapas sa border: kung taas pa ang linya, gamayon kini ra.
            double sc = used, lw = TextWidth(lines[k], slots[k].F * used);
            if (lw > slots[k].WMax)
                sc = used * slots[k].WMax / lw;

            // JUSTIFY: ang linya nga napuno (dili ang katapusan sa paragraph) i-inat hangtod sa tuo nga kilid,
            // pinaagi sa dugang nga luwag sa matag space (Tw). Dili kung gamay ra ang sulod (aron dili layo kaayo).
            double ws = 0;
            int spaces = lines[k].Count(ch => ch == ' ');
            if (wrap[k] && spaces > 0 && slots[k].Al == 'l' && sc == used)
            {
                double extra = (slots[k].W - lw) / spaces;
                if (extra > 0 && extra < slots[k].F * used * 1.2)
                    ws = extra;
            }
            DrawLine(pages, slots[k], lines[k], sc, ws);
        }
    }

    // Pinaka-dako nga gidak-on (scale) nga mohaum ang tanang sulat sa mga linya.
    private static double FitScale(List<Slot> slots, List<string> paras)
    {
        double[] scales = { 1.0, 0.92, 0.85, 0.78, 0.7, 0.62, 0.55, 0.5, 0.45, 0.4 };
        foreach (double scale in scales)
        {
            if (Layout(slots, paras, scale).Ok)
                return scale;
        }
        return scales[^1];
    }

    // Ibutang ang mga pulong sa mga linya sa usa ka gidak-on. Ok = false kung kulang ang linya.
    // Wrap[k] = true kung ang linya k napuno ug ang paragraph mipadayon sa sunod nga linya (para sa JUSTIFY).
    private static (string[] Lines, bool Ok, bool[] Wrap) Layout(List<Slot> slots, List<string> paras, double scale)
    {
        var lines = new string[slots.Count];
        var wrap = new bool[slots.Count];
        for (int j = 0; j < lines.Length; j++)
            lines[j] = "";
        int k = 0;
        bool ok = true;
        foreach (var para in paras)
        {
            var words = para.Split(' ').ToList();
            int i = 0;
            while (i < words.Count && k < slots.Count)
            {
                string next = lines[k].Length > 0 ? lines[k] + " " + words[i] : words[i];
                if (TextWidth(next, slots[k].F * scale) <= slots[k].W)
                {
                    lines[k] = next;
                    i++;
                }
                else if (lines[k].Length == 0)
                {
                    // TAAS NGA PULONG (pananglitan "99999999..." walay space): bahinon sa linya
                    int cut = FitChars(words[i], slots[k].F * scale, slots[k].W);
                    lines[k] = words[i][..cut];
                    words[i] = words[i][cut..];
                    k++;
                }
                else
                {
                    wrap[k] = true;   // puno na: ang sunod nga pulong sa sunod nga linya
                    k++;
                }
            }
            if (i < words.Count)
            {
                // kulang ang linya: ibutang ang nahabilin sa katapusan nga linya (gamayon sa ubos)
                ok = false;
                lines[^1] = (lines[^1] + " " + string.Join(" ", words.Skip(i))).Trim();
            }
            k++;
        }
        return (lines, ok, wrap);
    }

    // Pila ka letra sa taas nga pulong ang mohaum sa gilapdon (labing menos 1).
    private static int FitChars(string word, double size, double width)
    {
        int n = 1;
        while (n < word.Length && TextWidth(word[..(n + 1)], size) <= width)
            n++;
        return n;
    }

    // Taas nga tubag (defects#1, defects#2 ...). Kung ang mga linya sakop sa ITEM I, ITEM II ...
    // ug ang paragraph nagsugod og "IV." o "ITEM IV:", didto kini ibutang sa ITEM IV.
    // Kung walay "IV." bisan asa, sunod-sunod lang ang tanang linya.
    // Dili pa isulat dinhi: ibalik ang mga linya ug sulat, aron ang Fill makapili sa gidak-on sa letra.
    private static List<(List<Slot> Slots, List<string> Paras)> FlowLong(List<PrintMarker> parts, string text)
    {
        var paras = Paragraphs(text);
        var groups = parts.Where(m => !string.IsNullOrEmpty(m.G)).Select(m => m.G!).Distinct().ToList();
        var tags = paras.Select(x => Roman.Match(x)).ToList();

        if (groups.Count > 0 && tags.Any(x => x.Success && groups.Contains(x.Groups[1].Value.ToUpperInvariant())))
        {
            var buckets = groups.ToDictionary(x => x, _ => new List<string>());
            int cur = -1;
            for (int i = 0; i < paras.Count; i++)
            {
                string p = paras[i];
                var t = tags[i];
                if (t.Success && groups.Contains(t.Groups[1].Value.ToUpperInvariant()))
                {
                    cur = groups.IndexOf(t.Groups[1].Value.ToUpperInvariant());
                    p = p[t.Length..];
                }
                else
                    cur = Math.Max(cur, 0);   // walay "IV." sa sugod: padayon sa samang ITEM (bag-ong linya)
                buckets[groups[cur]].Add(p);
            }
            return groups.Select(g => (parts.Where(m => m.G == g).SelectMany(m => Slots(m, false)).ToList(), buckets[g])).ToList();
        }

        return new() { (parts.SelectMany(m => Slots(m, false)).ToList(), paras) };
    }

    // wordSpace > 0 = JUSTIFY (dugang nga luwag sa matag space; i-reset dayon sa 0 aron dili madala sa uban).
    private static void DrawLine(List<StringBuilder> pages, Slot sl, string text, double scale, double wordSpace = 0)
    {
        var s = sl.Seg;
        if (s.P < 0 || s.P >= pages.Count)
            return;
        double f = sl.F * scale;
        double w = TextWidth(text, f) + wordSpace * text.Count(ch => ch == ' ');
        double x;
        if (sl.Al == 'c')
            x = sl.X - w / 2;
        else if (sl.Al == 'r')
            x = sl.X - w;
        else
        {
            x = sl.X;
            if (!double.IsNaN(sl.Hi) && x + w > sl.Hi)
                x = Math.Max(sl.Lo, sl.Hi - w);
        }

        var pg = pages[s.P];
        pg.Append("BT /FsiHelv ").Append(N(f)).Append(" Tf ");
        if (wordSpace > 0)
            pg.Append(N(wordSpace)).Append(" Tw ");
        pg.Append(N(x)).Append(' ').Append(N(s.Y)).Append(" Td ").Append(PdfString(text)).Append(" Tj ");
        if (wordSpace > 0)
            pg.Append("0 Tw ");
        pg.Append("ET\n");

        // Sa linya "____": kung mas taas ang tubag kaysa linya, padayonon ang linya ilawom niini.
        if (sl.Line && x + w > s.X1)
        {
            double y = s.Y - 0.17 * s.S;
            pg.Append(N(0.05 * s.S)).Append(" w ").Append(N(s.X1 - 0.5)).Append(' ').Append(N(y)).Append(" m ")
              .Append(N(x + w + 1)).Append(' ').Append(N(y)).Append(" l S\n");
        }
    }

    // ------------------------------------------------------------ check and signature

    // Check mark nga gi-drawing og linya (dili font), aron pareho ang hitsura sa bisan unsang PDF viewer.
    private static void DrawCheck(List<StringBuilder> pages, PrintMarker m)
    {
        var s = m.Segs[0];
        if (s.P < 0 || s.P >= pages.Count)
            return;
        double cx, cy, size;
        if (m.T == "s")
        {
            // [   ]: sa tunga sa duha ka bracket
            cx = (s.X0 + s.X1) / 2;
            cy = s.Y + 0.26 * s.S;
            size = Math.Max(0.5 * s.S, Math.Min(0.6 * s.S, s.X1 - s.X0 + 0.05 * s.S));
        }
        else if (s.Box is { Length: 4 } b)
        {
            // gamay nga kahon o cell sa table: sa tunga sa kahon
            cx = (b[0] + b[2]) / 2;
            cy = (b[1] + b[3]) / 2;
            size = Math.Min(0.75 * Math.Min(b[2] - b[0], b[3] - b[1]), 0.9 * s.S);
        }
        else
        {
            cx = (s.X0 + s.X1) / 2;
            cy = s.Y + 0.3 * s.S;
            size = 0.6 * s.S;
        }

        double[,] pts = { { -0.45, 0.02 }, { -0.12, -0.40 }, { 0.48, 0.47 } };
        var pg = pages[s.P];
        pg.Append("q ").Append(N(Math.Max(0.6, 0.15 * size))).Append(" w 1 J 1 j ");
        for (int i = 0; i < 3; i++)
        {
            pg.Append(N(cx + pts[i, 0] * size)).Append(' ').Append(N(cy + pts[i, 1] * size - 0.03 * size))
              .Append(i == 0 ? " m " : " l ");
        }
        pg.Append("S Q\n");
    }

    // Pirma ibabaw sa printed name (mga linya, dili hulagway). Mogamay kung gamay ang luwag ibabaw sa ngalan.
    private static void DrawSignature(List<StringBuilder> pages, PrintSpot seg, SignatureJson.Dto sig)
    {
        if (seg.P < 0 || seg.P >= pages.Count)
            return;
        double aspect = Math.Max(sig.Aspect, 0.01);
        double cx = (seg.X0 + seg.X1) / 2;
        double bottom = seg.Y + 0.1 * seg.S;
        double bh = Math.Max(12, Math.Min(SigH, (seg.Up ?? 60) - 0.1 * seg.S - 1.5));
        double bw = SigW * bh / SigH;
        double sc = Math.Min(bw, bh / aspect);
        double ox = cx - sc / 2;
        double oy = bottom + (bh - sc * aspect) / 2;

        var pg = pages[seg.P];
        pg.Append("q 0.04 0.08 0.24 RG 0.9 w 1 J 1 j\n");
        foreach (var stroke in sig.Strokes)
        {
            var pts = stroke.Where(x => x.Length >= 2).ToList();
            if (pts.Count == 0)
                continue;
            for (int i = 0; i < pts.Count; i++)
            {
                pg.Append(N(ox + pts[i][0] * sc)).Append(' ').Append(N(oy + (aspect - pts[i][1]) * sc))
                  .Append(i == 0 ? " m\n" : " l\n");
            }
            if (pts.Count == 1)   // tuldok
                pg.Append(N(ox + pts[0][0] * sc + 0.1)).Append(' ').Append(N(oy + (aspect - pts[0][1]) * sc)).Append(" l\n");
            pg.Append("S\n");
        }
        pg.Append("Q\n");
    }

    // ------------------------------------------------------------ the file

    // Idugang sa katapusan sa PDF ang bag-ong overlay sa matag page (incremental update).
    // LISTAHAN: usa ka tubag matag linya (dili moagas sa sunod nga linya). Pareho ang gidak-on sa letra,
    // gamayon ra kung dili mohaum ang pinaka-taas.
    private static void OneEach(List<StringBuilder> pages, List<Slot> slots, List<string> items)
    {
        if (items.Count == 0)
            return;
        double scale = 1;
        for (int k = 0; k < items.Count; k++)
            scale = Math.Min(scale, slots[k].WMax / Math.Max(TextWidth(items[k], slots[k].F), 1));
        for (int k = 0; k < items.Count; k++)
            DrawLine(pages, slots[k], items[k], scale);
    }

    // PADAYON (tan-awa PrintCont). Mobalik og false kung mohaum ra (normal nga pagsulat).
    private static bool Continue(List<StringBuilder> pages, PrintMarker m, PrintCont ct, string text)
    {
        var sl = Slots(m, false)[0];
        if (TextWidth(text, sl.F) * 0.85 <= sl.W || ct.P < 0 || ct.P >= pages.Count)
            return false;

        // sa linya: ang pilian ra (pananglitan "Special Structure") + " *"; ang tibuok tubag naa sa "*" sa laing lugar
        int paren = text.IndexOf(" (", StringComparison.Ordinal);
        string head = (paren > 0 ? text[..paren] : string.Join(" ", text.Split(' ').Take(2))) + " *";
        double hw = TextWidth(head, sl.F);
        DrawLine(pages, sl, head, hw > sl.WMax ? sl.WMax / hw : 1);

        // sa laing lugar (pananglitan sa blangko nga cell sa table sa page 2, ibabaw sa "4. Means of Egress"):
        //   linya 1 = "* " + ulohan (bold), linya 2 = ang tibuok tubag. Pareho nga 10 pt; gamayon ra kung dili mohaum.
        double f = sl.F;
        double tw = TextWidth(text, f), lw = TextWidth("* " + ct.Label, f) * 1.08;
        double scale = Math.Min(1, ct.W / Math.Max(Math.Max(tw, lw), 1));
        f *= scale;
        if (ct.X2 > 0)
        {
            // sa bag-ong row: ulohan sa column 1, tubag sa column 2 (parehas nga linya)
            double w1 = ct.X2 - ct.X - 6, w2 = (ct.Cols.Count > 2 ? ct.Cols[2] : ct.X2 + ct.W) - ct.X2 - 6;
            double f1 = Math.Min(sl.F, sl.F * w1 / Math.Max(TextWidth("* " + ct.Label, sl.F) * 1.08, 1));
            double f2 = Math.Min(sl.F, sl.F * w2 / Math.Max(TextWidth(text, sl.F), 1));
            pages[ct.P].Append("BT /FsiHelvB ").Append(N(f1)).Append(" Tf ").Append(N(ct.X)).Append(' ').Append(N(ct.Y))
                .Append(" Td ").Append(PdfString("* " + ct.Label)).Append(" Tj ET\n")
                .Append("BT /FsiHelv ").Append(N(f2)).Append(" Tf ").Append(N(ct.X2)).Append(' ').Append(N(ct.Y))
                .Append(" Td ").Append(PdfString(text)).Append(" Tj ET\n");
            return true;
        }
        pages[ct.P].Append("BT /FsiHelvB ").Append(N(f)).Append(" Tf ").Append(N(ct.X)).Append(' ').Append(N(ct.Y))
            .Append(" Td ").Append(PdfString("* " + ct.Label)).Append(" Tj ET\n")
            .Append("BT /FsiHelv ").Append(N(f)).Append(" Tf ").Append(N(ct.X)).Append(' ').Append(N(ct.Y - 12))
            .Append(" Td ").Append(PdfString(text)).Append(" Tj ET\n");
        return true;
    }

    // "Page 5 of 5" (ang numero naka-bold, sama sa form).
    private static void PageNumber(StringBuilder sb, double x, double y, double size, int n, int total)
    {
        string f = N(size);
        sb.Append("BT /FsiHelv ").Append(f).Append(" Tf ").Append(N(x)).Append(' ').Append(N(y)).Append(" Td (Page ) Tj ")
          .Append("/FsiHelvB ").Append(f).Append(" Tf (").Append(n).Append(") Tj ")
          .Append("/FsiHelv ").Append(f).Append(" Tf ( of ) Tj ")
          .Append("/FsiHelvB ").Append(f).Append(" Tf (").Append(total).Append(") Tj ET\n");
    }

    // LITRATO: 4 matag page, usa matag row. Ang matag litrato EKSAKTO 7.12" x 2.49" (512.64 x 179.28 pt),
    // sa tunga sa page (sama sa Word: absolute width 7.12", height 2.49").
    private static List<ExtraPg> PhotoPages(double[] size, string title, List<byte[]> photos)
    {
        const double PhotoW = 7.12 * 72, PhotoH = 2.49 * 72, PerPage = 4;
        var list = new List<ExtraPg>();
        var ok = photos.Where(p => JpegInfo(p).W > 0).ToList();
        double w = size[0], h = size[1], left = (w - PhotoW) / 2, top = h - 72, bottom = 60;
        double gap = (top - bottom - PerPage * PhotoH) / (PerPage - 1);
        for (int k = 0; k < ok.Count; k++)
        {
            int slot = (int)(k % PerPage);
            if (slot == 0)
            {
                var pg = new ExtraPg();
                pg.Content.Append("BT /FsiHelvB 11 Tf ").Append(N(left)).Append(' ').Append(N(h - 55)).Append(" Td ")
                    .Append(PdfString(title)).Append(" Tj ET\n");
                list.Add(pg);
            }
            var page = list[^1];
            int idx = page.Images.Count;
            page.Images.Add(ok[k]);
            double y = top - slot * (PhotoH + gap) - PhotoH;
            page.Content.Append("q ").Append(N(PhotoW)).Append(" 0 0 ").Append(N(PhotoH)).Append(' ').Append(N(left)).Append(' ')
                .Append(N(y)).Append(" cm /Im").Append(idx).Append(" Do Q\n");
        }
        return list;
    }

    // JPEG: gilapdon, gitas-on ug ihap sa kolor (1 = gray, 3 = RGB, 4 = CMYK). W = 0 kung dili JPEG.
    private static (int W, int H, int C) JpegInfo(byte[] d)
    {
        if (d.Length < 4 || d[0] != 0xFF || d[1] != 0xD8)
            return (0, 0, 0);
        int i = 2;
        while (i + 9 < d.Length)
        {
            if (d[i] != 0xFF) { i++; continue; }
            byte m = d[i + 1];
            if (m == 0xD8 || m == 0x01 || (m >= 0xD0 && m <= 0xD7)) { i += 2; continue; }
            int len = (d[i + 2] << 8) | d[i + 3];
            if (m is >= 0xC0 and <= 0xCF && m != 0xC4 && m != 0xC8 && m != 0xCC)
                return ((d[i + 7] << 8) | d[i + 8], (d[i + 5] << 8) | d[i + 6], d[i + 9]);
            i += 2 + len;
        }
        return (0, 0, 0);
    }

    // DUGANG NGA PAGE: ulohan ug duha ka column nga mga linya (sama sa form), matag tubag usa ka linya.
    private static void ExtraPage(StringBuilder pg, double[] size, List<(string Title, List<string> Lines)> lists)
    {
        double w = size[0], top = size[1] - 60, left = 60, gap = 16, col = (w - 2 * left - gap) / 2;
        double y = top;
        var pages = new List<StringBuilder> { pg };
        foreach (var (title, lines) in lists)
        {
            pg.Append("BT /FsiHelvB 11 Tf ").Append(N(left)).Append(' ').Append(N(y)).Append(" Td ")
              .Append(PdfString(title)).Append(" Tj ET\n");
            y -= 26;
            for (int i = 0; i < lines.Count; i += 2)
            {
                for (int c = 0; c < 2 && i + c < lines.Count; c++)
                {
                    double x0 = left + c * (col + gap);
                    var seg = new PrintSpot { P = 0, X0 = x0, X1 = x0 + col, Y = y, S = 10, Xl = x0, Xr = x0 + col };
                    pg.Append("0.5 w ").Append(N(x0)).Append(' ').Append(N(y - 2)).Append(" m ")
                      .Append(N(x0 + col)).Append(' ').Append(N(y - 2)).Append(" l S\n");
                    var slot = new Slot { Seg = seg, F = 10, Al = 'l', X = x0 + 2, W = col - 3, WMax = col - 3 };
                    double sc = Math.Min(1, slot.WMax / Math.Max(TextWidth(lines[i + c], 10), 1));
                    DrawLine(pages, slot, lines[i + c], sc);
                }
                y -= 20;
            }
            y -= 18;
        }
    }

    private static byte[] Write(byte[] blank, PrintMap map, List<StringBuilder> pages, List<int> hide, List<ExtraPg> extras,
        List<(PrintList L, double D)> shifts)
    {
        var objs = new SortedDictionary<int, string>();
        for (int i = 0; i < pages.Count && i < map.Overlay.Count; i++)
        {
            if (pages[i].Length > 0)
                objs[map.Overlay[i]] = "q 0 g\n" + pages[i] + "Q\n";
        }
        foreach (int n in hide)
            objs[n] = "3 Tr\n";   // tagoan ang sulat sa form (pananglitan "(Station)")

        using var ms = new MemoryStream();
        ms.Write(blank);
        if (blank.Length > 0 && blank[^1] != (byte)'\n')
            ms.WriteByte((byte)'\n');

        var offsets = new Dictionary<int, long>();
        foreach (var (n, body) in objs)
        {
            offsets[n] = ms.Position;
            byte[] data = Encoding.Latin1.GetBytes(body);
            Ascii(ms, $"{n} 0 obj\n<< /Length {data.Length} >>\nstream\n");
            ms.Write(data);
            Ascii(ms, "\nendstream\nendobj\n");
        }

        int newSize = map.Size;

        // DUGANG NGA LINYA: ang page gi-drawing sa 3 ka bahin (clip): ibabaw (wala giusab),
        // tunga (ubos sa Cut, gipaubos og D) ug footer (wala giusab). Pareho ra nga mga content stream.
        foreach (var (l, d) in shifts)
        {
            if (l.Obj <= 0 || l.Contents.Count == 0 || l.Page >= map.Overlay.Count)
                continue;
            var size = l.Page < map.Pages.Count ? map.Pages[l.Page] : new double[] { 612, 936 };
            string w = N(size[0]);
            string[] bands =
            {
                $"q 0 {N(l.Cut)} {w} {N(size[1] - l.Cut)} re W n\n",
                $"Q\nq 0 {N(l.Floor)} {w} {N(l.Cut - d - l.Floor)} re W n 1 0 0 1 0 {N(-d)} cm\n",
                $"Q\nq 0 0 {w} {N(l.Floor)} re W n\n",
                "Q\n",
            };
            var ids = new List<int>();
            foreach (var b in bands)
            {
                int id = newSize++;
                byte[] data = Encoding.Latin1.GetBytes(b);
                offsets[id] = ms.Position;
                Ascii(ms, $"{id} 0 obj\n<< /Length {data.Length} >>\nstream\n");
                ms.Write(data);
                Ascii(ms, "\nendstream\nendobj\n");
                ids.Add(id);
            }
            string orig = string.Join(" ", l.Contents.Select(c => c + " 0 R"));
            string contents = $"{ids[0]} 0 R {orig} {ids[1]} 0 R {orig} {ids[2]} 0 R {orig} {ids[3]} 0 R {map.Overlay[l.Page]} 0 R";
            offsets[l.Obj] = ms.Position;
            Ascii(ms, $"{l.Obj} 0 obj\n{l.Dict.Replace("%CONTENTS%", contents)}\nendobj\n");
        }

        // DUGANG NGA PAGE(S): mga litrato (XObject), content, page; dayon ang /Pages nga naay bag-ong mga kid
        if (extras.Count > 0)
        {
            var sizeWh = map.Pages.Count > 0 ? map.Pages[^1] : new double[] { 612, 936 };
            string font(string name) => $"<< /Type /Font /Subtype /Type1 /BaseFont /{name} /Encoding /WinAnsiEncoding >>";
            var newKids = new List<int>();
            foreach (var ex in extras)
            {
                var xo = new StringBuilder();
                for (int k = 0; k < ex.Images.Count; k++)
                {
                    var img = ex.Images[k];
                    var (iw, ih, ic) = JpegInfo(img);
                    string cs = ic == 1 ? "/DeviceGray" : ic == 4 ? "/DeviceCMYK /Decode [1 0 1 0 1 0 1 0]" : "/DeviceRGB";
                    int id = newSize++;
                    offsets[id] = ms.Position;
                    Ascii(ms, $"{id} 0 obj\n<< /Type /XObject /Subtype /Image /Width {iw} /Height {ih} /ColorSpace {cs} " +
                        $"/BitsPerComponent 8 /Filter /DCTDecode /Length {img.Length} >>\nstream\n");
                    ms.Write(img);
                    Ascii(ms, "\nendstream\nendobj\n");
                    xo.Append($"/Im{k} {id} 0 R ");
                }

                int content = newSize++, page = newSize++;
                byte[] data = Encoding.Latin1.GetBytes("q 0 g 0 G\n" + ex.Content + "Q\n");
                offsets[content] = ms.Position;
                Ascii(ms, $"{content} 0 obj\n<< /Length {data.Length} >>\nstream\n");
                ms.Write(data);
                Ascii(ms, "\nendstream\nendobj\n");
                offsets[page] = ms.Position;
                Ascii(ms, $"{page} 0 obj\n<< /Type /Page /Parent {map.Tree} 0 R /MediaBox [0 0 {N(sizeWh[0])} {N(sizeWh[1])}] " +
                    $"/Resources << /Font << /FsiHelv {font("Helvetica")} /FsiHelvB {font("Helvetica-Bold")} >> " +
                    (xo.Length > 0 ? $"/XObject << {xo}>> " : "") + $">> /Contents {content} 0 R >>\nendobj\n");
                newKids.Add(page);
            }

            // isulod sa InsertAt (pananglitan 069: human sa page 3, una sa page sa pirma)
            var kids = map.Kids.ToList();
            int at = map.InsertAt >= 0 && map.InsertAt <= kids.Count ? map.InsertAt : kids.Count;
            kids.InsertRange(at, newKids);
            offsets[map.Tree] = ms.Position;
            Ascii(ms, $"{map.Tree} 0 obj\n<< /Type /Pages /Kids [{string.Join(" ", kids.Select(k => k + " 0 R"))}] /Count {kids.Count} >>\nendobj\n");
        }

        long xref = ms.Position;
        var sb = new StringBuilder("xref\n");
        foreach (int n in offsets.Keys.OrderBy(x => x))
            sb.Append(n).Append(" 1\n").Append(offsets[n].ToString("D10", CultureInfo.InvariantCulture)).Append(" 00000 n \n");
        sb.Append("trailer\n<< /Size ").Append(newSize).Append(" /Root ").Append(map.Root).Append(" 0 R /Prev ").Append(map.Prev);
        if (map.Info > 0)
            sb.Append(" /Info ").Append(map.Info).Append(" 0 R");
        if (map.Id.Count == 2)
            sb.Append(" /ID [<").Append(map.Id[0]).Append("> <").Append(map.Id[1]).Append(">]");
        sb.Append(" >>\nstartxref\n").Append(xref).Append("\n%%EOF\n");
        Ascii(ms, sb.ToString());
        return ms.ToArray();
    }

    private static void Ascii(Stream s, string text) => s.Write(Encoding.ASCII.GetBytes(text));

    // ------------------------------------------------------------ helpers

    private static string N(double v) => v.ToString("0.##", CultureInfo.InvariantCulture);

    private static List<string> Paragraphs(string text) =>
        text.Replace("\r", "").Split('\n')
            .Select(p => string.Join(" ", p.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)))
            .Where(p => p.Length > 0)
            .ToList();

    // Ang letra sa WinAnsi (ang encoding sa Helvetica sa PDF); "?" kung wala.
    private static byte Code(char ch)
    {
        if ((ch >= 32 && ch < 127) || (ch >= 160 && ch <= 255))
            return (byte)ch;
        return Win.TryGetValue(ch, out var b) ? b : (byte)'?';
    }

    private static double TextWidth(string text, double size)
    {
        double w = 0;
        foreach (char ch in text)
            w += Helv[Code(ch) - 32];
        return w * size / 1000;
    }

    // PDF string: (text) nga naay \ sa atubangan sa ( ) \
    private static string PdfString(string text)
    {
        var sb = new StringBuilder("(");
        foreach (char ch in text)
        {
            byte b = Code(ch);
            if (b == 0x28 || b == 0x29 || b == 0x5C)
                sb.Append('\\');
            sb.Append((char)b);
        }
        return sb.Append(')').ToString();
    }
}
