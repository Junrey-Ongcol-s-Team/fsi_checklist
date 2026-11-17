// =====================================================================
// CHECKLIST (FormPage): "Take photo" button. Ablihan ang camera para sa litrato sa usa ka pangutana.
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.ChecklistScreen;

public sealed class TakePhotoButton : Button
{
    public TakePhotoButton()
    {
        FieldParts.AppLook(this, typeof(Button));   // naandan nga hitsura sa app (apil ang abo kung disabled)
        Text = "Take photo";
        FontSize = 13;
    }
}
