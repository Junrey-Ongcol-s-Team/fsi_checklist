// =====================================================================
// NEW INSPECTION: pilianan kung unsang checklist (form) ang gamiton.
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.NewInspectionScreen;

public sealed class ChecklistPicker : Picker
{
    public ChecklistPicker()
    {
        FieldParts.AppLook(this, typeof(Picker));   // naandan nga hitsura sa app
        Title = "Choose a checklist";
    }
}
