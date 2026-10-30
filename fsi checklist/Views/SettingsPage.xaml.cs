// =====================================================================
// Code-behind sa Settings page.
// =====================================================================
using fsi_checklist.Services;
using fsi_checklist.ViewModels;

namespace fsi_checklist.Views;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsViewModel _viewModel;

    public SettingsPage(SettingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadCommand.ExecuteAsync(null);
    }

    // "Sign now / Change signature": ablihan ang SignaturePage para i-save ang pre-signed nga pirma.
    // (Code-behind, dili RelativeSource, aron dili mo-crash.)
    private async void OnPresignClicked(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is PresignRow row)
            await Nav.GoAsync($"{nameof(SignaturePage)}?presignRole={row.RoleCode}");
    }
}
