// =====================================================================
// DASHBOARD: "Export (n)" button (puno nga kolor). Himoon ang .zip sa mga gi-tick nga inspection
// ug ablihan ang Share (Bluetooth, Messenger, Drive ...). Ang text gikan sa ViewModel (ExportText).
// =====================================================================
namespace fsi_checklist.Controls.Dashboard;

public sealed class ExportBundleButton : DashboardBarButton
{
    public ExportBundleButton() : base("Export", filled: true) { }
}
