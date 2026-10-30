// =====================================================================
// Code-behind sa daan nga listahan (dili na gigamit).
// =====================================================================
using fsi_checklist.ViewModels;

namespace fsi_checklist.Views;

public partial class InspectionPage : ContentPage
{
    private readonly InspectionPageViewModel _viewModel;

    public InspectionPage(InspectionPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadCommand.ExecuteAsync(null);
    }
}
