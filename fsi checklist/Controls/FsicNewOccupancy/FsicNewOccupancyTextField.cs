// =====================================================================
// NEW FSIC / OCCUPANCY (061): kahon nga sulatan
// Gamiton sa mga pangutana sa 061 sama sa:
//   - Horizontal components: Anteroom
//   - Horizontal components: Common Path of Travel
//   - Horizontal components: Corridors
// Ang layout naa sa Common/FieldBases.cs (TextFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.FsicNewOccupancy;

public sealed class FsicNewOccupancyTextField : TextFieldBase
{
    public override string FormName => "New FSIC / Occupancy (061)";
}
