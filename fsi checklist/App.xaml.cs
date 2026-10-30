// =====================================================================
// Ang App mismo. Ang CreateWindow mohimo sa unang window ug ibutang ang AppShell (ang navigation) sulod niini.
// =====================================================================
using fsi_checklist.Services;

namespace fsi_checklist
{
    public partial class App : Application
    {
        public App(DatabaseService databaseService)
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}