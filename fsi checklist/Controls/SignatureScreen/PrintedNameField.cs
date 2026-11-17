// =====================================================================
// SIGNATURE: kahon para sa printed name sa mo-pirma.
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.SignatureScreen;

public sealed class PrintedNameField : Entry
{
    public PrintedNameField()
    {
        FieldParts.AppLook(this, typeof(Entry));   // naandan nga hitsura sa app
        Placeholder = "Full name";
    }
}
