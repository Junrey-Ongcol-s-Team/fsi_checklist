// =====================================================================
// SIGN-OFF: "View results" button. Ang tibuok checklist nga basa ra.
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.SignoffScreen;

public sealed class ViewResultsButton : Button
{
    public ViewResultsButton()
    {
        FieldParts.AppLook(this, typeof(Button));   // naandan nga hitsura sa app (apil ang abo kung disabled)
        Text = "View results";
    }
}
