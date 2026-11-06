// =====================================================================
// FSIC RENEWAL (069): Yes (berde) / No (pula) / N/A (abo)
// Gamiton sa mga pangutana sa 069 sama sa:
//   - Present/Actual Inspection: Compliant (Yes / No)
//   - Present/Actual Inspection: FSMR submitted
//   - Fire Safety Maintenance Report (FSMR)
// Ang layout naa sa Common/FieldBases.cs (YesNoFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.FsicRenewal;

public sealed class FsicRenewalYesNoButtons : YesNoFieldBase
{
    public override string FormName => "FSIC Renewal (069)";
}
