// =====================================================================
// SINGLE AND TWO-FAMILY DWELLING (021): mga chip, usa ra ang mapili
// Gamiton sa mga pangutana sa 021 sama sa:
//   - Nature of inspection conducted (choose one)
// Ang layout naa sa Common/FieldBases.cs (ChoiceFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.Dwelling;

public sealed class DwellingChoiceButtons : ChoiceFieldBase
{
    public override string FormName => "Single and Two-Family Dwelling (021)";
}
