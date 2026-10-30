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

[QueryProperty(nameof(InspectionId), "inspectionId")]
public sealed partial class ResultsPageViewModel : ObservableObject
{
    private readonly InspectionService _inspections;
    private bool _loading;

    public ResultsPageViewModel(InspectionService inspections)
    {
        _inspections = inspections;
    }

    [ObservableProperty] private string inspectionId = string.Empty;
    [ObservableProperty] private string ownerName = string.Empty;
    [ObservableProperty] private string formName = string.Empty;
    [ObservableProperty] private string detailLine = string.Empty;
    [ObservableProperty] private string statusText = string.Empty;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private List<ResultRow> rows = new();

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

            // ----- mga tubag + mga pirma -----
            var list = BuildAnswerRows(form);
            if (signoff != null)
                list.AddRange(BuildSignatureRows(signoff));
            Rows = list;
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
            if (!answers.TryGetValue((showIfItemId, "value"), out var a) || a.ValueText == null)
                return false;
            return (showIfValues ?? string.Empty)
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .Contains(a.ValueText, StringComparer.OrdinalIgnoreCase);
        }

        var rows = new List<ResultRow>();

        void AddItem(FormItem item, int depth)
        {
            if (!Shown(item.ShowIfItemId, item.ShowIfValues))
                return;

            var lines = new List<string>();
            bool anyMissing = false;

            // Ang main nga tubag sa pangutana.
            if (item.DataType != "NONE" && item.DataType != "PHOTO")
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
                foreach (var col in columns)
                {
                    answers.TryGetValue((item.Id, col.Key), out var a);
                    string text = Format(col.DataType, a, col.Id, null, choiceLabels);
                    if (a == null && col.DataType != "TEXT")
                        anyMissing = true;
                    // Ayaw ipakita ang blangko nga remarks.
                    if (a == null && col.DataType == "TEXT")
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
