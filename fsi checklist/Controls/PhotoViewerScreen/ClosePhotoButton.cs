// =====================================================================
// PHOTO VIEWER: "Close" button (dako para sayon i-tap). Mobalik sa page nga gigikanan.
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.PhotoViewerScreen;

public sealed class ClosePhotoButton : Button
{
    public ClosePhotoButton()
    {
        FieldParts.AppLook(this, typeof(Button));   // naandan nga hitsura sa app (apil ang abo kung disabled)
        Text = "Close";
        HeightRequest = 48;
        Margin = new Thickness(16);
    }
}
