// =====================================================================
// BUILDING PLANS (006): petsa
// Gamiton sa mga pangutana sa 006 sama sa:
//   - Plans checked: date
//   - Date Received
//   - Date Released
// Ang layout naa sa Common/FieldBases.cs (DateFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.BuildingPlans;

public sealed class BuildingPlansDateField : DateFieldBase
{
    public override string FormName => "Building Plans (006)";
}
