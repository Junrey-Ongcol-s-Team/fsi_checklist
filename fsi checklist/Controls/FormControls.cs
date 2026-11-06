// =====================================================================
// KAUGALINGONG CONTROLS SA MATAG FORM
//
// Ang matag form naay kaugalingong folder sa Controls:
//   Controls/FsicRenewal/      = 069  FSIC Renewal
//   Controls/FsicNewOccupancy/ = 061  New FSIC / Occupancy
//   ... (tan-awa ang listahan sa ubos)
// Sulod sa folder: ang mga control niana nga form (pananglitan FsicRenewalDateField)
// ug ang listahan (pananglitan FsicRenewalControls.cs).
//
// Unsaon pagpili: ang ItemId sa slot nagsugod sa id sa form
// (pananglitan "fsic-renewal.a03.prev_occupancy"), dayon ang klase sa tubag
// (Text, Date, YesNo ...) ang mo-ingon kung unsang control.
// Kung wala makit-an: ang daan nga disenyo sa FormPage.xaml ang gamiton (backup).
// =====================================================================
using fsi_checklist.ViewModels;

namespace fsi_checklist.Controls;

public static class FormControls
{
    // id sa form (form_template.id) -> listahan sa iyang controls
    private static readonly Dictionary<string, IReadOnlyDictionary<string, Type>> Forms = new()
    {
        ["fsic-renewal"] = FsicRenewal.FsicRenewalControls.All,             // 069 FSIC Renewal
        ["fsic-new-occupancy"] = FsicNewOccupancy.FsicNewOccupancyControls.All,// 061 New FSIC / Occupancy
        ["conveyance-clearance"] = ConveyanceClearance.ConveyanceClearanceControls.All,// 051 Conveyance Clearance
        ["during-construction"] = DuringConstruction.DuringConstructionControls.All,// 068 During Construction
        ["lpg-motorcycle"] = LpgMotorcycle.LpgMotorcycleControls.All,       // 064 LPG Motorcycle Conveyance
        ["conveyance-pipeline"] = ConveyancePipeline.ConveyancePipelineControls.All,// 075 Conveyance Clearance thru Pipelines
        ["small-business"] = SmallBusiness.SmallBusinessControls.All,       // 024 Small / General Business
        ["dwelling"] = Dwelling.DwellingControls.All,                       // 021 Single and Two-Family Dwelling
        ["gasoline-station"] = GasolineStation.GasolineStationControls.All, // 026 Gasoline Service Station
        ["building-plans"] = BuildingPlans.BuildingPlansControls.All,       // 006 Building Plans
    };

    // Usa ra ka DataTemplate matag control (dili na himoon pag-usab)
    private static readonly Dictionary<Type, DataTemplate> Cache = new();

    // Mga tubag nga Yes/No-type (berde/pula/abo nga buttons)
    private static readonly HashSet<string> YesNoValues = new() { "YES", "NO", "NA", "PASS", "FAIL" };

    /// <summary>Ang control sa form para niini nga slot, o null kung wala (gamiton ang daan).</summary>
    public static DataTemplate? TemplateFor(SlotViewModel slot)
    {
        int dot = slot.ItemId.IndexOf('.');
        if (dot <= 0 || !Forms.TryGetValue(slot.ItemId[..dot], out var controls))
            return null;

        string role = RoleOf(slot);
        if (!controls.TryGetValue(role, out var type) && role == "YesNo")
            controls.TryGetValue("Choice", out type);   // walay YesNo? gamiton ang Choice sa form
        if (type == null)
            return null;

        if (!Cache.TryGetValue(type, out var template))
        {
            template = new DataTemplate(type);
            Cache[type] = template;
        }
        return template;
    }

    /// <summary>Klase sa tubag: "Text", "Date", "YesNo", "Choice", "Chips", "Picker" ...</summary>
    public static string RoleOf(SlotViewModel slot) => slot.Kind switch
    {
        SlotKind.Bool => "Bool",
        SlotKind.Choice when slot.Options.Count > 0 && slot.Options.All(o => YesNoValues.Contains(o.Value)) => "YesNo",
        _ => slot.Kind.ToString(),
    };
}
