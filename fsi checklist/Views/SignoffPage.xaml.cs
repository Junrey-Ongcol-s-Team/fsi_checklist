// =====================================================================
// Code-behind sa Sign-off: i-reload kung mobalik gikan sa pag-pirma.
// =====================================================================
using fsi_checklist.ViewModels;

namespace fsi_checklist.Views;

public partial class SignoffPage : ContentPage
{
    private readonly SignoffPageViewModel _viewModel;

    public SignoffPage(SignoffPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    // PAG-TAP SA "Sign": ablihan ang pirma nga page para niini nga linya.
    // (Code-behind imbes RelativeSource, kay ang RelativeSource mahimong null ug mo-crash.)
    private async void OnSignClicked(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is SignatoryItem item)
            await _viewModel.SignCommand.ExecuteAsync(item);
    }

    // Reload when coming back from the signature page, so the new signature shows.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadCommand.ExecuteAsync(null);
    }
}
