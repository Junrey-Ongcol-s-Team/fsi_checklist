// =====================================================================
// GASOLINE SERVICE STATION (026): listahan sa kaugalingong controls niini nga form.
// Ang FormControls.cs mo-pangita diri kung unsang control ang gamiton.
// Bag-ong klase sa pangutana? I-add diri ang iyang control.
// =====================================================================
namespace fsi_checklist.Controls.GasolineStation;

public static class GasolineStationControls
{
    public static readonly IReadOnlyDictionary<string, Type> All = new Dictionary<string, Type>
    {
        ["Text"] = typeof(GasolineStationTextField),
        ["LongText"] = typeof(GasolineStationNotesField),
        ["Number"] = typeof(GasolineStationNumberField),
        ["Date"] = typeof(GasolineStationDateField),
        ["Bool"] = typeof(GasolineStationCheckItem),
        ["YesNo"] = typeof(GasolineStationYesNoButtons),
        ["Choice"] = typeof(GasolineStationChoiceButtons),
    };
}
