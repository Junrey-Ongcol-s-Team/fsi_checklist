// =====================================================================
// DASHBOARD: ang pundasyon (base) sa mga button sa UBOS sa Dashboard:
//   SelectInspectionsButton ("Select")  ImportBundleButton ("Import")
//   CancelSelectButton ("Cancel")       SelectAllButton ("Select all")   ExportBundleButton ("Export (n)")
// Ang matag usa naay kaugalingong file, text ug kolor (dili generic).
// =====================================================================
using fsi_checklist.Controls.Common;

namespace fsi_checklist.Controls.Dashboard;

public abstract class DashboardBarButton : Button
{
    protected static Color Primary => FieldParts.Primary;

    protected DashboardBarButton(string text, bool filled)
    {
        Text = text;
        HeightRequest = 52;
        CornerRadius = 12;
        FontSize = 14;
        FontAttributes = FontAttributes.Bold;
        Padding = new Thickness(16, 0);
        if (filled)
        {
            BackgroundColor = Primary;
            TextColor = Colors.White;
        }
        else
        {
            BackgroundColor = Colors.Transparent;
            BorderColor = Primary;
            BorderWidth = 1;
            TextColor = Primary;
        }
    }
}
