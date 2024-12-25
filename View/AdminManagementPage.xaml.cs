using System.Windows.Controls;
using WpfApp1.ViewModel;

namespace WpfApp1.View
{
    public partial class AdminManagementPage : Page
    {
        public AdminManagementPage()
        {
            InitializeComponent();
            DataContext = new AdminManagementViewModel();
        }
    }
}
