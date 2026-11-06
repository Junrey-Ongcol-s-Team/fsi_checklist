// =====================================================================
// Mga 'selector': mopili kung unsang disenyo (DataTemplate) ang gamiton.
// Pananglitan: Yes/No -> chips, petsa -> DatePicker, text -> Entry.
// =====================================================================
using fsi_checklist.Controls;
using fsi_checklist.ViewModels;

namespace fsi_checklist.Views;

public sealed class FormRowTemplateSelector : DataTemplateSelector
{
    public DataTemplate? SectionTemplate { get; set; }
    public DataTemplate? ItemTemplate { get; set; }
    public DataTemplate? ExpanderTemplate { get; set; }   // numero nga naay sulod (UraniumUI ExpanderView)

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container) => item switch
    {
        SectionRowViewModel => SectionTemplate!,
        ItemRowViewModel { IsExpandable: true } when ExpanderTemplate != null => ExpanderTemplate,
        _ => ItemTemplate!,
    };
}

public sealed class SlotTemplateSelector : DataTemplateSelector
{
    public DataTemplate? TextTemplate { get; set; }
    public DataTemplate? LongTextTemplate { get; set; }
    public DataTemplate? NumberTemplate { get; set; }
    public DataTemplate? DateTemplate { get; set; }
    public DataTemplate? BoolTemplate { get; set; }
    public DataTemplate? ChoiceTemplate { get; set; }
    public DataTemplate? PhotoTemplate { get; set; }
    public DataTemplate? ChipsTemplate { get; set; }
    public DataTemplate? PickerTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        var slot = (SlotViewModel)item;

        // 1) KAUGALINGONG CONTROL SA FORM (Controls/<Form>/...), pananglitan FsicRenewalDateField
        var own = FormControls.TemplateFor(slot);
        if (own != null)
            return own;

        // 2) Kung wala: ang daan nga disenyo sa FormPage.xaml (backup)
        var template = slot.Kind switch
        {
            SlotKind.LongText => LongTextTemplate,
            SlotKind.Number => NumberTemplate,
            SlotKind.Date => DateTemplate,
            SlotKind.Bool => BoolTemplate,
            SlotKind.Choice => ChoiceTemplate,
            SlotKind.Photo => PhotoTemplate,
            SlotKind.Chips => ChipsTemplate,
            SlotKind.Picker => PickerTemplate,
            _ => TextTemplate,
        };
        return template!;
    }
}
