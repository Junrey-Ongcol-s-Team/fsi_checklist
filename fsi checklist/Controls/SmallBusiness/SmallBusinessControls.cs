// =====================================================================
// SMALL / GENERAL BUSINESS (024): listahan sa kaugalingong controls niini nga form.
// Ang FormControls.cs mo-pangita diri kung unsang control ang gamiton.
// Bag-ong klase sa pangutana? I-add diri ang iyang control.
// =====================================================================
namespace fsi_checklist.Controls.SmallBusiness;

public static class SmallBusinessControls
{
    public static readonly IReadOnlyDictionary<string, Type> All = new Dictionary<string, Type>
    {
        ["Text"] = typeof(SmallBusinessTextField),
        ["LongText"] = typeof(SmallBusinessNotesField),
        ["Number"] = typeof(SmallBusinessNumberField),
        ["Date"] = typeof(SmallBusinessDateField),
        ["Bool"] = typeof(SmallBusinessCheckItem),
        ["YesNo"] = typeof(SmallBusinessYesNoButtons),
        ["Choice"] = typeof(SmallBusinessChoiceButtons),
    };
}
