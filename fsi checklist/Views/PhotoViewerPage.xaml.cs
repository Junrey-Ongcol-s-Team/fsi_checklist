// =====================================================================
// PHOTO VIEWER - code-behind (para sa CELLPHONE).
// Gamiton:  await Navigation.PushAsync(new PhotoViewerPage(fullPath));
// Walay ViewModel ug walay route - simple ra: dawaton ang path sa litrato, ipakita.
//   - Pinch (duha ka tudlo)  = zoom 1x hangtod 5x
//   - Drag (usa ka tudlo)    = i-move ang litrato kung naka-zoom
//   - Double-tap             = zoom 2.5x / balik sa normal
// =====================================================================
namespace fsi_checklist.Views;

public partial class PhotoViewerPage : ContentPage
{
    private double _startScale = 1;
    private double _startX, _startY;

    public PhotoViewerPage(string fullPath)
    {
        InitializeComponent();
        Photo.Source = ImageSource.FromFile(fullPath);
    }

    // Pinch = zoom.
    private void OnPinchUpdated(object? sender, PinchGestureUpdatedEventArgs e)
    {
        if (e.Status == GestureStatus.Started)
            _startScale = Photo.Scale;
        else if (e.Status == GestureStatus.Running)
            SetZoom(_startScale * e.Scale);
    }

    // Drag = i-move ang litrato (kung naka-zoom ra).
    private void OnPanUpdated(object? sender, PanUpdatedEventArgs e)
    {
        if (Photo.Scale <= 1)
            return;

        if (e.StatusType == GestureStatus.Started)
        {
            _startX = Photo.TranslationX;
            _startY = Photo.TranslationY;
        }
        else if (e.StatusType == GestureStatus.Running)
        {
            Photo.TranslationX = _startX + e.TotalX;
            Photo.TranslationY = _startY + e.TotalY;
            KeepInside();
        }
    }

    // Double-tap = zoom in, o balik sa normal kung naka-zoom na.
    private void OnDoubleTapped(object? sender, TappedEventArgs e) => SetZoom(Photo.Scale > 1 ? 1 : 2.5);

    private void SetZoom(double scale)
    {
        Photo.Scale = Math.Clamp(scale, 1, 5);
        KeepInside();
    }

    // Dili tugotan nga mo-layas ang litrato gawas sa screen.
    private void KeepInside()
    {
        var maxX = Photo.Width * (Photo.Scale - 1) / 2;
        var maxY = Photo.Height * (Photo.Scale - 1) / 2;
        Photo.TranslationX = Math.Clamp(Photo.TranslationX, -maxX, maxX);
        Photo.TranslationY = Math.Clamp(Photo.TranslationY, -maxY, maxY);
    }

    private async void OnCloseClicked(object? sender, EventArgs e) => await Navigation.PopAsync();
}
