// =====================================================================
// NEW FSIC / OCCUPANCY (061): listahan sa kaugalingong controls niini nga form.
// Ang FormControls.cs mo-pangita diri kung unsang control ang gamiton.
// Bag-ong klase sa pangutana? I-add diri ang iyang control.
// =====================================================================
namespace fsi_checklist.Controls.FsicNewOccupancy;

public static class FsicNewOccupancyControls
{
    public static readonly IReadOnlyDictionary<string, Type> All = new Dictionary<string, Type>
    {
        ["Text"] = typeof(FsicNewOccupancyTextField),
        ["LongText"] = typeof(FsicNewOccupancyNotesField),
        ["Number"] = typeof(FsicNewOccupancyNumberField),
        ["Date"] = typeof(FsicNewOccupancyDateField),
        ["Bool"] = typeof(FsicNewOccupancyCheckItem),
        ["YesNo"] = typeof(FsicNewOccupancyYesNoButtons),
        ["Choice"] = typeof(FsicNewOccupancyChoiceButtons),
        ["Chips"] = typeof(FsicNewOccupancyClearanceChips),
    };
}
