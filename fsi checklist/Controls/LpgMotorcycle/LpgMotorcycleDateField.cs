// =====================================================================
// LPG MOTORCYCLE CONVEYANCE (064): petsa
// Gamiton sa mga pangutana sa 064 sama sa:
//   - Date issued
//   - Validity of Registration
// Ang layout naa sa Common/FieldBases.cs (DateFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.LpgMotorcycle;

public sealed class LpgMotorcycleDateField : DateFieldBase
{
    public override string FormName => "LPG Motorcycle Conveyance (064)";
}
