// =====================================================================
// SINGLE AND TWO-FAMILY DWELLING (021): kahon nga sulatan
// Gamiton sa mga pangutana sa 021 sama sa:
//   - Building Permit
//   - Beams
//   - Ceiling
// Ang layout naa sa Common/FieldBases.cs (TextFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.Dwelling;

public sealed class DwellingTextField : TextFieldBase
{
    public override string FormName => "Single and Two-Family Dwelling (021)";
}
