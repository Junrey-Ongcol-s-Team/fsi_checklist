// =====================================================================
// E-SIGNATURE: ang pag-drawing ug pag-tipig sa pirma.
//   SignaturePadDrawable     = ang kahon diin mo-pirma gamit ang tudlo (o mouse/stylus)
//   SignaturePreviewDrawable = ang gamay nga hulagway sa pirma nga na-save na
//   SignatureJson            = himoon ang pirma nga JSON text (listahan sa mga linya/points), ug basahon balik
// =====================================================================
using System.Text.Json;
using Microsoft.Maui.Graphics;

namespace fsi_checklist.Controls;

/// <summary>The live drawing box: points are kept in the box's own coordinates.</summary>
public sealed class SignaturePadDrawable : IDrawable
{
    public List<List<PointF>> Strokes { get; } = new();

    public bool IsEmpty => Strokes.Count == 0;

    // Draw: tawgon sa MAUI matag Invalidate(). Puti nga background, dayon i-drawing ang matag linya (stroke).
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = Colors.White;
        canvas.FillRectangle(dirtyRect);

        canvas.StrokeColor = Colors.Black;
        canvas.StrokeSize = 3;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.StrokeLineJoin = LineJoin.Round;

        foreach (var stroke in Strokes)
        {
            if (stroke.Count == 0)
                continue;

            if (stroke.Count == 1)
            {
                canvas.FillColor = Colors.Black;
                canvas.FillCircle(stroke[0], 1.5f);
                continue;
            }

            var path = new PathF();
            path.MoveTo(stroke[0]);
            for (int i = 1; i < stroke.Count; i++)
                path.LineTo(stroke[i]);
            canvas.DrawPath(path);
        }
    }
}

/// <summary>A saved signature, drawn to fit any box without stretching.</summary>
public sealed class SignaturePreviewDrawable : IDrawable
{
    public float Aspect { get; set; } = 0.3f;
    public List<List<PointF>> Strokes { get; } = new();

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = Colors.White;
        canvas.FillRectangle(dirtyRect);

        if (Strokes.Count == 0)
            return;

        const float margin = 6f;
        float availW = Math.Max(1f, dirtyRect.Width - 2 * margin);
        float availH = Math.Max(1f, dirtyRect.Height - 2 * margin);
        float aspect = Math.Max(Aspect, 0.01f);
        float scale = Math.Min(availW, availH / aspect);
        float offX = dirtyRect.X + (dirtyRect.Width - scale) / 2f;
        float offY = dirtyRect.Y + (dirtyRect.Height - scale * aspect) / 2f;

        canvas.StrokeColor = Colors.Black;
        canvas.StrokeSize = 2;
        canvas.StrokeLineCap = LineCap.Round;
        canvas.StrokeLineJoin = LineJoin.Round;

        PointF Map(PointF p) => new(offX + p.X * scale, offY + p.Y * scale);

        foreach (var stroke in Strokes)
        {
            if (stroke.Count == 0)
                continue;

            if (stroke.Count == 1)
            {
                canvas.FillColor = Colors.Black;
                canvas.FillCircle(Map(stroke[0]), 1.5f);
                continue;
            }

            var path = new PathF();
            path.MoveTo(Map(stroke[0]));
            for (int i = 1; i < stroke.Count; i++)
                path.LineTo(Map(stroke[i]));
            canvas.DrawPath(path);
        }
    }
}

/// <summary>Signatures are stored as small JSON files (the pen strokes), not pictures.</summary>
public static class SignatureJson
{
    public sealed class Dto
    {
        public float Aspect { get; set; }
        public List<List<float[]>> Strokes { get; set; } = new();
    }

    // Himoon ang pirma nga JSON. Gi-normalize ang mga point (0 hangtod 1) aron mohaum bisan unsa ka dako ang screen.
    public static string Build(IReadOnlyList<List<PointF>> strokes)
    {
        float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;
        foreach (var stroke in strokes)
        {
            foreach (var p in stroke)
            {
                minX = Math.Min(minX, p.X);
                minY = Math.Min(minY, p.Y);
                maxX = Math.Max(maxX, p.X);
                maxY = Math.Max(maxY, p.Y);
            }
        }

        if (minX > maxX)
        {
            minX = minY = 0;
            maxX = maxY = 1;
        }

        float w = Math.Max(1f, maxX - minX);
        float h = Math.Max(1f, maxY - minY);

        var dto = new Dto
        {
            Aspect = h / w,
            Strokes = strokes
                .Select(s => s.Select(p => new[] { (p.X - minX) / w, (p.Y - minY) / w }).ToList())
                .ToList(),
        };

        return JsonSerializer.Serialize(dto);
    }

    // Basahon balik ang JSON aron ma-drawing ang pirma sa sign-off page.
    public static SignaturePreviewDrawable? Parse(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return null;

        try
        {
            var dto = JsonSerializer.Deserialize<Dto>(json);
            if (dto == null)
                return null;

            var drawable = new SignaturePreviewDrawable { Aspect = dto.Aspect };
            foreach (var stroke in dto.Strokes)
                drawable.Strokes.Add(stroke.Where(a => a.Length >= 2).Select(a => new PointF(a[0], a[1])).ToList());
            return drawable;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
