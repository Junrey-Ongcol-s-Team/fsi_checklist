// =====================================================================
// Code-behind sa Main page: nagkonektar sa page ug sa iyang ViewModel (BindingContext).
// =====================================================================
using fsi_checklist.ViewModels;

namespace fsi_checklist
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

    }
}
