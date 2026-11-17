// =====================================================================
// SETTINGS: kahon nga basa ra, nagpakita kung asa ang error log (ablihan sa Notepad).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.Settings;

public sealed class ErrorLogPathBox : Entry
{
    public ErrorLogPathBox()
    {
        FieldParts.AppLook(this, typeof(Entry));   // naandan nga hitsura sa app
        IsReadOnly = true;
    }
}
