using System.Windows;
using System.Windows.Controls;
using WpfApp1.Model;

namespace WpfApp1.View
{
    public partial class AdminWindow : Window
    {
        private YourDbContext _dbContext;
        private user _loggedInUser;

        public AdminWindow(user loggedInUser, YourDbContext dbContext)
        {
            InitializeComponent();
            _loggedInUser = loggedInUser;
            _dbContext = dbContext;
        }

        private void LoginInfoButton_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new LoginInfoPage(_loggedInUser, _dbContext));
        }

        private void AdminManagementButton_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new AdminManagementPage());
        }

        private void UserManagementButton_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new UserManagementPage());
        }

        private void FacilityManagementButton_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new FacilityManagementPage());
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Mở lại cửa sổ đăng nhập
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();

            // Đóng cửa sổ hiện tại
            this.Close();
        }

        private void MainContentFrame_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

        }
        private void LogManagementButton_Click(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new LogManagementPage());
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
