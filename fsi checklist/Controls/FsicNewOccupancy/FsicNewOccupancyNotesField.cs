// =====================================================================
// NEW FSIC / OCCUPANCY (061): taas nga sulatanan
// Gamiton sa mga pangutana sa 061 sama sa:
//   - Item I
//   - Item II
//   - Item III
// Ang layout naa sa Common/FieldBases.cs (LongTextFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.FsicNewOccupancy;

public sealed class FsicNewOccupancyNotesField : LongTextFieldBase
{
    public override string FormName => "New FSIC / Occupancy (061)";
}
