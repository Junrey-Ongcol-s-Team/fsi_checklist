// =====================================================================
// FSIC RENEWAL (069): numero + unit
// Gamiton sa mga pangutana sa 069 sama sa:
//   - Previous After Inspection Report: No. of floors
//   - Present/Actual Inspection: No. of fire extinguishers
// Ang layout naa sa Common/FieldBases.cs (NumberFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.FsicRenewal;

public sealed class FsicRenewalNumberField : NumberFieldBase
{
    public override string FormName => "FSIC Renewal (069)";
}
