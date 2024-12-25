using System.Windows.Controls;
using WpfApp1.ViewModel;

namespace WpfApp1.View
{
    public partial class UserManagementPage : Page
    {
        public UserManagementPage()
        {
            InitializeComponent();
            DataContext = new UserManagementViewModel();
        }
    }
}
