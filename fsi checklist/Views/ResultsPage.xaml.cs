// =====================================================================
// VIEW RESULTS - code-behind: ikonektar ang page sa iyang ViewModel.
// Ang pag-load mahitabo kung madawat ang inspectionId (tan-awa ResultsPageViewModel).
// =====================================================================
using fsi_checklist.ViewModels;

namespace fsi_checklist.Views;

public partial class ResultsPage : ContentPage
{
    public ResultsPage(ResultsPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    // I-tap ang litrato = ablihan og dako (PhotoViewerPage). Ang BindingContext mao ang path sa litrato.
    private async void OnPhotoTapped(object? sender, TappedEventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is string path)
            await Navigation.PushAsync(new PhotoViewerPage(path));
    }
}
