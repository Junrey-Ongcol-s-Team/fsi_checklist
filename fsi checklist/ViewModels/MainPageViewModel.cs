// =====================================================================
// ViewModel sa Main page: ang 'Inspections' button moabli sa Dashboard, ang 'Settings' sa Settings page.
// =====================================================================
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fsi_checklist.Services;
using fsi_checklist.Views;


namespace fsi_checklist.ViewModels
{
    public partial class MainPageViewModel : ObservableObject
    {
        [RelayCommand]
        private async Task OpenInspectionPageAsync()
        {
            await Nav.GoAsync(nameof(DashboardPage));
        }

        [RelayCommand]
        private async Task OpenSettingsAsync()
        {
            await Nav.GoAsync(nameof(SettingsPage));
        }
    }
}
