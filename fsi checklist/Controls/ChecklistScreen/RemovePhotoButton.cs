// =====================================================================
// CHECKLIST (FormPage): "Remove" button sa ubos sa gamay nga litrato. Tangtangon ang litrato.
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.ChecklistScreen;

public sealed class RemovePhotoButton : Button
{
    public RemovePhotoButton()
    {
        FieldParts.AppLook(this, typeof(Button));   // naandan nga hitsura sa app (apil ang abo kung disabled)
        Text = "Remove";
        FontSize = 12;
        HeightRequest = 32;
        Padding = new Thickness(8, 0);
    }
}
