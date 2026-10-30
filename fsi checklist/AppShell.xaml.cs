using fsi_checklist.Views;

namespace fsi_checklist
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(InspectionPage), typeof(InspectionPage));
            Routing.RegisterRoute(nameof(NewInspectionPage), typeof(NewInspectionPage));
            Routing.RegisterRoute(nameof(FormPage), typeof(FormPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        }
    }
}
