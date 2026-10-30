// =====================================================================
// E-SIGNATURE page - ViewModel: printed name, Approved/Disapproved (para sa Fire Marshal ra), ug pag-save sa pirma.
// Duha ka paagi sa pag-abli niini nga page:
//   ?signatoryId=...  = mo-pirma sa usa ka inspection (Owner, o Fire Marshal nga mo-decide)
//   ?presignRole=...  = gikan sa SETTINGS: i-save ang pre-signed nga pirma sa usa ka taga-BFP
// =====================================================================
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fsi_checklist.Controls;
using fsi_checklist.Services;

namespace fsi_checklist.ViewModels;

[QueryProperty(nameof(SignatoryId), "signatoryId")]
[QueryProperty(nameof(PresignRole), "presignRole")]
public sealed partial class SignaturePageViewModel : ObservableObject
{
    private readonly InspectionService _inspections;

    public SignaturePageViewModel(InspectionService inspections)
    {
        _inspections = inspections;
    }

    [ObservableProperty] private string signatoryId = string.Empty;
    [ObservableProperty] private string presignRole = string.Empty;   // naay sulod kung gikan sa Settings

    // Ang saved (pre-signed) nga pirma sa kini nga role, kung naa. Ipakita sa taas sa kahon.
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSaved))]
    private SignaturePreviewDrawable? savedPreview;
    private string? _savedJson;
    public bool HasSaved => SavedPreview != null;
    [ObservableProperty] private string roleLabel = "Signature";
    [ObservableProperty] private string printedName = string.Empty;
    [ObservableProperty] private bool needsDecision;
    [ObservableProperty] private string? decision;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string? errorText;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanSave))]
    private bool isBusy;

    public bool HasError => !string.IsNullOrEmpty(ErrorText);
    public bool CanSave => !IsBusy;

    partial void OnSignatoryIdChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
            _ = LoadAsync();
    }

    // Gikan sa Settings: ipakita ang daan nga pirma (kung naa) ug ang ngalan.
    partial void OnPresignRoleChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
            _ = LoadPresignAsync();
    }

    private async Task LoadPresignAsync()
    {
        try
        {
            var saved = await _inspections.GetPresignAsync(PresignRole);
            RoleLabel = $"{saved.RoleLabel} (saved signature)";
            PrintedName = saved.Name;
            NeedsDecision = false;
        }
        catch (Exception ex)
        {
            AppLog.Error(ex);
            await Dialogs.ShowAsync("Couldn't open signature", AppLog.Describe(ex));
        }
    }

    private async Task LoadAsync()
    {
        try
        {
            var info = await _inspections.GetSignatoryAsync(SignatoryId);
            if (info == null)
            {
                await Dialogs.ShowAsync("Not found", "This signature line no longer exists.");
                await Nav.GoAsync("..");
                return;
            }

            RoleLabel = info.RoleLabel;
            PrintedName = info.PrintedName;
            // Ang Fire Marshal ra ang mopili og Approved/Disapproved.
            NeedsDecision = info.RoleCode == "FIRE_MARSHAL";

            // Taga-BFP nga naay pre-signed nga pirma: ipakita ang "Use saved signature" (dili na mag-drawing).
            if (info.RoleCode != "OWNER_REP")
            {
                var saved = await _inspections.GetPresignAsync(info.RoleCode);
                if (saved.HasSignature)
                {
                    _savedJson = saved.StrokesJson;
                    SavedPreview = SignatureJson.Parse(_savedJson);
                    if (string.IsNullOrWhiteSpace(PrintedName))
                        PrintedName = saved.Name;
                }
            }
        }
        catch (Exception ex)
        {
            AppLog.Error(ex);
            await Dialogs.ShowAsync("Couldn't open signature", AppLog.Describe(ex));
        }
    }

    [RelayCommand]
    private void SetDecision(string? value) => Decision = value;

    // Susiha una ang printed name (ug decision), dayon i-save. Kung OK, mobalik sa sign-off page.
    /// <summary>Called by the page with the pen strokes (as JSON) when the signer taps Save.</summary>
    public async Task SaveAsync(string strokesJson)
    {
        ErrorText = null;

        if (string.IsNullOrWhiteSpace(PrintedName))
        {
            ErrorText = "Type the printed name of the signer.";
            return;
        }

        if (NeedsDecision && string.IsNullOrEmpty(Decision))
        {
            ErrorText = "Choose Approved or Disapproved.";
            return;
        }

        IsBusy = true;
        try
        {
            // Gikan sa Settings: i-save isip pre-signed nga pirma.
            if (!string.IsNullOrEmpty(PresignRole))
            {
                await _inspections.SavePresignAsync(PresignRole, PrintedName, strokesJson);
            }
            else
            {
                var (ok, message) = await _inspections.SaveSignatureAsync(SignatoryId, PrintedName, Decision, strokesJson);
                if (!ok)
                {
                    ErrorText = message;
                    return;
                }
            }

            await Nav.GoAsync("..");
        }
        catch (Exception ex)
        {
            AppLog.Error(ex);
            ErrorText = AppLog.Describe(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    // "Use saved signature": gamiton ang pre-signed nga pirma, walay drawing.
    [RelayCommand]
    private Task UseSavedAsync() => _savedJson == null ? Task.CompletedTask : SaveAsync(_savedJson);

    [RelayCommand]
    private Task CancelAsync() => Nav.GoAsync("..");
}
