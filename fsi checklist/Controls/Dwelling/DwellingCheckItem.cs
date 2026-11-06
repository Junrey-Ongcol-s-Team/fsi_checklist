// =====================================================================
// SINGLE AND TWO-FAMILY DWELLING (021): checkbox
// Gamiton sa mga pangutana sa 021 sama sa:
//   - Application for Business Permit
//   - Application for Occupancy Permit
//   - Building under construction
// Ang layout naa sa Common/FieldBases.cs (CheckFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.Dwelling;

public sealed class DwellingCheckItem : CheckFieldBase
{
    public override string FormName => "Single and Two-Family Dwelling (021)";
}
