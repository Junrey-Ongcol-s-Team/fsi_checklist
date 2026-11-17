// =====================================================================
// SIGN-OFF: "Complete inspection" button (puno nga kolor). Mahuman ang inspection kung kompleto na ang pirma.
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.SignoffScreen;

public sealed class CompleteInspectionButton : Button
{
    public CompleteInspectionButton()
    {
        FieldParts.AppLook(this, typeof(Button));   // naandan nga hitsura sa app (apil ang abo kung disabled)
        Text = "Complete inspection";
        HeightRequest = 52;
        CornerRadius = 12;
        FontAttributes = FontAttributes.Bold;
    }
}
