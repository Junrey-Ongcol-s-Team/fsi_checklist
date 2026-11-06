// =====================================================================
// LPG MOTORCYCLE CONVEYANCE (064): Yes (berde) / No (pula) / N/A (abo)
// Gamiton sa mga pangutana sa 064 sama sa:
//   - LTO Certificate of Registration and updated LTO OR of vehicle availabl...
//   - Business Permit and FSIC (for renewal) for Business (Operator)
//   - Copy of the latest License to Operate issued by the DOE
// Ang layout naa sa Common/FieldBases.cs (YesNoFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.LpgMotorcycle;

public sealed class LpgMotorcycleYesNoButtons : YesNoFieldBase
{
    public override string FormName => "LPG Motorcycle Conveyance (064)";
}
