// =====================================================================
// SMALL / GENERAL BUSINESS (024): Yes (berde) / No (pula) / N/A (abo)
// Gamiton sa mga pangutana sa 024 sama sa:
//   - Do doors swing in direction of egress?
//   - Exits doors open and close properly?
//   - At least two (2) means of egress for each floor
// Ang layout naa sa Common/FieldBases.cs (YesNoFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.SmallBusiness;

public sealed class SmallBusinessYesNoButtons : YesNoFieldBase
{
    public override string FormName => "Small / General Business (024)";
}
