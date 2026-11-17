// =====================================================================
// INSPECTION DETAILS (ang unang gi-fill up sa bag-ong inspection): usa ka kahon nga sulatan
// nga naay ulohan (Title) ug ehemplo (Example). Gamiton sa Edit details page:
//   Name of establishment, Address, Owner / representative, Inspection order no.
//   <DetailsTextField Title="Address" Text="{Binding Address}" />
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.InspectionDetails;

public sealed class DetailsTextField : ContentView
{
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title), typeof(string), typeof(DetailsTextField), string.Empty);

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text), typeof(string), typeof(DetailsTextField), string.Empty, BindingMode.TwoWay);

    public static readonly BindableProperty ExampleProperty = BindableProperty.Create(
        nameof(Example), typeof(string), typeof(DetailsTextField), string.Empty);

    public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
    public string Example { get => (string)GetValue(ExampleProperty); set => SetValue(ExampleProperty, value); }

    public DetailsTextField()
    {
        var title = new Label { FontSize = 12, FontAttributes = FontAttributes.Bold, TextColor = FieldParts.Primary };
        title.SetBinding(Label.TextProperty, new Binding(nameof(Title), source: this));

        var entry = new Entry();
        entry.SetBinding(Entry.TextProperty, new Binding(nameof(Text), BindingMode.TwoWay, source: this));
        entry.SetBinding(Entry.PlaceholderProperty, new Binding(nameof(Example), source: this));

        Content = new VerticalStackLayout { Spacing = 2, Children = { title, entry } };
    }
}
