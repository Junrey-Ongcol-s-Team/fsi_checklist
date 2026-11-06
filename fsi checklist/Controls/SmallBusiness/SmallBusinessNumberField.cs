// =====================================================================
// SMALL / GENERAL BUSINESS (024): numero + unit
// Gamiton sa mga pangutana sa 024 sama sa:
//   - Fire detection: units per floor
//   - Fire extinguisher quantity
//   - Occupant Load
// Ang layout naa sa Common/FieldBases.cs (NumberFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.SmallBusiness;

public sealed class SmallBusinessNumberField : NumberFieldBase
{
    public override string FormName => "Small / General Business (024)";
}
