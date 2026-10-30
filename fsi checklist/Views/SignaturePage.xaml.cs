// =====================================================================
// E-SIGNATURE page - code-behind: dinhi madawat ang paghikap sa tudlo ug mahimong linya sa kahon.
// =====================================================================
using fsi_checklist.Controls;
using fsi_checklist.ViewModels;
using Microsoft.Maui.Graphics;

namespace fsi_checklist.Views;

public partial class SignaturePage : ContentPage
{
    private readonly SignaturePageViewModel _viewModel;
    private readonly SignaturePadDrawable _drawable = new();

    public SignaturePage(SignaturePageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
        // Ikonektar ang kahon (GraphicsView) sa atong drawable.
        Pad.Drawable = _drawable;
    }

    // Pag-tandog sa tudlo: sugdi ang bag-ong linya.
    private void OnPadStart(object? sender, TouchEventArgs e)
    {
        if (e.Touches.Length == 0)
            return;

        _drawable.Strokes.Add(new List<PointF> { e.Touches[0] });
        Pad.Invalidate();
    }

    // Samtang ginalihok ang tudlo: dugangi og point ang linya, dayon i-redraw (Invalidate).
    private void OnPadDrag(object? sender, TouchEventArgs e)
    {
        if (e.Touches.Length == 0 || _drawable.Strokes.Count == 0)
            return;

        _drawable.Strokes[^1].Add(e.Touches[0]);
        Pad.Invalidate();
    }

    // Pag-alsa sa tudlo: tapuson ang linya.
    private void OnPadEnd(object? sender, TouchEventArgs e)
    {
        if (e.Touches.Length > 0 && _drawable.Strokes.Count > 0)
            _drawable.Strokes[^1].Add(e.Touches[0]);

        Pad.Invalidate();
    }

    // Kung na-cancel ang paghikap (pananglitan migawas ang tudlo sa kahon):
    // i-redraw lang ang kahon. Walay TouchEventArgs dinhi, EventArgs ra.
    private void OnPadCancel(object? sender, EventArgs e) => Pad.Invalidate();

    // Clear: papason ang pirma aron mo-pirma og usab.
    private void OnClear(object? sender, EventArgs e)
    {
        _drawable.Strokes.Clear();
        Pad.Invalidate();
    }

    // Save: kung naay pirma, himoon kini nga JSON ug ipadala sa ViewModel aron i-save.
    private async void OnSave(object? sender, EventArgs e)
    {
        if (_drawable.IsEmpty)
        {
            _viewModel.ErrorText = "Please sign inside the box first.";
            return;
        }

        await _viewModel.SaveAsync(SignatureJson.Build(_drawable.Strokes));
    }
}
