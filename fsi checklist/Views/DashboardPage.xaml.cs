// =====================================================================
// Code-behind sa Dashboard: i-reload ang listahan matag balik sa page.
// =====================================================================
using fsi_checklist.ViewModels;

namespace fsi_checklist.Views;

public partial class DashboardPage : ContentPage
{
    private readonly DashboardPageViewModel _viewModel;

    public DashboardPage(DashboardPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    // PAG-TAP SA CARD: ablihan ang inspection (form kung draft, sign-off kung dili).
    private async void OnCardTapped(object? sender, TappedEventArgs e)
    {
        var row = e.Parameter as DashboardRow ?? (sender as BindableObject)?.BindingContext as DashboardRow;
        if (row != null)
            await _viewModel.OpenCommand.ExecuteAsync(row);
    }

    // VIEW RESULTS button sa card: ablihan ang Results page para niini nga inspection.
    private async void OnViewResultsClicked(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is DashboardRow row)
            await fsi_checklist.Services.Nav.GoAsync($"{nameof(ResultsPage)}?inspectionId={Uri.EscapeDataString(row.Id)}");
    }

    // SWIPE -> DELETE: kuhaon ang card nga gi-swipe, dayon ipadala sa DeleteCommand sa ViewModel.
    private async void OnDeleteInvoked(object? sender, EventArgs e)
    {
        if (sender is not SwipeItem item)
            return;

        var row = item.CommandParameter as DashboardRow ?? item.BindingContext as DashboardRow;
        if (row != null)
            await _viewModel.DeleteCommand.ExecuteAsync(row);
    }

    // Reload every time the page is shown, so a draft you just left shows its new progress.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadCommand.ExecuteAsync(null);
    }
}
