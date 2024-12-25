using System.Windows;
using WpfApp1.Model;

namespace WpfApp1.View
{
    public partial class UserWindow : Window
    {
        private user _loggedInUser;
        private YourDbContext _dbContext;

        public UserWindow(user loggedInUser, YourDbContext dbContext)
        {
            InitializeComponent();
            _loggedInUser = loggedInUser;
            _dbContext = dbContext;
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            // Mở lại cửa sổ đăng nhập
            MainWindow loginWindow = new MainWindow();
            loginWindow.Show();

            // Đóng cửa sổ hiện tại
            this.Close();
        }

        private void RowDefinition_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }
    }
}
