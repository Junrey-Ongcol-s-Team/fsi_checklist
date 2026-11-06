// =====================================================================
// NEW FSIC / OCCUPANCY (061): numero + unit
// Gamiton sa mga pangutana sa 061 sama sa:
//   - Capacity (no. of persons)
//   - Maximum Occupant Load
//   - Number of Stories
// Ang layout naa sa Common/FieldBases.cs (NumberFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.FsicNewOccupancy;

public sealed class FsicNewOccupancyNumberField : NumberFieldBase
{
    public override string FormName => "New FSIC / Occupancy (061)";
}
