// =====================================================================
// ViewModel sa 'New inspection' page: dinhi isulat ang ngalan, address, owner, contact, IO no. ug petsa.
// Pag-pindot sa Start: himoon ang subject + inspection sa database, dayon ablihan ang form.
// =====================================================================
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fsi_checklist.Data.Models;
using fsi_checklist.Services;
using fsi_checklist.Views;

namespace fsi_checklist.ViewModels;

[QueryProperty(nameof(TemplateId), "templateId")]
public sealed partial class NewInspectionViewModel : ObservableObject
{
    private readonly InspectionService _inspections;

    public NewInspectionViewModel(InspectionService inspections)
    {
        _inspections = inspections;
    }

    public ObservableCollection<FormTemplate> Templates { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(SubjectLabel))]
    private FormTemplate? selectedTemplate;

    [ObservableProperty] private string subjectName = string.Empty;
    [ObservableProperty] private string address = string.Empty;
    [ObservableProperty] private string ownerName = string.Empty;
    [ObservableProperty] private string contactNo = string.Empty;
    [ObservableProperty] private string referenceNo = string.Empty;
    [ObservableProperty] private DateTime dateInspected = DateTime.Today;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private bool hasNoTemplates;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsTemplateFixed))]
    [NotifyPropertyChangedFor(nameof(ShowPicker))]
    private string? templateId;

    public bool IsTemplateFixed => !string.IsNullOrEmpty(TemplateId);
    public bool ShowPicker => !IsTemplateFixed;

    public string SubjectLabel => SelectedTemplate?.SubjectType switch
    {
        "VEHICLE" => "Plate number",
        "CONSTRUCTION_PROJECT" => "Project title",
        "ESTABLISHMENT" => "Name of establishment / building",
        _ => "Name",
    };

    [RelayCommand]
    public async Task LoadAsync()
    {
        try
        {
            var templates = await _inspections.GetAvailableTemplatesAsync();
            Templates.Clear();
            foreach (var template in templates)
                Templates.Add(template);

            HasNoTemplates = Templates.Count == 0;
            // Kung gikan sa dagkong button, napili na ang form, busa itago ang dropdown.
            if (!string.IsNullOrEmpty(TemplateId))
                SelectedTemplate = Templates.FirstOrDefault(t => t.Id == TemplateId);
            else if (SelectedTemplate == null && Templates.Count == 1)
                SelectedTemplate = Templates[0];
        }
        catch (Exception ex)
        {
            AppLog.Error(ex);
            await Dialogs.ShowAsync("Couldn't load checklists", AppLog.Describe(ex));
        }
    }

    // Pag-pindot sa Start: susiha ang ngalan, himoon ang inspection, dayon ablihan ang form.
    [RelayCommand]
    private async Task CreateAsync()
    {
        if (SelectedTemplate == null)
        {
            await Dialogs.ShowAsync("Choose a checklist", "Pick which checklist you're filling in.");
            return;
        }

        if (string.IsNullOrWhiteSpace(SubjectName))
        {
            await Dialogs.ShowAsync("Missing name", $"Enter the {SubjectLabel.ToLowerInvariant()}.");
            return;
        }

        IsBusy = true;
        try
        {
            string id = await _inspections.CreateInspectionAsync(new NewInspectionRequest(
                SelectedTemplate.Id, SubjectName, Address, OwnerName, ContactNo, ReferenceNo, DateInspected));

            // Replace this page and the checklist-types page with the form (Back / Save draft then lands on the dashboard).
            await Nav.GoAsync($"../../{nameof(FormPage)}?inspectionId={Uri.EscapeDataString(id)}");
        }
        catch (Exception ex)
        {
            AppLog.Error(ex);
            await Dialogs.ShowAsync("Couldn't create inspection", AppLog.Describe(ex));
        }
        finally
        {
            IsBusy = false;
        }
    }
}
