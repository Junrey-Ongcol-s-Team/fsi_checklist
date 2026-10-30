// =====================================================================
// ViewModel sa Settings page (ngalan ug address sa istasyon, ug ang PRE-SIGNED nga pirma sa BFP).
// =====================================================================
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fsi_checklist.Controls;
using fsi_checklist.Services;

namespace fsi_checklist.ViewModels;

/// <summary>One saved BFP signature on the Settings page.</summary>
public sealed class PresignRow
{
    public required string RoleCode { get; init; }
    public required string RoleLabel { get; init; }
    public required string NameText { get; init; }
    public required string ButtonText { get; init; }
    public SignaturePreviewDrawable? Preview { get; init; }
    public bool HasPreview => Preview != null;
}

public sealed partial class SettingsViewModel : ObservableObject
{
    private readonly AppSettingsService _settings;
    private readonly DatabaseService _database;
    private readonly InspectionService _inspections;

    public SettingsViewModel(AppSettingsService settings, DatabaseService database, InspectionService inspections)
    {
        _settings = settings;
        _database = database;
        _inspections = inspections;
    }

    // PRE-SIGNED: listahan sa mga taga-BFP ug ilang saved nga pirma.
    [ObservableProperty] private List<PresignRow> presigns = new();

    // Para sa debugging: asa ang database ug ang log file. Pwede i-copy gikan sa screen.
    public string DatabasePath => DatabaseService.DatabasePath;
    public string LogPath => AppLog.LogPath;

    [ObservableProperty] private string stationName = string.Empty;
    [ObservableProperty] private string stationAddress = string.Empty;

    [RelayCommand]
    public async Task LoadAsync()
    {
        try
        {
            StationName = await _settings.GetAsync(AppSettingsService.StationName);
            StationAddress = await _settings.GetAsync(AppSettingsService.StationAddress);

            Presigns = (await _inspections.GetPresignsAsync())
                .Select(p => new PresignRow
                {
                    RoleCode = p.RoleCode,
                    RoleLabel = p.RoleLabel,
                    NameText = p.HasSignature ? p.Name : "Not signed yet",
                    ButtonText = p.HasSignature ? "Change signature" : "Sign now",
                    Preview = p.HasSignature ? SignatureJson.Parse(p.StrokesJson) : null,
                })
                .ToList();
        }
        catch (Exception ex)
        {
            AppLog.Error(ex);
            await Dialogs.ShowAsync("Couldn't load settings", AppLog.Describe(ex));
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        try
        {
            await _settings.SetAsync(AppSettingsService.StationName, StationName.Trim());
            await _settings.SetAsync(AppSettingsService.StationAddress, StationAddress.Trim());
            await Dialogs.ShowAsync("Saved", "Settings saved.");
        }
        catch (Exception ex)
        {
            AppLog.Error(ex);
            await Dialogs.ShowAsync("Couldn't save settings", AppLog.Describe(ex));
        }
    }

    // RESET DATABASE: i-backup ang daan ug himoon og bag-o (mawala sa app ang mga inspection,
    // apan naa gihapon sa backup file). Gamita kini kung naay sayop sa database samtang nag-develop.
    [RelayCommand]
    private async Task ResetDatabaseAsync()
    {
        bool ok = await Dialogs.ConfirmAsync(
            "Reset database?",
            "All inspections will be removed from the app. The old database is kept as a backup file.",
            "Reset");
        if (!ok)
            return;

        try
        {
            string? backup = await _database.ResetDatabaseAsync();
            await _database.InitializeAsync();
            await LoadAsync();
            await Dialogs.ShowAsync("Database reset",
                backup == null ? "A new database was created." : $"A new database was created.\nBackup: {backup}");
        }
        catch (Exception ex)
        {
            AppLog.Error(ex);
            await Dialogs.ShowAsync("Couldn't reset the database", AppLog.Describe(ex));
        }
    }
}
