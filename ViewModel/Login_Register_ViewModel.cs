using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Model;
using WpfApp1.View;

namespace WpfApp1.ViewModel
{
    internal class Login_Register_ViewModel : INotifyPropertyChanged
    {
        private User_Service _userService;
        private YourDbContext _dbContext;

        // Properties for Binding
        private string _loginUsername;
        public string LoginUsername
        {
            get { return _loginUsername; }
            set
            {
                _loginUsername = value;
                OnPropertyChanged("LoginUsername");
            }
        }

        private string _loginPassword;
        public string LoginPassword
        {
            get { return _loginPassword; }
            set
            {
                _loginPassword = value;
                OnPropertyChanged("LoginPassword");
            }
        }

        private string _registerUsername;
        public string RegisterUsername
        {
            get { return _registerUsername; }
            set
            {
                _registerUsername = value;
                OnPropertyChanged("RegisterUsername");
            }
        }

        private string _registerPassword;
        public string RegisterPassword
        {
            get { return _registerPassword; }
            set
            {
                _registerPassword = value;
                OnPropertyChanged("RegisterPassword");
            }
        }

        private Role _selectedRole;
        public Role SelectedRole
        {
            get { return _selectedRole; }
            set
            {
                _selectedRole = value;
                OnPropertyChanged("SelectedRole");
            }
        }

        public List<Role> Roles { get; set; }

        // Commands
        public ICommand LoginCommand { get; set; }
        public ICommand RegisterCommand { get; set; }

        public Login_Register_ViewModel()
        {
            _dbContext = new YourDbContext();
            _userService = new User_Service(_dbContext);

          //  Roles = _dbContext.Roles.ToList();

            LoginCommand = new RelayCommand(Login);
            RegisterCommand = new RelayCommand(Register);
        }

        private void Login(object obj)
        {
            try
            {
                var user = _userService.ValidateUser(LoginUsername, LoginPassword);
                if (user != null)
                {
                    MessageBox.Show("Đăng nhập thành công!");

                    // Mở cửa sổ tương ứng dựa trên vai trò của người dùng
                    Window window = null;
                    if (user.id_role == 2) // id = 2 là admin
                    {
                        window = new AdminWindow(user, _dbContext);
                    }
                    else // id = 1 là user
                    {
                        window = new UserWindow(user, _dbContext);
                    }

                    // Hiển thị cửa sổ mới và đóng MainWindow
                    window.Show();
                    Application.Current.MainWindow.Close();
                }
                else
                {
                    MessageBox.Show("Sai thông tin đăng nhập.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        private void Register(object obj)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(RegisterUsername) || string.IsNullOrWhiteSpace(RegisterPassword) || SelectedRole == null)
                {
                    MessageBox.Show("Vui lòng điền đầy đủ thông tin đăng kí.");
                    return;
                }

                var newUser = new user { user_name = RegisterUsername, password = RegisterPassword, Role = SelectedRole };
                _userService.AddUser(newUser);

                MessageBox.Show("Đăng kí thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
