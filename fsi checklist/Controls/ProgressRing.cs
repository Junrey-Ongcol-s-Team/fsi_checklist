// =====================================================================
// ProgressRing: gamay nga lingin (circle) nga progress.
//   Progress  = 0.0 hangtod 1.0 (pila na ang natubag)
//   RingColor = kolor sa napuno nga bahin (yellow kung kulang pa, green kung kompleto)
// Gigamit sa header sa matag numero sa Section V (069). Walay dugang nga library: GraphicsView ra.
// =====================================================================
namespace fsi_checklist.Controls;

public sealed class ProgressRing : GraphicsView, IDrawable
{
    public static readonly BindableProperty ProgressProperty = BindableProperty.Create(
        nameof(Progress), typeof(double), typeof(ProgressRing), 0.0,
        propertyChanged: (b, _, _) => ((ProgressRing)b).Invalidate());

    public static readonly BindableProperty RingColorProperty = BindableProperty.Create(
        nameof(RingColor), typeof(Color), typeof(ProgressRing), Color.FromArgb("#F9A825"),
        propertyChanged: (b, _, _) => ((ProgressRing)b).Invalidate());

    public double Progress
    {
        get => (double)GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    public Color RingColor
    {
        get => (Color)GetValue(RingColorProperty);
        set => SetValue(RingColorProperty, value);
    }

    public ProgressRing()
    {
        Drawable = this;
        BackgroundColor = Colors.Transparent;
    }

    public void Draw(ICanvas canvas, RectF bounds)
    {
        const float stroke = 4;
        float size = Math.Min(bounds.Width, bounds.Height) - stroke;
        var rect = new RectF(
            bounds.Center.X - size / 2, bounds.Center.Y - size / 2, size, size);

        // Abuhon nga lingin (ang tibuok)
        canvas.StrokeSize = stroke;
        canvas.StrokeColor = Color.FromArgb("#40808080");
        canvas.DrawEllipse(rect);

        // Ang napuno nga bahin, sugod sa ibabaw, padulong sa tuo (clockwise)
        double p = Math.Clamp(Progress, 0, 1);
        if (p <= 0)
            return;

        canvas.StrokeColor = RingColor;
        canvas.StrokeLineCap = LineCap.Round;
        if (p >= 1)
            canvas.DrawEllipse(rect);
        else
            canvas.DrawArc(rect, 90, (float)(90 - 360 * p), true, false);
    }
}
