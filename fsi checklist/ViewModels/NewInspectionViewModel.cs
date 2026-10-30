using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fsi_checklist.Data.Models;
using fsi_checklist.Services;
using fsi_checklist.Views;

namespace fsi_checklist.ViewModels;

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
    [NotifyPropertyChangedFor(nameof(HasTemplateError))]
    private FormTemplate? selectedTemplate;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasNameError))]
    private string subjectName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasNameError))]
    [NotifyPropertyChangedFor(nameof(HasTemplateError))]
    private bool showErrors;
    [ObservableProperty] private string address = string.Empty;
    [ObservableProperty] private string ownerName = string.Empty;
    [ObservableProperty] private string contactNo = string.Empty;
    [ObservableProperty] private string referenceNo = string.Empty;
    [ObservableProperty] private DateTime dateInspected = DateTime.Today;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private bool hasNoTemplates;

    public bool HasNameError => ShowErrors && string.IsNullOrWhiteSpace(SubjectName);
    public bool HasTemplateError => ShowErrors && SelectedTemplate == null;

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
            if (SelectedTemplate == null && Templates.Count == 1)
                SelectedTemplate = Templates[0];
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await Dialogs.ShowAsync("Couldn't load checklists", ex.Message);
        }
    }

    [RelayCommand]
    private async Task CreateAsync()
    {
        ShowErrors = true;
        if (SelectedTemplate == null || string.IsNullOrWhiteSpace(SubjectName))
            return;

        IsBusy = true;
        try
        {
            string id = await _inspections.CreateInspectionAsync(new NewInspectionRequest(
                SelectedTemplate.Id, SubjectName, Address, OwnerName, ContactNo, ReferenceNo, DateInspected));

            // Replace this page with the form.
            await Shell.Current.GoToAsync($"../{nameof(FormPage)}?inspectionId={Uri.EscapeDataString(id)}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await Dialogs.ShowAsync("Couldn't create inspection", ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
