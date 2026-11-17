// =====================================================================
// SETTINGS: "Reset database" button (pula). Papason ang tanang data - para ra sa debugging.
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.Settings;

public sealed class ResetDatabaseButton : Button
{
    public ResetDatabaseButton()
    {
        FieldParts.AppLook(this, typeof(Button));   // naandan nga hitsura sa app (apil ang abo kung disabled)
        Text = "Reset database";
        BackgroundColor = Color.FromArgb("#C62828");
        TextColor = Colors.White;
    }
}
