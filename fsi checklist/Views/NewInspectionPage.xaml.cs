using fsi_checklist.ViewModels;

namespace fsi_checklist.Views;

public partial class NewInspectionPage : ContentPage
{
    private readonly NewInspectionViewModel _viewModel;

    public NewInspectionPage(NewInspectionViewModel viewModel)
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
