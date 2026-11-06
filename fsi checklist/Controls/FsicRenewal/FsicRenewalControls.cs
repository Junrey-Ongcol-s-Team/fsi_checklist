// =====================================================================
// FSIC RENEWAL (069): listahan sa kaugalingong controls niini nga form.
// Ang FormControls.cs mo-pangita diri kung unsang control ang gamiton.
// Bag-ong klase sa pangutana? I-add diri ang iyang control.
// =====================================================================
namespace fsi_checklist.Controls.FsicRenewal;

public static class FsicRenewalControls
{
    public static readonly IReadOnlyDictionary<string, Type> All = new Dictionary<string, Type>
    {
        ["Text"] = typeof(FsicRenewalTextField),
        ["LongText"] = typeof(FsicRenewalNotesField),
        ["Number"] = typeof(FsicRenewalNumberField),
        ["Date"] = typeof(FsicRenewalDateField),
        ["Bool"] = typeof(FsicRenewalCheckItem),
        ["YesNo"] = typeof(FsicRenewalYesNoButtons),
        ["Choice"] = typeof(FsicRenewalChoiceButtons),
        ["Chips"] = typeof(FsicRenewalClearanceChips),
        ["Picker"] = typeof(FsicRenewalOccupancyPicker),
    };
}
