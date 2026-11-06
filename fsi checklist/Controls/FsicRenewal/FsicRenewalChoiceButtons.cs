// =====================================================================
// FSIC RENEWAL (069): mga chip, usa ra ang mapili
// Gamiton sa mga pangutana sa 069 sama sa:
//   - Present/Actual Inspection: No changes / With changes
//   - Previous After Inspection Report: Type of occupancy
//   - Nature of inspection conducted (choose one)
// Ang layout naa sa Common/FieldBases.cs (ChoiceFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.FsicRenewal;

public sealed class FsicRenewalChoiceButtons : ChoiceFieldBase
{
    public override string FormName => "FSIC Renewal (069)";
}
