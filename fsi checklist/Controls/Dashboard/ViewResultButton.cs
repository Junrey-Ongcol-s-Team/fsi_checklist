// =====================================================================
// DASHBOARD: "View result" button (puno nga kolor). Ablihan ang Results page (basa ra).
// =====================================================================
namespace fsi_checklist.Controls.Dashboard;

public sealed class ViewResultButton : InspectionActionButton
{
    public ViewResultButton() : base("View result")
    {
        BackgroundColor = Primary;
        TextColor = Colors.White;
    }
}
