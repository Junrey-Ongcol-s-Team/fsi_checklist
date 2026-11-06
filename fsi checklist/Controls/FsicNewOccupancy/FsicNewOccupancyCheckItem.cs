// =====================================================================
// NEW FSIC / OCCUPANCY (061): checkbox
// Gamiton sa mga pangutana sa 061 sama sa:
//   - Exit access components: Anteroom
//   - Exit access components: Corridors
//   - Exit access components: Doors
// Ang layout naa sa Common/FieldBases.cs (CheckFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.FsicNewOccupancy;

public sealed class FsicNewOccupancyCheckItem : CheckFieldBase
{
    public override string FormName => "New FSIC / Occupancy (061)";
}
