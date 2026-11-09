// =====================================================================
// NEW FSIC / OCCUPANCY (061): dropdown sa Residential sub classification
// Mogawas ra kung RESIDENTIAL ang Occupancy classification. Pilian:
//   - Single and Two Family Dwellings
//   - Hotels
//   - Dormitories
//   - Apartments
//   - Lodging or rooming
// Ang pilian naa sa seeds/061m.sql. Ang layout naa sa Common/FieldBases.cs (PickerFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.FsicNewOccupancy;

public sealed class FsicNewOccupancyResidentialPicker : PickerFieldBase
{
    public override string FormName => "New FSIC / Occupancy (061)";

    // Title sa dropdown
    protected override string PickerTitle => "Select residential sub classification";
}
