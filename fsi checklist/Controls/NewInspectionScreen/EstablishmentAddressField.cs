// =====================================================================
// NEW INSPECTION: kahon para sa address.
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.NewInspectionScreen;

public sealed class EstablishmentAddressField : Entry
{
    public EstablishmentAddressField()
    {
        FieldParts.AppLook(this, typeof(Entry));   // naandan nga hitsura sa app
    }
}
