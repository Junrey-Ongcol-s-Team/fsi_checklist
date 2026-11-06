// =====================================================================
// DURING CONSTRUCTION (068): mga chip, usa ra ang mapili
// Gamiton sa mga pangutana sa 068 sama sa:
//   - Stage of inspection
//   - Ceiling finish
//   - Walls finish
// Ang layout naa sa Common/FieldBases.cs (ChoiceFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.DuringConstruction;

public sealed class DuringConstructionChoiceButtons : ChoiceFieldBase
{
    public override string FormName => "During Construction (068)";
}
