// =====================================================================
// SectionCard (code-behind): ang Title ug ang sulod (Body) sa card.
// Ang sulod nga imong gisulat sulod sa <controls:SectionCard> mapunta sa Body.
// =====================================================================
namespace fsi_checklist.Controls;

[ContentProperty(nameof(Body))]
public partial class SectionCard : ContentView
{
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(
        nameof(Title), typeof(string), typeof(SectionCard), string.Empty,
        propertyChanged: (bindable, _, newValue) =>
        {
            var card = (SectionCard)bindable;
            var text = newValue as string ?? string.Empty;
            card.TitleLabel.Text = text;
            card.TitleLabel.IsVisible = text.Length > 0;   // walay titulo = walay label
        });

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public View? Body
    {
        get => Host.Content;
        set => Host.Content = value;
    }

    public SectionCard()
    {
        InitializeComponent();
    }
}
