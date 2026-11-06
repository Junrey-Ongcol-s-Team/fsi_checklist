// =====================================================================
// CONVEYANCE CLEARANCE (051): kahon nga sulatan
// Gamiton sa mga pangutana sa 051 sama sa:
//   - Chassis No.
//   - Fire Safety Inspection Certificate No.
//   - Driver License No.
// Ang layout naa sa Common/FieldBases.cs (TextFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.ConveyanceClearance;

public sealed class ConveyanceClearanceTextField : TextFieldBase
{
    public override string FormName => "Conveyance Clearance (051)";
}
