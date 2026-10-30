// =====================================================================
// DAAN nga listahan sa inspections. Wala na kini gigamit; gipulihan sa Dashboard (page 2).
// =====================================================================
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fsi_checklist.Models;
using fsi_checklist.Services;
using fsi_checklist.Views;

namespace fsi_checklist.ViewModels
{
    public sealed partial class InspectionPageViewModel : ObservableObject
    {
        private readonly InspectionService _inspections;

        public InspectionPageViewModel(InspectionService inspections)
        {
            _inspections = inspections;
        }

        public ObservableCollection<InspectionListItem> Items { get; } = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EmptyText))]
        private string searchText = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EmptyText))]
        private string selectedFilter = "ALL";

        [ObservableProperty]
        private bool isRefreshing;

        public string EmptyText =>
            !string.IsNullOrWhiteSpace(SearchText) ? "No inspections match your search."
            : SelectedFilter switch
            {
                "DRAFT" => "No draft inspections.",
                "COMPLETED" => "No completed inspections yet.",
                _ => "No inspections yet. Tap “New inspection” to start.",
            };

        [RelayCommand]
        public async Task LoadAsync()
        {
            try
            {
                var list = await _inspections.GetInspectionsAsync(SearchText, SelectedFilter);
                Items.Clear();
                foreach (var item in list)
                    Items.Add(item);
            }
            catch (Exception ex)
            {
                AppLog.Error(ex);
                await Dialogs.ShowAsync("Couldn't load inspections", AppLog.Describe(ex));
            }
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            IsRefreshing = true;
            try { await LoadAsync(); }
            finally { IsRefreshing = false; }
        }

        [RelayCommand]
        private async Task SetFilterAsync(string? filter)
        {
            SelectedFilter = filter ?? "ALL";
            await LoadAsync();
        }

        [RelayCommand]
        private Task NewInspectionAsync() =>
            Nav.GoAsync(nameof(NewInspectionPage));

        [RelayCommand]
        private Task OpenSettingsAsync() =>
            Nav.GoAsync(nameof(SettingsPage));

        [RelayCommand]
        private Task OpenAsync(InspectionListItem? item) =>
            item == null
                ? Task.CompletedTask
                : Nav.GoAsync($"{nameof(FormPage)}?inspectionId={Uri.EscapeDataString(item.Id)}");

        [RelayCommand]
        private async Task DeleteAsync(InspectionListItem? item)
        {
            if (item == null)
                return;

            if (item.IsCompleted)
            {
                await Dialogs.ShowAsync("Can't delete", "Completed inspections are locked and can't be deleted.");
                return;
            }

            if (!await Dialogs.ConfirmAsync("Delete draft?", $"Delete the draft inspection for “{item.SubjectName}”?"))
                return;

            try
            {
                if (await _inspections.DeleteDraftAsync(item.Id))
                    Items.Remove(item);
            }
            catch (Exception ex)
            {
                AppLog.Error(ex);
                await Dialogs.ShowAsync("Couldn't delete", AppLog.Describe(ex));
            }
        }
    }
}
