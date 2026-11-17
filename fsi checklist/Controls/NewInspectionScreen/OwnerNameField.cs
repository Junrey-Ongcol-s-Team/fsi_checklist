// =====================================================================
// NEW INSPECTION: kahon para sa ngalan sa tag-iya / representative.
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.NewInspectionScreen;

public sealed class OwnerNameField : Entry
{
    public OwnerNameField()
    {
        FieldParts.AppLook(this, typeof(Entry));   // naandan nga hitsura sa app
    }
}
