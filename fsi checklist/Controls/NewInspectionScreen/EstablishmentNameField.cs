// =====================================================================
// NEW INSPECTION: kahon para sa ngalan sa establisemento / sakyanan (ang unang gi-fill up).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.NewInspectionScreen;

public sealed class EstablishmentNameField : Entry
{
    public EstablishmentNameField()
    {
        FieldParts.AppLook(this, typeof(Entry));   // naandan nga hitsura sa app
    }
}
