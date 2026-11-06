// =====================================================================
// FSIC RENEWAL (069): dropdown sa Type of occupancy
// Gamiton sa mga pangutana sa 069 sama sa:
//   - 3. Type of occupancy (previous)
// Ang layout naa sa Common/FieldBases.cs (PickerFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.FsicRenewal;

public sealed class FsicRenewalOccupancyPicker : PickerFieldBase
{
    public override string FormName => "FSIC Renewal (069)";

    // Title sa dropdown
    protected override string PickerTitle => "Select type of occupancy";
}
