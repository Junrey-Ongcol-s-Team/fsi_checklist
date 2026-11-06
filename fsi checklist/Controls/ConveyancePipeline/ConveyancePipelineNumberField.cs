// =====================================================================
// CONVEYANCE CLEARANCE THRU PIPELINES (075): numero + unit
// Gamiton sa mga pangutana sa 075 sama sa:
//   - Year of Manufacturing of the Pipeline
//   - Diameter of the Pipeline
//   - Inlet Gas Pressure in the Pipeline
// Ang layout naa sa Common/FieldBases.cs (NumberFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.ConveyancePipeline;

public sealed class ConveyancePipelineNumberField : NumberFieldBase
{
    public override string FormName => "Conveyance Clearance thru Pipelines (075)";
}
