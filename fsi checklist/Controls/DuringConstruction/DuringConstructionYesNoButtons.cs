// =====================================================================
// DURING CONSTRUCTION (068): Yes (berde) / No (pula) / N/A (abo)
// Gamiton sa mga pangutana sa 068 sama sa:
//   - Permissible exit components
//   - Capacity factor (means of egress)
//   - Occupant load factor
// Ang layout naa sa Common/FieldBases.cs (YesNoFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.DuringConstruction;

public sealed class DuringConstructionYesNoButtons : YesNoFieldBase
{
    public override string FormName => "During Construction (068)";
}
