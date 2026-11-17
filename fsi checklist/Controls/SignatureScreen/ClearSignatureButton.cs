// =====================================================================
// SIGNATURE: "Clear" button. Papason ang gi-drawing sa kahon sa pirma.
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.SignatureScreen;

public sealed class ClearSignatureButton : Button
{
    public ClearSignatureButton()
    {
        FieldParts.AppLook(this, typeof(Button));   // naandan nga hitsura sa app (apil ang abo kung disabled)
        Text = "Clear";
    }
}
