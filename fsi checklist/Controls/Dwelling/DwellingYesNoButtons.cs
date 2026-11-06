// =====================================================================
// SINGLE AND TWO-FAMILY DWELLING (021): Yes (berde) / No (pula) / N/A (abo)
// Gamiton sa mga pangutana sa 021 sama sa:
//   - Readily accessible?
//   - Obstructed?
//   - Travel distance within limits?
// Ang layout naa sa Common/FieldBases.cs (YesNoFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.Dwelling;

public sealed class DwellingYesNoButtons : YesNoFieldBase
{
    public override string FormName => "Single and Two-Family Dwelling (021)";
}
