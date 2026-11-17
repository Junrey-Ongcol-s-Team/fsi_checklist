// =====================================================================
// SETTINGS: "Save" button. I-save ang impormasyon sa station (letterhead).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.Settings;

public sealed class SaveSettingsButton : Button
{
    public SaveSettingsButton()
    {
        FieldParts.AppLook(this, typeof(Button));   // naandan nga hitsura sa app (apil ang abo kung disabled)
        Text = "Save";
        HeightRequest = 52;
        CornerRadius = 12;
        Margin = new Thickness(0, 12, 0, 0);
    }
}
