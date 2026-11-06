// =====================================================================
// BUILDING PLANS (006): checkbox
// Gamiton sa mga pangutana sa 006 sama sa:
//   - Means of egress
//   - Exits sign
//   - Directional sign
// Ang layout naa sa Common/FieldBases.cs (CheckFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.BuildingPlans;

public sealed class BuildingPlansCheckItem : CheckFieldBase
{
    public override string FormName => "Building Plans (006)";
}
