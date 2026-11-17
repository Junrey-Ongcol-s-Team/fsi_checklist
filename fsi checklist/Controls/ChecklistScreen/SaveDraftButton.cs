// =====================================================================
// CHECKLIST (FormPage): "Save draft" button sa ubos. I-save ug mobalik sa dashboard.
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.ChecklistScreen;

public sealed class SaveDraftButton : Button
{
    public SaveDraftButton()
    {
        FieldParts.AppLook(this, typeof(Button));   // naandan nga hitsura sa app (apil ang abo kung disabled)
        Text = "Save draft";
        HeightRequest = 48;
        CornerRadius = 12;
        FontSize = 13;
        BorderWidth = 1;
        BackgroundColor = Colors.Transparent;
        this.SetAppThemeColor(BorderColorProperty, Color.FromArgb("#C9BFBC"), Color.FromArgb("#5A5A5A"));
        this.SetAppThemeColor(TextColorProperty, Color.FromArgb("#231918"), Color.FromArgb("#E0E0E0"));
    }
}
