// =====================================================================
// CONVEYANCE CLEARANCE THRU PIPELINES (075): mga chip, usa ra ang mapili
// Gamiton sa mga pangutana sa 075 sama sa:
//   - Recommendation (choose one): For issuance of Conveyance Clearance / Notice of Disapproval
// Ang layout naa sa Common/FieldBases.cs (ChoiceFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.ConveyancePipeline;

public sealed class ConveyancePipelineChoiceButtons : ChoiceFieldBase
{
    public override string FormName => "Conveyance Clearance thru Pipelines (075)";
}
