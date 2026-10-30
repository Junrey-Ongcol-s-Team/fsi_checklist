// =====================================================================
// Code-behind sa Form: i-save ang nahabilin nga tubag kung mobiya sa page, ug mo-scroll sa kulang nga item.
// =====================================================================
using fsi_checklist.ViewModels;

namespace fsi_checklist.Views;

public partial class FormPage : ContentPage
{
    private readonly FormPageViewModel _viewModel;

    public FormPage(FormPageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;

        // Kung mag-tap og 'Next missing', mo-scroll sa kulang nga item.
        _viewModel.ScrollToRowRequested += async index =>
        {
            if (index >= 0 && index < RowsLayout.Children.Count && RowsLayout.Children[index] is Element target)
                await FormScroll.ScrollToAsync(target, ScrollToPosition.Start, true);
        };
    }

    // Kung mobiya sa page, i-save ang nahabilin.
    // LITRATO: ang mga button sa litrato. (Code-behind, dili RelativeSource, aron dili mo-crash.)
    private async void OnTakePhotoClicked(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is PhotoSlotViewModel slot)
            await _viewModel.AddPhotoAsync(slot, useCamera: true);
    }

    private async void OnChoosePhotoClicked(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is PhotoSlotViewModel slot)
            await _viewModel.AddPhotoAsync(slot, useCamera: false);
    }

    // I-tap ang gamay nga litrato = ablihan og dako (PhotoViewerPage).
    private async void OnPhotoTapped(object? sender, TappedEventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is PhotoViewModel photo)
            await Navigation.PushAsync(new PhotoViewerPage(photo.FullPath));
    }

    private async void OnRemovePhotoClicked(object? sender, EventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is PhotoViewModel photo)
            await _viewModel.RemovePhotoAsync(photo);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Make sure nothing typed in the last moments is lost.
        _ = _viewModel.FlushAsync();
    }
}
