// =====================================================================
// CONVEYANCE CLEARANCE THRU PIPELINES (075): taas nga sulatanan
// Gamiton sa mga pangutana sa 075 sama sa:
//   - Defects / deficiencies noted during inspection
// Ang layout naa sa Common/FieldBases.cs (LongTextFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.ConveyancePipeline;

public sealed class ConveyancePipelineNotesField : LongTextFieldBase
{
    public override string FormName => "Conveyance Clearance thru Pipelines (075)";
}
