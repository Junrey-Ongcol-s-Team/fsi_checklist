// =====================================================================
// DURING CONSTRUCTION (068): listahan sa kaugalingong controls niini nga form.
// Ang FormControls.cs mo-pangita diri kung unsang control ang gamiton.
// Bag-ong klase sa pangutana? I-add diri ang iyang control.
// =====================================================================
namespace fsi_checklist.Controls.DuringConstruction;

public static class DuringConstructionControls
{
    public static readonly IReadOnlyDictionary<string, Type> All = new Dictionary<string, Type>
    {
        ["Text"] = typeof(DuringConstructionTextField),
        ["LongText"] = typeof(DuringConstructionNotesField),
        ["Number"] = typeof(DuringConstructionNumberField),
        ["Date"] = typeof(DuringConstructionDateField),
        ["Bool"] = typeof(DuringConstructionCheckItem),
        ["YesNo"] = typeof(DuringConstructionYesNoButtons),
        ["Choice"] = typeof(DuringConstructionChoiceButtons),
    };
}
