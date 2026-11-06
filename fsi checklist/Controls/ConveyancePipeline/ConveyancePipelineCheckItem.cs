// =====================================================================
// CONVEYANCE CLEARANCE THRU PIPELINES (075): checkbox
// Gamiton sa mga pangutana sa 075 sama sa:
//   - For issuance of Conveyance Clearance
//   - Notice of Disapproval
// Ang layout naa sa Common/FieldBases.cs (CheckFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.ConveyancePipeline;

public sealed class ConveyancePipelineCheckItem : CheckFieldBase
{
    public override string FormName => "Conveyance Clearance thru Pipelines (075)";
}
