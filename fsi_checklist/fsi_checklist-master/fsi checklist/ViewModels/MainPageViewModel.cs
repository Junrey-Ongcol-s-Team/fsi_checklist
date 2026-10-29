using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fsi_checklist.Views;


namespace fsi_checklist.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        [RelayCommand]
        private async Task OpenInspectionPageAsync()
        {
            await Shell.Current.GoToAsync(nameof(InspectionPage));
        }

        [RelayCommand]
        private async Task OpenSettingsAsync()
        {
            await Shell.Current.GoToAsync(nameof(SettingsPage));
        }
    }
}
