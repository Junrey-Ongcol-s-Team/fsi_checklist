// =====================================================================
// INSPECTION DETAILS: CONTACT NUMBER.
// Numero ra ang madawat (ang letra, space, "-" tangtangon dayon), hangtod 11 ra ka numero
// (pananglitan 09171234567). IsComplete = blangko o eksakto 11 ka numero.
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.InspectionDetails;

public sealed class ContactNumberField : ContentView
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(
        nameof(Text), typeof(string), typeof(ContactNumberField), string.Empty, BindingMode.TwoWay,
        propertyChanged: (bindable, _, value) => ((ContactNumberField)bindable).Clean((string?)value));

    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }

    public ContactNumberField()
    {
        var title = new Label
        {
            Text = "Contact number (11 digits)",
            FontSize = 12,
            FontAttributes = FontAttributes.Bold,
            TextColor = FieldParts.Primary,
        };

        var entry = new Entry { Keyboard = Keyboard.Telephone, Placeholder = "09XXXXXXXXX", MaxLength = 11 };
        entry.SetBinding(Entry.TextProperty, new Binding(nameof(Text), BindingMode.TwoWay, source: this));

        Content = new VerticalStackLayout { Spacing = 2, Children = { title, entry } };
    }

    // Tangtangon ang dili numero, ug putlon sa 11
    private void Clean(string? value)
    {
        string digits = new string((value ?? string.Empty).Where(c => c >= '0' && c <= '9').Take(11).ToArray());
        if (digits != (value ?? string.Empty))
            Text = digits;
    }
}
