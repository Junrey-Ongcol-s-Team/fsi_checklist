// =====================================================================
// NEW INSPECTION: kahon para sa contact number. Numero ra, 11 ka numero (09XXXXXXXXX).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.NewInspectionScreen;

public sealed class OwnerContactNumberField : Entry
{
    public OwnerContactNumberField()
    {
        FieldParts.AppLook(this, typeof(Entry));   // naandan nga hitsura sa app
        Keyboard = Keyboard.Telephone;
        Placeholder = "09XXXXXXXXX";
    }
}
