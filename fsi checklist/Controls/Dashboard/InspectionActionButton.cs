// =====================================================================
// DASHBOARD: ang pundasyon (base) sa 3 ka button sa matag inspection card:
//   ViewResultButton      = "View result"           (tan-awon ang tubag)
//   EditEntriesButton     = "Edit entries"          (usbon ang tubag; pwede kanunay gawas kung locked)
//   LockInspectionButton  = "Lock this inspection"  (human niini dili na ma-edit)
// Ang matag usa naay kaugalingong file, kolor ug text (dili generic).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.Dashboard;

public abstract class InspectionActionButton : Button
{
    protected static Color Primary => FieldParts.Primary;

    protected InspectionActionButton(string text)
    {
        Text = text;
        FontSize = 13;
        HeightRequest = 36;
        CornerRadius = 18;
        Padding = new Thickness(14, 0);
        Margin = new Thickness(0, 0, 8, 6);
    }
}
