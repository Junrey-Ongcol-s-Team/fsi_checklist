using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fsi_checklist.Services;

namespace fsi_checklist.ViewModels;

public sealed partial class SettingsViewModel : ObservableObject
{
    private readonly AppSettingsService _settings;

    public SettingsViewModel(AppSettingsService settings)
    {
        _settings = settings;
    }

    [ObservableProperty] private string stationName = string.Empty;
    [ObservableProperty] private string stationAddress = string.Empty;

    [RelayCommand]
    public async Task LoadAsync()
    {
        try
        {
            StationName = await _settings.GetAsync(AppSettingsService.StationName);
            StationAddress = await _settings.GetAsync(AppSettingsService.StationAddress);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            await Dialogs.ShowAsync("Couldn't load settings", ex.Message);
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
            System.Diagnostics.Debug.WriteLine(ex);
            await Dialogs.ShowAsync("Couldn't save settings", ex.Message);
        }
    }
}
