// =====================================================================
// GASOLINE SERVICE STATION (026): petsa
// Gamiton sa mga pangutana sa 026 sama sa:
//   - Building Permit: date issued
//   - Latest Mayor's/Bus. Permit: date issued
//   - Latest Certificate of Electrical Inspection: date issued
// Ang layout naa sa Common/FieldBases.cs (DateFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.GasolineStation;

public sealed class GasolineStationDateField : DateFieldBase
{
    public override string FormName => "Gasoline Service Station (026)";
}
