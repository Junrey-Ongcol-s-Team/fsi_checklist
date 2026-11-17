// =====================================================================
// INSPECTION DETAILS: DATE INSPECTED (petsa sa inspection), naay ulohan.
//   <InspectedDateField Date="{Binding DateInspected}" />
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.InspectionDetails;

public sealed class InspectedDateField : ContentView
{
    public static readonly BindableProperty DateProperty = BindableProperty.Create(
        nameof(Date), typeof(DateTime), typeof(InspectedDateField), DateTime.Today, BindingMode.TwoWay);

    public DateTime Date { get => (DateTime)GetValue(DateProperty); set => SetValue(DateProperty, value); }

    public InspectedDateField()
    {
        var title = new Label
        {
            Text = "Date inspected",
            FontSize = 12,
            FontAttributes = FontAttributes.Bold,
            TextColor = FieldParts.Primary,
        };

        var picker = new DatePicker { HorizontalOptions = LayoutOptions.Start };
        picker.SetBinding(DatePicker.DateProperty, new Binding(nameof(Date), BindingMode.TwoWay, source: this));

        Content = new VerticalStackLayout { Spacing = 2, Children = { title, picker } };
    }
}
