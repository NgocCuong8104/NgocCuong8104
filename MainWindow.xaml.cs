using System.Windows;
using System.Windows.Controls;
using WpfApp1.ViewModel;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new Login_Register_ViewModel();
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is Login_Register_ViewModel viewModel)
            {
                viewModel.LoginPassword = ((PasswordBox)sender).Password;
            }
        }

        private void OnRegisterPasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is Login_Register_ViewModel viewModel)
            {
                viewModel.RegisterPassword = ((PasswordBox)sender).Password;
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
