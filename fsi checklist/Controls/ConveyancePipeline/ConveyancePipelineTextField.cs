// =====================================================================
// CONVEYANCE CLEARANCE THRU PIPELINES (075): kahon nga sulatan
// Gamiton sa mga pangutana sa 075 sama sa:
//   - Is piping labeled/tagged with equipment ID number?
//   - Is the pipeline adequately identified?
//   - Are labels/tags readable?
// Ang layout naa sa Common/FieldBases.cs (TextFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.ConveyancePipeline;

public sealed class ConveyancePipelineTextField : TextFieldBase
{
    public override string FormName => "Conveyance Clearance thru Pipelines (075)";
}
