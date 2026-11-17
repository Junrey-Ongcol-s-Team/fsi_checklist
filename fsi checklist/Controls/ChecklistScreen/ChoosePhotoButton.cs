// =====================================================================
// CHECKLIST (FormPage): "Choose photo" button. Mopili og litrato gikan sa gallery / file.
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.ChecklistScreen;

public sealed class ChoosePhotoButton : Button
{
    public ChoosePhotoButton()
    {
        FieldParts.AppLook(this, typeof(Button));   // naandan nga hitsura sa app (apil ang abo kung disabled)
        Text = "Choose photo";
        FontSize = 13;
    }
}
