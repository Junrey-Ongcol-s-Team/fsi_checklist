// =====================================================================
// ViewModel sa 'Edit details' page: usbon ang UNANG gi-fill up sa inspection
// (ngalan, address, owner, contact number, IO no., petsa). STEP 1 sa "Edit" sa Dashboard;
// human ma-save, mopadayon sa form (entries).
// =====================================================================
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fsi_checklist.Services;
using fsi_checklist.Views;

namespace fsi_checklist.ViewModels;

[QueryProperty(nameof(InspectionId), "inspectionId")]
public sealed partial class EditDetailsViewModel : ObservableObject
{
    private readonly InspectionService _inspections;
    private string _subjectType = string.Empty;
    private string _loadedId = string.Empty;

    public EditDetailsViewModel(InspectionService inspections)
    {
        _inspections = inspections;
    }

    [ObservableProperty] private string inspectionId = string.Empty;
    [ObservableProperty] private string formName = string.Empty;
    [ObservableProperty] private string subjectLabel = "Name";
    [ObservableProperty] private string subjectName = string.Empty;
    [ObservableProperty] private string address = string.Empty;
    [ObservableProperty] private string ownerName = string.Empty;
    [ObservableProperty] private string contactNo = string.Empty;
    [ObservableProperty] private string referenceNo = string.Empty;
    [ObservableProperty] private DateTime dateInspected = DateTime.Today;
    [ObservableProperty] private bool isBusy;

    // I-load ang detalye sa inspection (kausa ra matag inspection, aron dili mawala ang gi-type)
    [RelayCommand]
    public async Task LoadAsync()
    {
        if (string.IsNullOrEmpty(InspectionId) || _loadedId == InspectionId)
            return;
        try
        {
            var d = await _inspections.GetDetailsAsync(InspectionId);
            if (d == null)
            {
                await Dialogs.ShowAsync("Not found", "This inspection no longer exists.");
                await Nav.GoAsync("..");
                return;
            }
            if (d.IsLocked)
            {
                await Dialogs.ShowAsync("Can't edit", "This inspection is locked, so its details can no longer be edited.");
                await Nav.GoAsync("..");
                return;
            }

            _loadedId = InspectionId;
            _subjectType = d.SubjectType;
            FormName = d.TemplateName;
            SubjectLabel = d.SubjectType switch
            {
                "VEHICLE" => "Plate number",
                "CONSTRUCTION_PROJECT" => "Project title",
                "ESTABLISHMENT" => "Name of establishment / building",
                _ => "Name",
            };
            SubjectName = d.SubjectName;
            Address = d.Address;
            OwnerName = d.OwnerName;
            ContactNo = d.ContactNo;
            ReferenceNo = d.ReferenceNo;
            DateInspected = d.DateInspected;
        }
        catch (Exception ex)
        {
            AppLog.Error(ex);
            await Dialogs.ShowAsync("Couldn't load the details", AppLog.Describe(ex));
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(SubjectName))
        {
            await Dialogs.ShowAsync("Missing name", $"Enter the {SubjectLabel.ToLowerInvariant()}.");
            return;
        }
        // Kung naay contact number, kinahanglan eksakto 11 ka numero.
        if (ContactNo.Length > 0 && ContactNo.Length != 11)
        {
            await Dialogs.ShowAsync("Contact number", "The contact number must have exactly 11 digits (example: 09171234567).");
            return;
        }

        IsBusy = true;
        try
        {
            var (ok, message) = await _inspections.UpdateDetailsAsync(new InspectionDetailsData(
                InspectionId, FormName, _subjectType, SubjectName, Address, OwnerName, ContactNo, ReferenceNo,
                DateInspected, false));
            if (!ok)
            {
                await Dialogs.ShowAsync("Can't save", message);
                return;
            }
            // STEP 2: ablihan ang form (entries). Ilisan kini nga page aron ang Back mobalik sa Dashboard.
            await Nav.GoAsync($"../{nameof(FormPage)}?inspectionId={Uri.EscapeDataString(InspectionId)}");
        }
        catch (Exception ex)
        {
            AppLog.Error(ex);
            await Dialogs.ShowAsync("Couldn't save the details", AppLog.Describe(ex));
        }
        finally
        {
            IsBusy = false;
        }
    }
}
