// =====================================================================
// GASOLINE SERVICE STATION (026): Yes (berde) / No (pula) / N/A (abo)
// Gamiton sa mga pangutana sa 026 sama sa:
//   - Exit path properly illuminated?
//   - Operational
//   - Automatic Emergency Lights Provided?
// Ang layout naa sa Common/FieldBases.cs (YesNoFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.GasolineStation;

public sealed class GasolineStationYesNoButtons : YesNoFieldBase
{
    public override string FormName => "Gasoline Service Station (026)";
}
