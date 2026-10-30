// =====================================================================
// SIGN-OFF page - ViewModel: ipakita ang progress, ang mga linya sa pirma, ug ang 'Complete inspection'.
// =====================================================================
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fsi_checklist.Controls;
using fsi_checklist.Services;
using fsi_checklist.Views;

namespace fsi_checklist.ViewModels;

/// <summary>One signature line on the sign-off page.</summary>
public sealed class SignatoryItem
{
    public required string Id { get; init; }
    public required string RoleLabel { get; init; }
    public required string StatusText { get; init; }
    public required Color StatusColor { get; init; }
    public required string DetailText { get; init; }
    public bool IsSigned { get; init; }
    public bool CanSign { get; init; }
    public string? Reason { get; init; }
    public SignaturePreviewDrawable? Preview { get; init; }

    public bool ShowSignButton => !IsSigned && CanSign;
    public bool HasReason => !IsSigned && !CanSign && !string.IsNullOrEmpty(Reason);
}

[QueryProperty(nameof(InspectionId), "inspectionId")]
[QueryProperty(nameof(FromForm), "fromForm")]
public sealed partial class SignoffPageViewModel : ObservableObject
{
    private static readonly Color Green = Color.FromArgb("#2E7D32");
    private static readonly Color Yellow = Color.FromArgb("#F9A825");

    private readonly InspectionService _inspections;

    public SignoffPageViewModel(InspectionService inspections)
    {
        _inspections = inspections;
    }

    [ObservableProperty] private string inspectionId = string.Empty;
    [ObservableProperty] private string fromForm = string.Empty;   // "1" when opened from the form page
    [ObservableProperty] private string title = "Sign-off";
    [ObservableProperty] private string subtitle = string.Empty;
    [ObservableProperty] private string stateText = string.Empty;
    [ObservableProperty] private string progressText = string.Empty;
    [ObservableProperty] private double progressFraction;
    [ObservableProperty] private Color progressColor = Yellow;
    [ObservableProperty] private string hint = string.Empty;
    [ObservableProperty] private bool canComplete;
    [ObservableProperty] private bool canEditForm;
    [ObservableProperty] private bool hasApproval;
    [ObservableProperty] private List<SignatoryItem> onSite = new();
    [ObservableProperty] private List<SignatoryItem> approval = new();

    partial void OnInspectionIdChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
            _ = LoadAsync();
    }

    // I-load ang sign-off: progress, status, hint, ug ang duha ka listahan sa pirma (sa inspection ug sa opisina).
    // GUARD: para dili mag-doble ang pag-load (pananglitan OnAppearing ug OnInspectionIdChanged
    // parehas mo-tawag). Ang ikaduha nga tawag laktawan ra.
    private bool _loading;

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (_loading)
            return;

        _loading = true;
        try { await LoadCoreAsync(); }
        finally { _loading = false; }
    }

    private async Task LoadCoreAsync()
    {
        if (string.IsNullOrEmpty(InspectionId))
            return;

        try
        {
            var data = await _inspections.LoadSignoffAsync(InspectionId);
            if (data == null)
            {
                await Dialogs.ShowAsync("Not found", "This inspection no longer exists.");
                await Nav.GoAsync("..");
                return;
            }

            bool isDraft = data.Status == "DRAFT";
            bool formDone = data.Progress.IsComplete;

            Title = data.OwnerName;
            Subtitle = data.OwnerName == data.SubjectName
                ? data.TemplateName
                : $"{data.TemplateName} · {data.SubjectName}";

            ProgressFraction = data.Progress.Fraction;
            ProgressColor = formDone ? Green : Yellow;
            ProgressText = formDone
                ? $"All {data.Progress.Total} items done"
                : $"{data.Progress.Done} of {data.Progress.Total} done · {data.Progress.Missing} missing";

            StateText = !isDraft
                ? (data.IsApproved ? "Completed" : "Awaiting approval")
                : formDone && data.MissingOnSite > 0 ? "Missing signature" : "Draft";

            CanEditForm = isDraft;
            // Ang 'Complete inspection' mugana ra kung DRAFT pa, humana ang form, ug kompleto ang pirma.
            CanComplete = isDraft && formDone && data.MissingOnSite == 0;

            Hint = !isDraft
                ? (data.IsApproved
                    ? "This inspection is complete."
                    : "Completed and locked. Waiting for the approval signatures from the office.")
                : !formDone
                    ? $"Finish the form first: {data.Progress.Missing} items are still missing."
                    : data.MissingOnSite > 0
                        ? $"{data.MissingOnSite} signature(s) are still needed at the inspection."
                        : "Everything is filled in and signed. Tap “Complete inspection” to lock it.";

            var items = data.Signatories.Select(s => BuildItem(s, data.Status, formDone)).ToList();
            OnSite = items.Where((_, i) => data.Signatories[i].Stage == "ON_SITE").ToList();
            Approval = items.Where((_, i) => data.Signatories[i].Stage == "APPROVAL").ToList();
            HasApproval = Approval.Count > 0;
        }
        catch (Exception ex)
        {
            AppLog.Error(ex);
            await Dialogs.ShowAsync("Couldn't open sign-off", AppLog.Describe(ex));
        }
    }

    // Himoon ang usa ka linya sa pirma para sa screen, ug tinoon kung pwede na ba mo-pirma.
    private static SignatoryItem BuildItem(SignatoryInfo s, string inspectionStatus, bool formDone)
    {
        bool signed = s.Status == "SIGNED";
        bool can = false;
        string? reason = null;

        if (!signed)
        {
            if (s.Stage == "ON_SITE")
            {
                if (inspectionStatus != "DRAFT") reason = "This inspection is already completed.";
                else if (!formDone) reason = "Finish the form first.";
                else can = true;
            }
            else
            {
                if (inspectionStatus != "COMPLETED") reason = "Available after the inspection is completed.";
                else can = true;
            }
        }

        string detail;
        if (signed)
        {
            var when = s.SignedAt is { } t
                ? DateTime.SpecifyKind(t, DateTimeKind.Utc).ToLocalTime().ToString("d MMM yyyy, h:mm tt")
                : string.Empty;
            string decision = s.Decision switch
            {
                "APPROVED" => " · Approved",
                "DISAPPROVED" => " · Disapproved",
                _ => string.Empty,
            };
            detail = $"{s.PrintedName} · {when}{decision}";
        }
        else
        {
            detail = s.IsRequired ? "Required" : "Optional";
        }

        return new SignatoryItem
        {
            Id = s.Id,
            RoleLabel = s.RoleLabel,
            IsSigned = signed,
            CanSign = can,
            Reason = reason,
            StatusText = signed ? "Signed" : "Pending",
            StatusColor = signed ? Color.FromArgb("#A8E6B0") : Color.FromArgb("#FFE08A"),
            DetailText = detail,
            Preview = signed ? SignatureJson.Parse(InspectionService.ReadSignatureFile(s.SignaturePath)) : null,
        };
    }

    // Pag-tap sa 'Sign': ablihan ang SignaturePage para niini nga linya.
    [RelayCommand]
    private async Task SignAsync(SignatoryItem? item)
    {
        if (item == null)
            return;

        if (!item.CanSign)
        {
            await Dialogs.ShowAsync("Can't sign yet", item.Reason ?? "Not available right now.");
            return;
        }

        await Nav.GoAsync($"{nameof(SignaturePage)}?signatoryId={Uri.EscapeDataString(item.Id)}");
    }

    // Pangutan-on una ang user, dayon i-lock ang inspection.
    [RelayCommand]
    private async Task CompleteAsync()
    {
        bool confirmed = await Dialogs.ConfirmAsync(
            "Complete this inspection?",
            "The answers will be locked and can no longer be changed.",
            "Complete");
        if (!confirmed)
            return;

        var (ok, message) = await _inspections.CompleteInspectionAsync(InspectionId);
        await Dialogs.ShowAsync(ok ? "Done" : "Can't complete yet", message);
        await LoadAsync();
    }

    // VIEW RESULTS: ablihan ang tibuok gi-fill-up nga checklist (basa ra).
    [RelayCommand]
    private Task ViewResultsAsync() =>
        Nav.GoAsync($"{nameof(ResultsPage)}?inspectionId={Uri.EscapeDataString(InspectionId)}");

    [RelayCommand]
    private async Task EditFormAsync()
    {
        if (FromForm == "1")
            await Nav.GoAsync("..");
        else
            await Nav.GoAsync($"{nameof(FormPage)}?inspectionId={Uri.EscapeDataString(InspectionId)}");
    }

    // Mobalik sa Dashboard gikan bisan asa (tan-awa Nav.BackToDashboardAsync).
    [RelayCommand]
    private Task BackToDashboardAsync() => Nav.BackToDashboardAsync();
}
