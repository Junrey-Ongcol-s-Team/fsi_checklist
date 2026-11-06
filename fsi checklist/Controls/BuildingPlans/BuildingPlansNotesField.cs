// =====================================================================
// BUILDING PLANS (006): taas nga sulatanan
// Gamiton sa mga pangutana sa 006 sama sa:
//   - Notice of Disapproval: deficiencies noted in the submitted building pl...
//   - Identified fire safety requirements and additional requirements (item,...
// Ang layout naa sa Common/FieldBases.cs (LongTextFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.BuildingPlans;

public sealed class BuildingPlansNotesField : LongTextFieldBase
{
    public override string FormName => "Building Plans (006)";
}
