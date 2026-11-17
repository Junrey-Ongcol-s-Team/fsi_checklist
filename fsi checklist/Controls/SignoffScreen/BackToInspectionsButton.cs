// =====================================================================
// SIGN-OFF: "Back to inspections" button. Mobalik sa dashboard.
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.SignoffScreen;

public sealed class BackToInspectionsButton : Button
{
    public BackToInspectionsButton()
    {
        FieldParts.AppLook(this, typeof(Button));   // naandan nga hitsura sa app (apil ang abo kung disabled)
        Text = "Back to inspections";
    }
}
