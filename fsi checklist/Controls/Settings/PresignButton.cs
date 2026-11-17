// =====================================================================
// SETTINGS: button sa matag saved signature (BFP). Ang text gikan sa ButtonText
// (pananglitan "Sign" o "Sign again").
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.Settings;

public sealed class PresignButton : Button
{
    public PresignButton()
    {
        FieldParts.AppLook(this, typeof(Button));   // naandan nga hitsura sa app (apil ang abo kung disabled)
        HeightRequest = 44;
        this.SetBinding(TextProperty, "ButtonText");
    }
}
