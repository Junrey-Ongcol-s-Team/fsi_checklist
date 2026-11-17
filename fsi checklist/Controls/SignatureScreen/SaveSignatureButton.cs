// =====================================================================
// SIGNATURE: "Save signature" button. I-save ang pirma ug printed name.
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.SignatureScreen;

public sealed class SaveSignatureButton : Button
{
    public SaveSignatureButton()
    {
        FieldParts.AppLook(this, typeof(Button));   // naandan nga hitsura sa app (apil ang abo kung disabled)
        Text = "Save signature";
    }
}
