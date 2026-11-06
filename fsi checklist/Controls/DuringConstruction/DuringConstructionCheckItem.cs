// =====================================================================
// DURING CONSTRUCTION (068): checkbox
// Gamiton sa mga pangutana sa 068 sama sa:
//   - Doors
//   - Stairs
//   - Smoke-proof enclosures
// Ang layout naa sa Common/FieldBases.cs (CheckFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.DuringConstruction;

public sealed class DuringConstructionCheckItem : CheckFieldBase
{
    public override string FormName => "During Construction (068)";
}
