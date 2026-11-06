// =====================================================================
// CONVEYANCE CLEARANCE (051): Yes (berde) / No (pula) / N/A (abo)
// Gamiton sa mga pangutana sa 051 sama sa:
//   - LTO Certificate of Registration & Updated LTO Official Receipt (OR) of...
//   - Business Permit and Fire Safety Inspection Certificate for Business (O...
//   - Certification of the design, construction & testing of tank from manuf...
// Ang layout naa sa Common/FieldBases.cs (YesNoFieldBase).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.ConveyanceClearance;

public sealed class ConveyanceClearanceYesNoButtons : YesNoFieldBase
{
    public override string FormName => "Conveyance Clearance (051)";
}
