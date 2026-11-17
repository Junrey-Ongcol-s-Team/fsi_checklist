// =====================================================================
// RESULTS: "Print form" button. Pun-on ang BFP form (PDF) ug ablihan para i-print.
// Mogana ra kung COMPLETED ug walay kulang.
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.ResultsScreen;

public sealed class PrintFormButton : Button
{
    public PrintFormButton()
    {
        FieldParts.AppLook(this, typeof(Button));   // naandan nga hitsura sa app (apil ang abo kung disabled)
        Text = "Print form";
        HeightRequest = 48;
        CornerRadius = 12;
        FontAttributes = FontAttributes.Bold;
        Margin = new Thickness(0, 8, 0, 0);
    }
}
