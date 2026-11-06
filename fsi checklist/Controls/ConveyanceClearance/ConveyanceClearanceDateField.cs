// =====================================================================
// CONVEYANCE CLEARANCE (051): petsa
// Gamiton sa mga pangutana sa 051 sama sa:
//   - Date issued
//   - Validity of Registration
// Ang layout naa sa Common/FieldBases.cs (DateFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.ConveyanceClearance;

public sealed class ConveyanceClearanceDateField : DateFieldBase
{
    public override string FormName => "Conveyance Clearance (051)";
}
