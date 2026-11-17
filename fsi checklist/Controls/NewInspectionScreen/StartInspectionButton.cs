// =====================================================================
// NEW INSPECTION: "Start inspection" button (puno nga kolor). Himoon ang inspection ug ablihan ang checklist.
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.NewInspectionScreen;

public sealed class StartInspectionButton : Button
{
    public StartInspectionButton()
    {
        FieldParts.AppLook(this, typeof(Button));   // naandan nga hitsura sa app (apil ang abo kung disabled)
        Text = "Start inspection";
        HeightRequest = 52;
        CornerRadius = 12;
        FontAttributes = FontAttributes.Bold;
        Margin = new Thickness(0, 12, 0, 0);
    }
}
