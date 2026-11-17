// =====================================================================
// SETTINGS: kahon nga basa ra, nagpakita kung asa ang database file.
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.Settings;

public sealed class DatabasePathBox : Entry
{
    public DatabasePathBox()
    {
        FieldParts.AppLook(this, typeof(Entry));   // naandan nga hitsura sa app
        IsReadOnly = true;
    }
}
