// =====================================================================
// FSIC RENEWAL (069): checkbox
// Gamiton sa mga pangutana sa 069 sama sa:
//   - Present/Actual Inspection: Additional
//   - Floor interior finish
//   - Previous After Inspection Report: Compliant (Yes)
// Ang layout naa sa Common/FieldBases.cs (CheckFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.FsicRenewal;

public sealed class FsicRenewalCheckItem : CheckFieldBase
{
    public override string FormName => "FSIC Renewal (069)";
}
