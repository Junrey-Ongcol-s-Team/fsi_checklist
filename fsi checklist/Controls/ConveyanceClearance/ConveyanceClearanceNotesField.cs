// =====================================================================
// CONVEYANCE CLEARANCE (051): taas nga sulatanan
// Gamiton sa mga pangutana sa 051 sama sa:
//   - Defects / deficiencies noted during inspection
//   - Recommendations
// Ang layout naa sa Common/FieldBases.cs (LongTextFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.ConveyanceClearance;

public sealed class ConveyanceClearanceNotesField : LongTextFieldBase
{
    public override string FormName => "Conveyance Clearance (051)";
}
