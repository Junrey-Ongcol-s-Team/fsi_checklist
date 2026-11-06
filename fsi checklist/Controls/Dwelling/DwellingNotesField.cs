// =====================================================================
// SINGLE AND TWO-FAMILY DWELLING (021): taas nga sulatanan
// Gamiton sa mga pangutana sa 021 sama sa:
//   - Defects / deficiencies noted during inspection (attach pictures, sketc...
//   - Recommendations
//   - Specific usage of each floor, section or rooms
// Ang layout naa sa Common/FieldBases.cs (LongTextFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.Dwelling;

public sealed class DwellingNotesField : LongTextFieldBase
{
    public override string FormName => "Single and Two-Family Dwelling (021)";
}
