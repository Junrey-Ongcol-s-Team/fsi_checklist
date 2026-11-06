// =====================================================================
// FSIC RENEWAL (069): kahon nga sulatan
// Gamiton sa mga pangutana sa 069 sama sa:
//   - Previous After Inspection Report: FSIC No. (latest)
//   - Present/Actual Inspection: State particular floor
//   - Please specify
// Ang layout naa sa Common/FieldBases.cs (TextFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.FsicRenewal;

public sealed class FsicRenewalTextField : TextFieldBase
{
    public override string FormName => "FSIC Renewal (069)";
}
