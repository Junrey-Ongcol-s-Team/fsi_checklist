// =====================================================================
// LPG MOTORCYCLE CONVEYANCE (064): checkbox
// Gamiton sa mga pangutana sa 064 sama sa:
//   - For issuance of Conveyance Clearance
//   - Notice of Disapproval
// Ang layout naa sa Common/FieldBases.cs (CheckFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.LpgMotorcycle;

public sealed class LpgMotorcycleCheckItem : CheckFieldBase
{
    public override string FormName => "LPG Motorcycle Conveyance (064)";
}
