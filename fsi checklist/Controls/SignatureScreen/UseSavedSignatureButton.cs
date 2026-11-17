// =====================================================================
// SIGNATURE: "Use saved signature" button. Gamiton ang pirma nga na-save sa Settings (dili na mag-drawing).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.SignatureScreen;

public sealed class UseSavedSignatureButton : Button
{
    public UseSavedSignatureButton()
    {
        FieldParts.AppLook(this, typeof(Button));   // naandan nga hitsura sa app (apil ang abo kung disabled)
        Text = "Use saved signature";
        HeightRequest = 48;
    }
}
