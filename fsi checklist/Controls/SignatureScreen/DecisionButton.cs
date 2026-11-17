// =====================================================================
// SIGNATURE: ang pundasyon (base) sa duha ka button sa Fire Marshal:
//   ApprovedButton    = "Approved"
//   DisapprovedButton = "Disapproved"
// Ang napili mahimong puno og kolor (gikan sa Decision sa SignatureViewModel).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.SignatureScreen;

public abstract class DecisionButton : Button
{
    protected DecisionButton(string text, string value)
    {
        FieldParts.AppLook(this, typeof(Button));
        Text = text;
        CommandParameter = value;
        HeightRequest = 40;
        CornerRadius = 20;
        Padding = new Thickness(18, 0);
        BorderWidth = 1;
        BackgroundColor = Colors.Transparent;
        this.SetAppThemeColor(BorderColorProperty, Color.FromArgb("#BDBDBD"), Color.FromArgb("#5A5A5A"));
        this.SetAppThemeColor(TextColorProperty, Color.FromArgb("#424242"), Color.FromArgb("#E0E0E0"));

        // Kung kini ang napili (Decision == value): puno og kolor, puti ang letra
        var trigger = new DataTrigger(typeof(Button)) { Binding = new Binding("Decision"), Value = value };
        trigger.Setters.Add(new Setter { Property = BackgroundColorProperty, Value = FieldParts.Primary });
        trigger.Setters.Add(new Setter { Property = BorderColorProperty, Value = FieldParts.Primary });
        trigger.Setters.Add(new Setter { Property = TextColorProperty, Value = Colors.White });
        Triggers.Add(trigger);
    }
}
