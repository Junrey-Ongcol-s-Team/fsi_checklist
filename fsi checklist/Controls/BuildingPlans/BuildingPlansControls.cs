// =====================================================================
// BUILDING PLANS (006): listahan sa kaugalingong controls niini nga form.
// Ang FormControls.cs mo-pangita diri kung unsang control ang gamiton.
// Bag-ong klase sa pangutana? I-add diri ang iyang control.
// =====================================================================
namespace fsi_checklist.Controls.BuildingPlans;

public static class BuildingPlansControls
{
    public static readonly IReadOnlyDictionary<string, Type> All = new Dictionary<string, Type>
    {
        ["Text"] = typeof(BuildingPlansTextField),
        ["LongText"] = typeof(BuildingPlansNotesField),
        ["Number"] = typeof(BuildingPlansNumberField),
        ["Date"] = typeof(BuildingPlansDateField),
        ["Bool"] = typeof(BuildingPlansCheckItem),
        ["YesNo"] = typeof(BuildingPlansYesNoButtons),
    };
}
