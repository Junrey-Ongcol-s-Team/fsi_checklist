// =====================================================================
// SETTINGS: usa ka kahon sa impormasyon sa station (para sa LETTERHEAD sa tanang printout):
//   Fire station name  ->  "Region 10 TANGUB CITY FIRE STATION"
//   Fire station address, Email, Hotline number
// Naay ulohan (Title), sulatanan (Text), ehemplo (Example) ug klase sa keyboard (Keyboard).
//   <settings:StationInfoField Title="Email" Example="tangubcfs_bfp10@yahoo.com" Keyboard="Email" Text="{Binding StationEmail}" />
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.Settings;

public sealed class StationInfoField : ContentView
{
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title), typeof(string), typeof(StationInfoField), string.Empty);

    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text), typeof(string), typeof(StationInfoField), string.Empty, BindingMode.TwoWay);

    public static readonly BindableProperty ExampleProperty = BindableProperty.Create(
        nameof(Example), typeof(string), typeof(StationInfoField), string.Empty);

    public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(
        nameof(Keyboard), typeof(Keyboard), typeof(StationInfoField), Keyboard.Default);

    public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
    public string Example { get => (string)GetValue(ExampleProperty); set => SetValue(ExampleProperty, value); }
    public Keyboard Keyboard { get => (Keyboard)GetValue(KeyboardProperty); set => SetValue(KeyboardProperty, value); }

    public StationInfoField()
    {
        var title = new Label { FontSize = 12, FontAttributes = FontAttributes.Bold, TextColor = FieldParts.Primary };
        title.SetBinding(Label.TextProperty, new Binding(nameof(Title), source: this));

        var entry = new Entry();
        entry.SetBinding(Entry.TextProperty, new Binding(nameof(Text), BindingMode.TwoWay, source: this));
        entry.SetBinding(Entry.PlaceholderProperty, new Binding(nameof(Example), source: this));
        entry.SetBinding(InputView.KeyboardProperty, new Binding(nameof(Keyboard), source: this));

        Content = new VerticalStackLayout { Spacing = 2, Children = { title, entry } };
    }
}
