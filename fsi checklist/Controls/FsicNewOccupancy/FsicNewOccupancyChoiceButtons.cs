// =====================================================================
// NEW FSIC / OCCUPANCY (061): mga chip, usa ra ang mapili
// Gamiton sa mga pangutana sa 061 sama sa:
//   - Horizontal components: Anteroom
//   - Horizontal components: Common Path of Travel
//   - Horizontal components: Corridors
// Ang layout naa sa Common/FieldBases.cs (ChoiceFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.FsicNewOccupancy;

public sealed class FsicNewOccupancyChoiceButtons : ChoiceFieldBase
{
    public override string FormName => "New FSIC / Occupancy (061)";
}
