// =====================================================================
// BUILDING PLANS (006): Yes (berde) / No (pula) / N/A (abo)
// Gamiton sa mga pangutana sa 006 sama sa:
//   - Permissible exit components
//   - Capacity factor (means of egress)
//   - Occupant load factor
// Ang layout naa sa Common/FieldBases.cs (YesNoFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.BuildingPlans;

public sealed class BuildingPlansYesNoButtons : YesNoFieldBase
{
    public override string FormName => "Building Plans (006)";
}
