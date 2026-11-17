// =====================================================================
// DASHBOARD: "Cancel" button. Mobalik sa normal (tago ang checkbox, walay napili).
// =====================================================================
namespace fsi_checklist.Controls.Dashboard;

public sealed class CancelSelectButton : DashboardBarButton
{
    public CancelSelectButton() : base("Cancel", filled: false)
    {
        // Abo nga outline: dili kini ang main nga lihok
        BorderColor = Color.FromArgb("#9E9E9E");
        TextColor = Color.FromArgb("#616161");
    }
}
