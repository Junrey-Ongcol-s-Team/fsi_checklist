// =====================================================================
// Mga 'selector': mopili kung unsang disenyo (DataTemplate) ang gamiton.
// Pananglitan: Yes/No -> chips, petsa -> DatePicker, text -> Entry.
// =====================================================================
using fsi_checklist.ViewModels;

namespace fsi_checklist.Views;

public sealed class FormRowTemplateSelector : DataTemplateSelector
{
    public DataTemplate? SectionTemplate { get; set; }
    public DataTemplate? ItemTemplate { get; set; }

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container) =>
        (item is SectionRowViewModel ? SectionTemplate : ItemTemplate)!;
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

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        var template = ((SlotViewModel)item).Kind switch
        {
            SlotKind.LongText => LongTextTemplate,
            SlotKind.Number => NumberTemplate,
            SlotKind.Date => DateTemplate,
            SlotKind.Bool => BoolTemplate,
            SlotKind.Choice => ChoiceTemplate,
            SlotKind.Photo => PhotoTemplate,
            _ => TextTemplate,
        };
        return template!;
    }
}
