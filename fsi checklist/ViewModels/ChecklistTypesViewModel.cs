// =====================================================================
// Ang ViewModel sa page nga naay dagkong button sa matag klase sa inspection.
// Ang mga button gikan sa database (form_template), dili hardcoded.
// =====================================================================
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fsi_checklist.Services;
using fsi_checklist.Views;

namespace fsi_checklist.ViewModels;

public sealed record ChecklistTile(string TemplateId, string Title, string Subtitle, bool IsInstalled)
{
    public double TileOpacity => IsInstalled ? 1.0 : 0.55;
}

public sealed partial class ChecklistTypesViewModel : ObservableObject
{
    private readonly InspectionService _inspections;

    public ChecklistTypesViewModel(InspectionService inspections)
    {
        _inspections = inspections;
    }

    [ObservableProperty]
    private List<ChecklistTile> tiles = new();

    [RelayCommand]
    public async Task LoadAsync()
    {
        try
        {
            var checklists = await _inspections.GetChecklistsAsync();
            Tiles = checklists
                .Select(c => new ChecklistTile(
                    c.Id,
                    c.DisplayName,
                    c.IsInstalled ? SubjectText(c.SubjectType) : "Coming soon",
                    c.IsInstalled))
                .ToList();
        }
        catch (Exception ex)
        {
            AppLog.Error(ex);
            await Dialogs.ShowAsync("Couldn't load checklists", AppLog.Describe(ex));
        }
    }

    private static string SubjectText(string subjectType) => subjectType switch
    {
        "VEHICLE" => "Vehicle",
        "CONSTRUCTION_PROJECT" => "Construction project",
        "ESTABLISHMENT" => "Building or business",
        _ => "Other",
    };

    // Pag-tap sa dagkong button: ablihan ang 'New inspection' nga napili na ang form.
    [RelayCommand]
    private async Task OpenAsync(ChecklistTile? tile)
    {
        if (tile == null)
            return;

        if (!tile.IsInstalled)
        {
            await Dialogs.ShowAsync("Coming soon", $"The {tile.Title} checklist isn't installed yet.");
            return;
        }

        await Nav.GoAsync(
            $"{nameof(NewInspectionPage)}?templateId={Uri.EscapeDataString(tile.TemplateId)}");
    }
}
