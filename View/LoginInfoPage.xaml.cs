using System.Windows.Controls;
using WpfApp1.ViewModel;
using WpfApp1.Model;
using System.Windows;

namespace WpfApp1.View
{
    public partial class LoginInfoPage : Page
    {
        public LoginInfoPage(user loggedInUser, YourDbContext dbContext)
        {
            InitializeComponent();
            DataContext = new LoginInfoViewModel(loggedInUser, dbContext);
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as LoginInfoViewModel;
            if (viewModel != null)
            {
                viewModel.LoggedInUser.password = ((PasswordBox)sender).Password;
            }
        }
    }
}
