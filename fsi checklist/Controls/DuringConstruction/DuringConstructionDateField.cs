// =====================================================================
// DURING CONSTRUCTION (068): petsa
// Gamiton sa mga pangutana sa 068 sama sa:
//   - FSEC: date issued
//   - Inspection order: date issued
// Ang layout naa sa Common/FieldBases.cs (DateFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.DuringConstruction;

public sealed class DuringConstructionDateField : DateFieldBase
{
    public override string FormName => "During Construction (068)";
}
