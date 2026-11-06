// =====================================================================
// FSIC RENEWAL (069): Fire Safety Clearances: chips + search
// Gamiton sa mga pangutana sa 069 sama sa:
//   - Secure the following Fire Safety Clearance/s
// Ang layout naa sa Common/FieldBases.cs (ChipsFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.FsicRenewal;

public sealed class FsicRenewalClearanceChips : ChipsFieldBase
{
    public override string FormName => "FSIC Renewal (069)";

    // Placeholder sa search box
    protected override string SearchPlaceholder => "Search or type a clearance";
}
