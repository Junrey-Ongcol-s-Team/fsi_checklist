// =====================================================================
// SMALL / GENERAL BUSINESS (024): checkbox
// Gamiton sa mga pangutana sa 024 sama sa:
//   - Emergency lighting location: Exit Doors
//   - Emergency lighting location: Hallways
//   - Emergency lighting location: Stairway Landings
// Ang layout naa sa Common/FieldBases.cs (CheckFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.SmallBusiness;

public sealed class SmallBusinessCheckItem : CheckFieldBase
{
    public override string FormName => "Small / General Business (024)";
}
