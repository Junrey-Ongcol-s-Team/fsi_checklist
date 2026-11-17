// =====================================================================
// SIGN-OFF: "Review / edit the form" button. Mobalik sa checklist aron usbon ang tubag.
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.SignoffScreen;

public sealed class ReviewFormButton : Button
{
    public ReviewFormButton()
    {
        FieldParts.AppLook(this, typeof(Button));   // naandan nga hitsura sa app (apil ang abo kung disabled)
        Text = "Review / edit the form";
    }
}
