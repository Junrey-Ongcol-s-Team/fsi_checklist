// =====================================================================
// NEW FSIC / OCCUPANCY (061): Fire Safety Clearances: chips + search
// Gamiton sa mga pangutana sa 061 sama sa:
//   - Secure the following Fire Safety Clearances
// Ang layout naa sa Common/FieldBases.cs (ChipsFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.FsicNewOccupancy;

public sealed class FsicNewOccupancyClearanceChips : ChipsFieldBase
{
    public override string FormName => "New FSIC / Occupancy (061)";

    // Placeholder sa search box
    protected override string SearchPlaceholder => "Search or type a clearance";
}
