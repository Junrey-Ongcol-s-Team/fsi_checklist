// =====================================================================
// SMALL / GENERAL BUSINESS (024): petsa
// Gamiton sa mga pangutana sa 024 sama sa:
//   - Business Permit No.: date issued
//   - FSEC No.: date issued
//   - FSIC No. (Latest): date issued
// Ang layout naa sa Common/FieldBases.cs (DateFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.SmallBusiness;

public sealed class SmallBusinessDateField : DateFieldBase
{
    public override string FormName => "Small / General Business (024)";
}
