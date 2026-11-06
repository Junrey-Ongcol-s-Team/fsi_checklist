// =====================================================================
// NEW FSIC / OCCUPANCY (061): Yes (berde) / No (pula) / N/A (abo)
// Gamiton sa mga pangutana sa 061 sama sa:
//   - High Rise
//   - Within MAQ
//   - Fire Safety Compliance and Commissioning Report (FSCCR)
// Ang layout naa sa Common/FieldBases.cs (YesNoFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.FsicNewOccupancy;

public sealed class FsicNewOccupancyYesNoButtons : YesNoFieldBase
{
    public override string FormName => "New FSIC / Occupancy (061)";
}
