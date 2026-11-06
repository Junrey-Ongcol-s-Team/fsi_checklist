// =====================================================================
// FSIC RENEWAL (069): petsa
// Gamiton sa mga pangutana sa 069 sama sa:
//   - Previous After Inspection Report: FSIC dated
//   - Previous After Inspection Report: FSMR submitted dated
//   - Present/Actual Inspection: FSMR submitted dated
// Ang layout naa sa Common/FieldBases.cs (DateFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.FsicRenewal;

public sealed class FsicRenewalDateField : DateFieldBase
{
    public override string FormName => "FSIC Renewal (069)";
}
