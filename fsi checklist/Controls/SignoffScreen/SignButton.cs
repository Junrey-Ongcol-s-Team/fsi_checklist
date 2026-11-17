// =====================================================================
// SIGN-OFF: "Sign" button sa matag mo-pirma (Owner, Inspector ...). Ablihan ang kahon sa pirma.
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.SignoffScreen;

public sealed class SignButton : Button
{
    public SignButton()
    {
        FieldParts.AppLook(this, typeof(Button));   // naandan nga hitsura sa app (apil ang abo kung disabled)
        Text = "Sign";
    }
}
