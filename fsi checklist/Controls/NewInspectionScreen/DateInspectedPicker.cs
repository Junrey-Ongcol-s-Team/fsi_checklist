// =====================================================================
// NEW INSPECTION: pilianan sa petsa sa inspection.
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.NewInspectionScreen;

public sealed class DateInspectedPicker : DatePicker
{
    public DateInspectedPicker()
    {
        FieldParts.AppLook(this, typeof(DatePicker));   // naandan nga hitsura sa app
    }
}
