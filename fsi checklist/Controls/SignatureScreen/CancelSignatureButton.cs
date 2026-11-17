// =====================================================================
// SIGNATURE: "Cancel" button. Mobalik nga walay gi-save.
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.SignatureScreen;

public sealed class CancelSignatureButton : Button
{
    public CancelSignatureButton()
    {
        FieldParts.AppLook(this, typeof(Button));   // naandan nga hitsura sa app (apil ang abo kung disabled)
        Text = "Cancel";
    }
}
