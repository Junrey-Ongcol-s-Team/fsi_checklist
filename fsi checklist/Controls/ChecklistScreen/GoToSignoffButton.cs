// =====================================================================
// CHECKLIST (FormPage): "Sign-off" button sa ubos (puno nga kolor). Moadto sa mga pirma.
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.ChecklistScreen;

public sealed class GoToSignoffButton : Button
{
    public GoToSignoffButton()
    {
        FieldParts.AppLook(this, typeof(Button));   // naandan nga hitsura sa app (apil ang abo kung disabled)
        Text = "Sign-off";
        HeightRequest = 48;
        CornerRadius = 12;
        FontSize = 13;
        FontAttributes = FontAttributes.Bold;
    }
}
