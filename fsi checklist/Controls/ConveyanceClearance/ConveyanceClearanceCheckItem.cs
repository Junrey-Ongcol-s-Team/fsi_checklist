// =====================================================================
// CONVEYANCE CLEARANCE (051): checkbox
// Gamiton sa mga pangutana sa 051 sama sa:
//   - Recommend issuance of FSC for Conveyance
// Ang layout naa sa Common/FieldBases.cs (CheckFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.ConveyanceClearance;

public sealed class ConveyanceClearanceCheckItem : CheckFieldBase
{
    public override string FormName => "Conveyance Clearance (051)";
}
