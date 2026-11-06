// =====================================================================
// VIEW RESULTS - ViewModel: ipakita ang tibuok gi-fill-up nga checklist (BASA RA, dili ma-edit).
//   - ulohan: owner, form, code, petsa, reference, status
//   - matag seksyon ug pangutana, uban ang tubag ("—" kung wala'y tubag)
//   - ang mga pirma sa katapusan (uban ang hulagway sa pirma)
// =====================================================================
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fsi_checklist.Controls;
using fsi_checklist.Data.Models;
using fsi_checklist.Services;

namespace fsi_checklist.ViewModels;

/// <summary>Usa ka row sa results: ulohan sa seksyon, o usa ka pangutana uban ang tubag.</summary>
public sealed class ResultRow
{
    public bool IsSection { get; init; }
    public bool IsItem => !IsSection;
    public string Title { get; init; } = string.Empty;
    public string Answer { get; init; } = string.Empty;
    public bool IsUnanswered { get; init; }
    public Thickness Margin { get; init; }
    public SignaturePreviewDrawable? Signature { get; init; }
    public bool HasSignature => Signature != null;

    // Mga litrato sa niini nga pangutana (full path sa file).
    public List<string> PhotoPaths { get; init; } = new();
    public bool HasPhotos => PhotoPaths.Count > 0;
}

/// <summary>Usa ka seksyon sa results = usa ka ExpanderView (header = titulo + pila ang kulang, sulod = mga tubag).</summary>
public sealed partial class ResultSection : ObservableObject
{
    public string Title { get; init; } = string.Empty;
    public List<ResultRow> Items { get; } = new();

    // Pila ang walay tubag (yellow) sulod niini nga seksyon.
    public int Missing => Items.Count(r => r.IsUnanswered);
    public bool HasMissing => Missing > 0;
    public string MissingText => $"⚠ {Missing}";

    // Sirado sa sugod; i-tap ang header aron moabli.
    [ObservableProperty]
    private bool isExpanded;
}

[QueryProperty(nameof(InspectionId), "inspectionId")]
public sealed partial class ResultsPageViewModel : ObservableObject
{
    private readonly InspectionService _inspections;
    private readonly PrintService _print;
    private bool _loading;

    public ResultsPageViewModel(InspectionService inspections, PrintService print)
    {
        _inspections = inspections;
        _print = print;
    }

    // PRINT: pun-on ang tinuod nga BFP form ug ablihan sa Word / WPS para i-print.
    [RelayCommand]
    private async Task PrintAsync()
    {
        // DILI MA-PRINT kung draft pa o naay kulang (tan-awa ang CanPrint / PrintHint).
        if (!CanPrint)
        {
            await Dialogs.ShowAsync("Can't print yet", PrintHint);
            return;
        }

        IsBusy = true;
        try
        {
            string? error = await _print.PrintAsync(InspectionId);
            if (error != null)
                await Dialogs.ShowAsync("Can't print", error);
        }
        catch (Exception ex)
        {
            AppLog.Error(ex);
            await Dialogs.ShowAsync("Couldn't print", AppLog.Describe(ex));
        }
        finally
        {
            IsBusy = false;
        }
    }

    [ObservableProperty] private string inspectionId = string.Empty;
    [ObservableProperty] private string ownerName = string.Empty;
    [ObservableProperty] private string formName = string.Empty;
    [ObservableProperty] private string detailLine = string.Empty;
    [ObservableProperty] private string statusText = string.Empty;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private List<ResultSection> sections = new();

    // PRINT: pwede ra kung COMPLETED na ug walay kulang nga tubag.
    [ObservableProperty] private bool canPrint;
    [ObservableProperty] private string printHint = string.Empty;
    public bool HasPrintHint => !CanPrint;
    partial void OnCanPrintChanged(bool value) => OnPropertyChanged(nameof(HasPrintHint));

    partial void OnInspectionIdChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
            _ = LoadAsync();
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (_loading || string.IsNullOrEmpty(InspectionId))
            return;

        _loading = true;
        IsBusy = true;
        try
        {
            var form = await _inspections.LoadFormAsync(InspectionId);
            if (form == null)
            {
                await Dialogs.ShowAsync("Not found", "This inspection no longer exists.");
                await Nav.GoAsync("..");
                return;
            }

            var signoff = await _inspections.LoadSignoffAsync(InspectionId);

            // ----- ulohan -----
            var i = form.Inspection;
            OwnerName = string.IsNullOrWhiteSpace(i.Subject.OwnerName) ? i.Subject.Name : i.Subject.OwnerName!;
            FormName = $"{i.Template.DisplayName}  ({i.Template.Code} rev {i.Template.Revision})";

            var parts = new List<string>();
            if (!string.Equals(i.Subject.Name, OwnerName, StringComparison.OrdinalIgnoreCase))
                parts.Add(i.Subject.Name);
            if (!string.IsNullOrWhiteSpace(i.Subject.Address))
                parts.Add(i.Subject.Address!);
            if (i.DateInspected is { } d)
                parts.Add("Inspected " + d.ToString("d MMM yyyy"));
            if (!string.IsNullOrWhiteSpace(i.ReferenceNo))
                parts.Add("Ref. " + i.ReferenceNo);
            DetailLine = string.Join(" · ", parts);

            StatusText = i.Status != "COMPLETED"
                ? "Draft (not completed yet)"
                : signoff?.IsApproved == true ? "Completed" : "Awaiting approval";

            // ----- mga tubag + mga pirma: matag seksyon = usa ka ExpanderView -----
            var list = BuildAnswerRows(form);
            if (signoff != null)
                list.AddRange(BuildSignatureRows(signoff));
            Sections = GroupBySection(list);

            // ----- PRINT: COMPLETED ug walay kulang (pareho sa ihap sa progress bar sa Form) -----
            int missing = CountMissing(form);
            bool completed = i.Status == "COMPLETED";
            CanPrint = completed && missing == 0;
            PrintHint = !completed && missing > 0
                ? $"Draft pa ug naay {missing} ka kulang nga tubag. Human-a ug i-sign-off una sa dili pa i-print."
                : !completed
                    ? "Draft pa (wala pa ma-sign-off). I-sign-off una sa dili pa i-print."
                    : missing > 0 ? $"Naay {missing} ka kulang nga tubag. Human-a una sa dili pa i-print." : string.Empty;
        }
        catch (Exception ex)
        {
            AppLog.Error(ex);
            await Dialogs.ShowAsync("Couldn't open results", AppLog.Describe(ex));
        }
        finally
        {
            IsBusy = false;
            _loading = false;
        }
    }

    // Himoon ang mga row sa parehas nga han-ay sa form: seksyon, dayon ang mga pangutana, dayon ang mga anak nga pangutana.
    private static List<ResultRow> BuildAnswerRows(FormData data)
    {
        var answers = data.Answers
            .Where(a => a.RepeatNo == 0 && (a.ValueText != null || a.ValueNum != null))
            .GroupBy(a => (a.ItemId, a.SlotKey))
            .ToDictionary(g => g.Key, g => g.First());

        var choiceLabels = data.Choices.ToDictionary(c => (c.ItemId ?? c.ColumnId ?? string.Empty, c.Value), c => c.Label);
        var columnsBySet = data.Columns.GroupBy(c => c.ColumnSetId)
            .ToDictionary(g => g.Key, g => g.OrderBy(c => c.SortOrder).ToList());
        var byId = data.Items.ToDictionary(x => x.Id);

        // Nakatago ba ang pangutana (show_if)? Kung nakatago sa form, dili usab ipakita diri.
        bool Shown(string? showIfItemId, string? showIfValues)
        {
            if (string.IsNullOrEmpty(showIfItemId))
                return true;
            var (slot, values) = ShowIfRule.Parse(showIfValues);
            answers.TryGetValue((showIfItemId, slot), out var a);
            return ShowIfRule.Matches(a?.ValueText, values);
        }

        var rows = new List<ResultRow>();

        void AddItem(FormItem item, int depth)
        {
            if (!Shown(item.ShowIfItemId, item.ShowIfValues))
                return;

            var lines = new List<string>();
            bool anyMissing = false;

            // N/A: kung "N/A" ang gipili sa Passed / Failed / N/A, dili na ipakita ang numero (dili kinahanglan).
            bool naNumber = item.DataType is "NUMBER" or "INTEGER"
                && answers.TryGetValue((item.Id, "result"), out var res) && res.ValueText == "NA";

            // Ang main nga tubag sa pangutana.
            if (item.DataType != "NONE" && item.DataType != "PHOTO" && !naNumber)
            {
                answers.TryGetValue((item.Id, "value"), out var a);
                string text = Format(item.DataType, a, item.Id, item.Unit, choiceLabels);
                anyMissing |= a == null && item.DataType != "BOOL";
                lines.Add(text);
            }
            else if (item.DataType == "PHOTO")
            {
                int count = data.Photos.Count(p => p.ItemId == item.Id);
                lines.Add(count == 0 ? "No photos" : $"{count} photo(s)");
                anyMissing |= count == 0;
            }

            // Ang mga column (Yes/No, Installed?, Compliant, Remarks...).
            if (item.ColumnSetId != null && columnsBySet.TryGetValue(item.ColumnSetId, out var columns))
            {
                bool label = columns.Count > 1 || item.DataType != "NONE";
                answers.TryGetValue((item.Id, "result"), out var resultAnswer);
                foreach (var col in columns)
                {
                    answers.TryGetValue((item.Id, col.Key), out var a);
                    string text = Format(col.DataType, a, col.Id, null, choiceLabels);
                    // REMARKS kinahanglan kung FAILED
                    bool remarksNeeded = ProgressCalculator.RemarksRequired(col.Key, resultAnswer?.ValueText);
                    if (a == null && (col.DataType != "TEXT" || remarksNeeded))
                        anyMissing = true;
                    // Ayaw ipakita ang blangko nga remarks (gawas kung FAILED, kay kinahanglan kini).
                    if (a == null && col.DataType == "TEXT" && !remarksNeeded)
                        continue;
                    lines.Add(label ? $"{col.Label}: {text}" : text);
                }
            }

            rows.Add(new ResultRow
            {
                Title = item.IsRequired ? item.Label + " *" : item.Label,
                Answer = string.Join("\n", lines),
                IsUnanswered = anyMissing,
                Margin = new Thickness(depth * 18, 0, 0, 0),
                PhotoPaths = data.Photos
                    .Where(p => p.ItemId == item.Id)
                    .Select(p => InspectionService.PhotoFullPath(p.FilePath))
                    .ToList(),
            });

            foreach (var child in data.Items.Where(c => c.ParentItemId == item.Id).OrderBy(c => c.SortOrder))
                AddItem(child, depth + 1);
        }

        void AddSection(FormSection section)
        {
            if (!Shown(section.ShowIfItemId, section.ShowIfValues))
                return;

            rows.Add(new ResultRow { IsSection = true, Title = section.Title });

            foreach (var item in data.Items.Where(x => x.SectionId == section.Id && x.ParentItemId == null).OrderBy(x => x.SortOrder))
                AddItem(item, 0);

            foreach (var child in data.Sections.Where(s => s.ParentId == section.Id).OrderBy(s => s.SortOrder))
                AddSection(child);
        }

        foreach (var root in data.Sections.Where(s => s.ParentId == null).OrderBy(s => s.SortOrder))
            AddSection(root);

        return rows;
    }

    // Ibahin ang mga row sa matag seksyon (ang ulohan sa seksyon = header sa ExpanderView).
    private static List<ResultSection> GroupBySection(List<ResultRow> rows)
    {
        var sections = new List<ResultSection>();
        ResultSection? current = null;
        foreach (var r in rows)
        {
            if (r.IsSection)
            {
                current = new ResultSection { Title = r.Title };
                sections.Add(current);
                continue;
            }
            if (current == null)
            {
                current = new ResultSection { Title = "Answers" };
                sections.Add(current);
            }
            current.Items.Add(r);
        }
        return sections.Where(s => s.Items.Count > 0).ToList();
    }

    // Pila ang kulang: pareho sa progress bar sa Form (ang nakatago ug optional dili iapil).
    private static int CountMissing(FormData data)
    {
        var answers = data.Answers
            .Where(a => a.RepeatNo == 0 && (a.ValueText != null || a.ValueNum != null))
            .GroupBy(a => (a.ItemId, a.SlotKey))
            .ToDictionary(g => g.Key, g => g.First().ValueText ?? string.Empty);
        var columnsBySet = data.Columns.GroupBy(c => c.ColumnSetId)
            .ToDictionary(g => g.Key, g => g.OrderBy(c => c.SortOrder).ToList());
        return ProgressCalculator.Compute(data.Items, columnsBySet, answers).Missing;
    }

    // Himoon nga mabasa sa tawo ang tubag: YES -> Yes, PASS -> Passed, CHOICE value -> iyang label, ug uban pa.
    private static string Format(
        string dataType, InspectionAnswer? a, string ownerId, string? unit,
        IReadOnlyDictionary<(string, string), string> choiceLabels)
    {
        if (dataType == "BOOL")
            return a != null && (a.ValueNum == 1 || a.ValueText == "1") ? "☑ Yes" : "☐ No";

        if (a == null)
            return "—";

        switch (dataType)
        {
            case "YES_NO":
            case "YES_NO_NA":
                return a.ValueText switch { "YES" => "Yes", "NO" => "No", "NA" => "N/A", _ => a.ValueText ?? "—" };

            case "PASS_FAIL":
                return a.ValueText switch { "PASS" => "Passed", "FAIL" => "Failed", _ => a.ValueText ?? "—" };

            case "CHOICE":
                return a.ValueText != null && choiceLabels.TryGetValue((ownerId, a.ValueText), out var label)
                    ? label
                    : a.ValueText ?? "—";

            case "NUMBER":
            case "INTEGER":
                string n = a.ValueNum?.ToString("G", CultureInfo.CurrentCulture) ?? a.ValueText ?? "—";
                return string.IsNullOrEmpty(unit) ? n : $"{n} {unit}";

            case "DATE":
                return DateTime.TryParse(a.ValueText, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d)
                    ? d.ToString("d MMM yyyy")
                    : a.ValueText ?? "—";

            default:
                return a.ValueText ?? "—";
        }
    }

    // Ang mga pirma sa katapusan: role, ngalan, oras, ug ang hulagway sa pirma.
    private static IEnumerable<ResultRow> BuildSignatureRows(SignoffData signoff)
    {
        yield return new ResultRow { IsSection = true, Title = "Signatures" };

        foreach (var s in signoff.Signatories)
        {
            string answer;
            if (s.Status == "SIGNED")
            {
                string when = s.SignedAt is { } t
                    ? DateTime.SpecifyKind(t, DateTimeKind.Utc).ToLocalTime().ToString("d MMM yyyy, h:mm tt")
                    : string.Empty;
                string decision = s.Decision switch
                {
                    "APPROVED" => " · Approved",
                    "DISAPPROVED" => " · Disapproved",
                    _ => string.Empty,
                };
                answer = $"{s.PrintedName} · {when}{decision}";
            }
            else
            {
                answer = s.IsRequired ? "Not signed yet (required)" : "Not signed yet";
            }

            yield return new ResultRow
            {
                Title = s.RoleLabel,
                Answer = answer,
                IsUnanswered = s.Status != "SIGNED",
                Signature = s.Status == "SIGNED"
                    ? SignatureJson.Parse(InspectionService.ReadSignatureFile(s.SignaturePath))
                    : null,
            };
        }
    }
}
