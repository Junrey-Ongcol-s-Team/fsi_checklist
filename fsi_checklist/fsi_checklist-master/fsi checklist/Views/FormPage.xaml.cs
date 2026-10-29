using fsi_checklist.ViewModels;

namespace fsi_checklist.Views;

public partial class FormPage : ContentPage
{
    private readonly FormPageViewModel _viewModel;

    public FormPage(FormPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Make sure nothing typed in the last moments is lost.
        _ = _viewModel.FlushAsync();
    }
}
