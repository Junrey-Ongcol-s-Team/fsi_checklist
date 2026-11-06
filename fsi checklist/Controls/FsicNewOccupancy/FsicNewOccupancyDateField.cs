// =====================================================================
// NEW FSIC / OCCUPANCY (061): petsa
// Gamiton sa mga pangutana sa 061 sama sa:
//   - Building Permit: date issued
//   - Business Permit No.: date issued
//   - Certificate of Fire Drill: date issued
// Ang layout naa sa Common/FieldBases.cs (DateFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.FsicNewOccupancy;

public sealed class FsicNewOccupancyDateField : DateFieldBase
{
    public override string FormName => "New FSIC / Occupancy (061)";
}
