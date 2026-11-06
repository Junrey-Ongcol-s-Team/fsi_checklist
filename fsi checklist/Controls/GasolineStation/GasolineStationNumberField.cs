// =====================================================================
// GASOLINE SERVICE STATION (026): numero + unit
// Gamiton sa mga pangutana sa 026 sama sa:
//   - No. of Units per Floor
//   - No. of Units
//   - Number of Lubricating Bays
// Ang layout naa sa Common/FieldBases.cs (NumberFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.GasolineStation;

public sealed class GasolineStationNumberField : NumberFieldBase
{
    public override string FormName => "Gasoline Service Station (026)";
}
