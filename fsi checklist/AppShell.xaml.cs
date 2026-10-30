// =====================================================================
// Dinhi gi-register ang ngalan (route) sa matag page.
// Kung naay page nga dili ma-register diri, mag-crash ang GoToAsync ug moingon og 'unable to figure out route'.
// =====================================================================
using fsi_checklist.Views;

namespace fsi_checklist
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(InspectionPage), typeof(InspectionPage));
            Routing.RegisterRoute(nameof(DashboardPage), typeof(DashboardPage));
            Routing.RegisterRoute(nameof(ChecklistTypesPage), typeof(ChecklistTypesPage));
            // Ang bag-ong mga page para sa pirma.
            Routing.RegisterRoute(nameof(SignoffPage), typeof(SignoffPage));
            Routing.RegisterRoute(nameof(SignaturePage), typeof(SignaturePage));
            Routing.RegisterRoute(nameof(ResultsPage), typeof(ResultsPage));
            Routing.RegisterRoute(nameof(NewInspectionPage), typeof(NewInspectionPage));
            Routing.RegisterRoute(nameof(FormPage), typeof(FormPage));
            Routing.RegisterRoute(nameof(SettingsPage), typeof(SettingsPage));
        }
    }
}
