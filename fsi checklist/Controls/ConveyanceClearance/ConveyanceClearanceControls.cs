// =====================================================================
// CONVEYANCE CLEARANCE (051): listahan sa kaugalingong controls niini nga form.
// Ang FormControls.cs mo-pangita diri kung unsang control ang gamiton.
// Bag-ong klase sa pangutana? I-add diri ang iyang control.
// =====================================================================
namespace fsi_checklist.Controls.ConveyanceClearance;

public static class ConveyanceClearanceControls
{
    public static readonly IReadOnlyDictionary<string, Type> All = new Dictionary<string, Type>
    {
        ["Text"] = typeof(ConveyanceClearanceTextField),
        ["LongText"] = typeof(ConveyanceClearanceNotesField),
        ["Date"] = typeof(ConveyanceClearanceDateField),
        ["Bool"] = typeof(ConveyanceClearanceCheckItem),
        ["YesNo"] = typeof(ConveyanceClearanceYesNoButtons),
    };
}
