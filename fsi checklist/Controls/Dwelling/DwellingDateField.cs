// =====================================================================
// SINGLE AND TWO-FAMILY DWELLING (021): petsa
// Gamiton sa mga pangutana sa 021 sama sa:
//   - Building permit: date issued
//   - Fire code fee: date issued
//   - Inspection order: date issued
// Ang layout naa sa Common/FieldBases.cs (DateFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.Dwelling;

public sealed class DwellingDateField : DateFieldBase
{
    public override string FormName => "Single and Two-Family Dwelling (021)";
}
