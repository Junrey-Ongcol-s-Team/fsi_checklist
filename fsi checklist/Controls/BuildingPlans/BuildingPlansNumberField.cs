// =====================================================================
// BUILDING PLANS (006): numero + unit
// Gamiton sa mga pangutana sa 006 sama sa:
//   - Doors: capacity (no. of persons)
//   - Stairs: capacity (no. of persons)
//   - Smoke-proof enclosures: capacity (no. of persons)
// Ang layout naa sa Common/FieldBases.cs (NumberFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.BuildingPlans;

public sealed class BuildingPlansNumberField : NumberFieldBase
{
    public override string FormName => "Building Plans (006)";
}
