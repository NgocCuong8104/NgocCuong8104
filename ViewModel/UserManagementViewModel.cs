using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Entity.Validation;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Model;

namespace WpfApp1.ViewModel
{
    public class UserManagementViewModel : INotifyPropertyChanged
    {
        private readonly User_Service _userService;
        private readonly Huyen_Xa_Service _huyenXaService;
        private user _selectedUser;

        public ObservableCollection<Huyen> Huyens { get; set; }
        public ObservableCollection<Xa> Xas { get; set; }
        public ObservableCollection<Role> Roles { get; set; }
        public ObservableCollection<user> Users { get; set; }

        // New properties for adding user
        public string NewUsername { get; set; }
        private int? _newHuyenId;
        public int? NewHuyenId
        {
            get => _newHuyenId;
            set
            {
                if (_newHuyenId != value)
                {
                    _newHuyenId = value;
                    LoadNewXas(); // Tải lại danh sách xã khi huyện thay đổi
                    NewXaId = null; // Reset lựa chọn xã
                    OnPropertyChanged(nameof(NewHuyenId));
                    OnPropertyChanged(nameof(NewXaId));
                }
            }
        }

        public int? NewXaId { get; set; }
        public string NewSdt { get; set; }
        public string NewEmail { get; set; }
        public int? NewRoleId { get; set; }
        public string NewPassword { get; set; }


        public ObservableCollection<Xa> NewXas { get; set; }

        public user SelectedUser
        {
            get => _selectedUser;
            set
            {
                _selectedUser = value;
                if (_selectedUser != null)
                {
                    NewUsername = _selectedUser.user_name;
                    NewHuyenId = _selectedUser.id_huyen;
                    NewXaId = _selectedUser.id_xa;
                    NewSdt = _selectedUser.sdt;
                    NewEmail = _selectedUser.email;
                    NewRoleId = _selectedUser.id_role;
                    LoadXas();
                }
                else
                {
                    ResetField(); // Reset form khi không có người dùng được chọn
                }
                OnPropertyChanged(nameof(SelectedUser));
                OnPropertyChanged(nameof(NewUsername));
                OnPropertyChanged(nameof(NewHuyenId));
                OnPropertyChanged(nameof(NewXaId));
                OnPropertyChanged(nameof(NewSdt));
                OnPropertyChanged(nameof(NewEmail));
                OnPropertyChanged(nameof(NewRoleId));
            }
        }


        public UserManagementViewModel()
        {
            _userService = new User_Service(new YourDbContext());
            _huyenXaService = new Huyen_Xa_Service();

            Huyens = new ObservableCollection<Huyen>(_huyenXaService.dbContext.Huyens.ToList());
            Roles = new ObservableCollection<Role>(_userService.GetAllRoles());
            NewXas = new ObservableCollection<Xa>();

            // Load dữ liệu vào DataGrid
            LoadDataFromDatabase();
        }





        private void UserManagementViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(NewHuyenId))
            {
                LoadNewXas();
                // Reset lại NewXaId để tránh chọn xã cũ không hợp lệ
                NewXaId = null;
                OnPropertyChanged(nameof(NewXaId));
            }
        }

        public ICommand OpenAddUserWindowCommand => new RelayCommand(param => OpenAddUserWindow());

        private void OpenAddUserWindow()
        {
            // Tạo cửa sổ AddUserWindow
            var addUserWindow = new View.AddUserWindow
            {
                DataContext = this // Gắn ViewModel vào cửa sổ
            };
            addUserWindow.ShowDialog();
        }



        private void LoadDataFromDatabase()
        {
            try
            {
                // Tải danh sách người dùng
                Users = new ObservableCollection<user>(_userService.GetAllUsers().ToList());

                // Tải các dữ liệu liên quan khác
                Huyens = new ObservableCollection<Huyen>(_huyenXaService.dbContext.Huyens.ToList());
                Roles = new ObservableCollection<Role>(_userService.GetAllRoles());
                NewXas = new ObservableCollection<Xa>();

                // Kích hoạt thông báo cho UI
                OnPropertyChanged(nameof(Users));
                OnPropertyChanged(nameof(Huyens));
                OnPropertyChanged(nameof(Roles));
                OnPropertyChanged(nameof(NewXas));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void LoadXas()
        {
            if (NewHuyenId != null)
            {
                Xas = new ObservableCollection<Xa>(_huyenXaService.dbContext.Xas.Where(x => x.id_Huyen == NewHuyenId).ToList());
            }
            else
            {
                Xas.Clear();
            }
            OnPropertyChanged(nameof(Xas));
        }

        private void LoadNewXas()
        {
            if (NewHuyenId != null)
            {
                NewXas = new ObservableCollection<Xa>(
                    _huyenXaService.dbContext.Xas.Where(x => x.id_Huyen == NewHuyenId).ToList());
                Console.WriteLine($"Loaded {NewXas.Count} Xas for HuyenId {NewHuyenId}");
            }
            else
            {
                NewXas.Clear();
                Console.WriteLine("Cleared Xas because NewHuyenId is null.");
            }
            OnPropertyChanged(nameof(NewXas));
        }





        public ICommand AddUserCommand => new RelayCommand(param => AddUser());

        public ICommand UpdateUserCommand => new RelayCommand(param => UpdateUser());
        public ICommand DeleteUserCommand => new RelayCommand(param => DeleteUser());
        public ICommand SearchUserCommand => new RelayCommand(param => SearchUser());
        public ICommand ResetFieldCommand => new RelayCommand(param => ResetField());
        public ICommand ReloadDataCommand => new RelayCommand(param => LoadDataFromDatabase());



        private void AddUser()
        {
            if (!ValidateUser())
                return;

            try
            {
                // Kiểm tra nếu username đã tồn tại
                if (_userService.CheckUserExists(NewUsername))
                {
                    MessageBox.Show("Username đã tồn tại. Vui lòng chọn một tên khác.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Tạo người dùng mới
                var newUser = new user
                {
                    user_name = NewUsername,
                    password = NewPassword,
                    id_huyen = NewHuyenId,
                    id_xa = NewXaId,
                    sdt = NewSdt,
                    email = NewEmail,
                    id_role = NewRoleId
                };

                _userService.AddUser(newUser); // Gọi hàm thêm user vào database
                MessageBox.Show("Thêm người dùng thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm người dùng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }



        private void UpdateUser()
        {
            if (SelectedUser != null)
            {
                SelectedUser.user_name = NewUsername;
                SelectedUser.id_huyen = NewHuyenId;
                SelectedUser.id_xa = NewXaId;
                SelectedUser.sdt = NewSdt;
                SelectedUser.email = NewEmail;
                SelectedUser.id_role = NewRoleId;

                _userService.UpdateUser(SelectedUser);
                MessageBox.Show("Cập nhật người dùng thành công!");
            }
            else
            {
                MessageBox.Show("Vui lòng chọn người dùng để cập nhật.");
            }
        }

        private void DeleteUser()
        {
            if (SelectedUser != null)
            {
                _userService.DeleteUser(SelectedUser.id);
                Users.Remove(SelectedUser);
                MessageBox.Show("Xóa người dùng thành công!");
                ResetField();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn người dùng để xóa.");
            }
        }
        private bool ValidateUser()
        {
            if (string.IsNullOrEmpty(NewUsername) || NewUsername.Length < 3)
            {
                MessageBox.Show("Username phải có ít nhất 3 ký tự.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (NewHuyenId == null)
            {
                MessageBox.Show("Vui lòng chọn Huyện.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (NewXaId == null)
            {
                MessageBox.Show("Vui lòng chọn Xã.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(NewSdt) || !NewSdt.All(char.IsDigit))
            {
                MessageBox.Show("Số điện thoại không hợp lệ. Chỉ chấp nhận số.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(NewEmail) || !NewEmail.Contains("@"))
            {
                MessageBox.Show("Email không hợp lệ.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            if (string.IsNullOrEmpty(NewPassword) || NewPassword.Length < 6)
            {
                MessageBox.Show("Password phải có ít nhất 6 ký tự.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }



        private void ResetField()
        {
            NewUsername = string.Empty;
            NewHuyenId = null;
            NewXaId = null;
            NewSdt = string.Empty;
            NewEmail = string.Empty;
            NewRoleId = null;

            OnPropertyChanged(nameof(NewUsername));
            OnPropertyChanged(nameof(NewHuyenId));
            OnPropertyChanged(nameof(NewXaId));
            OnPropertyChanged(nameof(NewSdt));
            OnPropertyChanged(nameof(NewEmail));
            OnPropertyChanged(nameof(NewRoleId));
            LoadDataFromDatabase();
        }


        private void SearchUser()
        {
            var result = _userService.SearchUsers(null, NewUsername, NewSdt, NewEmail, NewXaId, NewHuyenId).ToList();
            if (result.Count == 0)
            {
                MessageBox.Show("Không tìm thấy người dùng nào phù hợp.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            Users = new ObservableCollection<user>(result);
            OnPropertyChanged(nameof(Users));
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}