// =====================================================================
// GASOLINE SERVICE STATION (026): taas nga sulatanan
// Gamiton sa mga pangutana sa 026 sama sa:
//   - Defects / deficiencies noted during inspection (attach pictures, sketc...
//   - Recommendations
// Ang layout naa sa Common/FieldBases.cs (LongTextFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.GasolineStation;

public sealed class GasolineStationNotesField : LongTextFieldBase
{
    public override string FormName => "Gasoline Service Station (026)";
}
