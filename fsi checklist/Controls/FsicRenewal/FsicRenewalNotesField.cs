// =====================================================================
// FSIC RENEWAL (069): taas nga sulatanan
// Gamiton sa mga pangutana sa 069 sama sa:
//   - Defects / deficiencies (attach pictures, sketch and others)
//   - Fire Safety Clearance/s to secure (search or tap to add)
// Ang layout naa sa Common/FieldBases.cs (LongTextFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.FsicRenewal;

public sealed class FsicRenewalNotesField : LongTextFieldBase
{
    public override string FormName => "FSIC Renewal (069)";
}
