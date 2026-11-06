// =====================================================================
// LPG MOTORCYCLE CONVEYANCE (064): listahan sa kaugalingong controls niini nga form.
// Ang FormControls.cs mo-pangita diri kung unsang control ang gamiton.
// Bag-ong klase sa pangutana? I-add diri ang iyang control.
// =====================================================================
namespace fsi_checklist.Controls.LpgMotorcycle;

public static class LpgMotorcycleControls
{
    public static readonly IReadOnlyDictionary<string, Type> All = new Dictionary<string, Type>
    {
        ["Text"] = typeof(LpgMotorcycleTextField),
        ["Date"] = typeof(LpgMotorcycleDateField),
        ["Bool"] = typeof(LpgMotorcycleCheckItem),
        ["YesNo"] = typeof(LpgMotorcycleYesNoButtons),
        ["Choice"] = typeof(LpgMotorcycleChoiceButtons),
        ["Photo"] = typeof(LpgMotorcyclePhotoNote),
    };
}
