// =====================================================================
// GASOLINE SERVICE STATION (026): checkbox
// Gamiton sa mga pangutana sa 026 sama sa:
//   - Application for Business Permit
//   - Application for Occupancy Permit
//   - Building under construction
// Ang layout naa sa Common/FieldBases.cs (CheckFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.GasolineStation;

public sealed class GasolineStationCheckItem : CheckFieldBase
{
    public override string FormName => "Gasoline Service Station (026)";
}
