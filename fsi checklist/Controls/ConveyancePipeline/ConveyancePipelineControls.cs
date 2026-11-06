// =====================================================================
// CONVEYANCE CLEARANCE THRU PIPELINES (075): listahan sa kaugalingong controls niini nga form.
// Ang FormControls.cs mo-pangita diri kung unsang control ang gamiton.
// Bag-ong klase sa pangutana? I-add diri ang iyang control.
// =====================================================================
namespace fsi_checklist.Controls.ConveyancePipeline;

public static class ConveyancePipelineControls
{
    public static readonly IReadOnlyDictionary<string, Type> All = new Dictionary<string, Type>
    {
        ["Text"] = typeof(ConveyancePipelineTextField),
        ["LongText"] = typeof(ConveyancePipelineNotesField),
        ["Number"] = typeof(ConveyancePipelineNumberField),
        ["Bool"] = typeof(ConveyancePipelineCheckItem),
        ["YesNo"] = typeof(ConveyancePipelineYesNoButtons),
    };
}
