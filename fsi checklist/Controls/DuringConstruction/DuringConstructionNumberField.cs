// =====================================================================
// DURING CONSTRUCTION (068): numero + unit
// Gamiton sa mga pangutana sa 068 sama sa:
//   - Each room with occupant load of 50 or more, or with hazard content
//   - Other % completion
//   - With sprinkler
// Ang layout naa sa Common/FieldBases.cs (NumberFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.DuringConstruction;

public sealed class DuringConstructionNumberField : NumberFieldBase
{
    public override string FormName => "During Construction (068)";
}
