// =====================================================================
// INSPECTION DETAILS: "Save and continue to entries" button (puno nga kolor).
// I-save ang detalye (ngalan, address, contact ...), dayon ablihan ang form aron usbon ang mga tubag.
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.InspectionDetails;

public sealed class ContinueToEntriesButton : Button
{
    public ContinueToEntriesButton()
    {
        Text = "Save and continue to entries  →";
        HeightRequest = 52;
        CornerRadius = 12;
        FontAttributes = FontAttributes.Bold;
        BackgroundColor = FieldParts.Primary;
        TextColor = Colors.White;
        Margin = new Thickness(0, 12, 0, 0);
    }
}
