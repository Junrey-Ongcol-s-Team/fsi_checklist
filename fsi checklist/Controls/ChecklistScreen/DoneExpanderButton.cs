// =====================================================================
// CHECKLIST (FormPage): "Done" button sa ubos sa naabli nga expander.
// Sirad-an ang expander ug mobalik sa numero (dili na kinahanglan mo-scroll pataas).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.ChecklistScreen;

public sealed class DoneExpanderButton : Button
{
    public DoneExpanderButton()
    {
        FieldParts.AppLook(this, typeof(Button));   // naandan nga hitsura sa app (apil ang abo kung disabled)
        Text = "Done  ▲";
        HeightRequest = 48;
        CornerRadius = 12;
        FontSize = 13;
        BorderWidth = 1;
        BackgroundColor = Colors.Transparent;
        this.SetAppThemeColor(BorderColorProperty, Color.FromArgb("#C9BFBC"), Color.FromArgb("#5A5A5A"));
        this.SetAppThemeColor(TextColorProperty, Color.FromArgb("#231918"), Color.FromArgb("#E0E0E0"));
        WidthRequest = 120;
        HorizontalOptions = LayoutOptions.End;
    }
}
