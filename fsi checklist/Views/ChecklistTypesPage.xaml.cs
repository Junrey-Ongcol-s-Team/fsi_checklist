// =====================================================================
// Code-behind: i-load ang mga button matag higayon nga makita ang page.
// =====================================================================
using fsi_checklist.ViewModels;

namespace fsi_checklist.Views;

public partial class ChecklistTypesPage : ContentPage
{
    private readonly ChecklistTypesViewModel _viewModel;

    public ChecklistTypesPage(ChecklistTypesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    // PAG-TAP SA DAGKONG BUTTON: ablihan ang New inspection para niini nga form.
    private async void OnTileTapped(object? sender, TappedEventArgs e)
    {
        var tile = e.Parameter as ChecklistTile ?? (sender as BindableObject)?.BindingContext as ChecklistTile;
        if (tile != null)
            await _viewModel.OpenCommand.ExecuteAsync(tile);
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadCommand.ExecuteAsync(null);
    }
}
