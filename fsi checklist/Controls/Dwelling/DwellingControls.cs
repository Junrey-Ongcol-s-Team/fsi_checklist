// =====================================================================
// SINGLE AND TWO-FAMILY DWELLING (021): listahan sa kaugalingong controls niini nga form.
// Ang FormControls.cs mo-pangita diri kung unsang control ang gamiton.
// Bag-ong klase sa pangutana? I-add diri ang iyang control.
// =====================================================================
namespace fsi_checklist.Controls.Dwelling;

public static class DwellingControls
{
    public static readonly IReadOnlyDictionary<string, Type> All = new Dictionary<string, Type>
    {
        ["Text"] = typeof(DwellingTextField),
        ["LongText"] = typeof(DwellingNotesField),
        ["Date"] = typeof(DwellingDateField),
        ["Bool"] = typeof(DwellingCheckItem),
        ["YesNo"] = typeof(DwellingYesNoButtons),
    };
}
