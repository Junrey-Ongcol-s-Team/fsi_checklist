// =====================================================================
// NEW INSPECTION: kahon para sa Inspection order / control no.
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.NewInspectionScreen;

public sealed class InspectionOrderNumberField : Entry
{
    public InspectionOrderNumberField()
    {
        FieldParts.AppLook(this, typeof(Entry));   // naandan nga hitsura sa app
    }
}
