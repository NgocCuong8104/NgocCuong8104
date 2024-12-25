using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Model;

namespace WpfApp1.ViewModel
{
    public class LoginInfoViewModel : INotifyPropertyChanged
    {
        private User_Service _userService;
        private YourDbContext _dbContext;
        private user _loggedInUser;
        private ICommand _saveCommand;
        private ICommand _refreshCommand;

        public user LoggedInUser
        {
            get { return _loggedInUser; }
            set
            {
                _loggedInUser = value;
                OnPropertyChanged(nameof(LoggedInUser)); // Thông báo cập nhật dữ liệu

                // Cập nhật SelectedHuyen dựa trên LoggedInUser
                SelectedHuyen = Huyens.FirstOrDefault(h => h.id_Huyen == _loggedInUser.id_huyen);
                LoadXas(); // Tải lại danh sách xã tương ứng
            }
        }

        public ObservableCollection<Huyen> Huyens { get; set; }
        public ObservableCollection<Xa> Xas { get; set; }
        public ObservableCollection<quy_mo> QuyMo { get; set; }

        private Huyen _selectedHuyen;
        public Huyen SelectedHuyen
        {
            get { return _selectedHuyen; }
            set
            {
                if (_selectedHuyen != value)
                {
                    _selectedHuyen = value;
                    OnPropertyChanged(nameof(SelectedHuyen));

                    // Cập nhật Huyện trong LoggedInUser
                    if (value != null)
                    {
                        LoggedInUser.id_huyen = value.id_Huyen;
                    }

                    // Tải lại danh sách xã
                    LoadXas();
                }
            }
        }

        public ICommand ResetCommand
        {
            get
            {
                return new RelayCommand(param => LoadData());
            }
        }


        private void LoadData()
        {
            try
            {
                Debug.WriteLine("LoadData bắt đầu.");

                // Lấy thông tin mới của người dùng từ cơ sở dữ liệu
                var reloadedUser = _userService.GetUserById(LoggedInUser.id);
                if (reloadedUser != null)
                {
                    LoggedInUser = reloadedUser;
                    OnPropertyChanged(nameof(LoggedInUser)); // Cập nhật Binding
                }

                // Tải danh sách Huyens
                Huyens = new ObservableCollection<Huyen>(_dbContext.Huyens.ToList());
                OnPropertyChanged(nameof(Huyens));

                // Tải danh sách QuyMo
                QuyMo = new ObservableCollection<quy_mo>(_dbContext.quy_mo.ToList());
                OnPropertyChanged(nameof(QuyMo));

                // Tải danh sách Xas dựa trên Huyện hiện tại
                SelectedHuyen = Huyens.FirstOrDefault(h => h.id_Huyen == LoggedInUser.id_huyen);
                LoadXas();

                Debug.WriteLine("LoadData hoàn thành.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Lỗi trong LoadData: {ex.Message}");
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }






        public LoginInfoViewModel(user loggedInUser, YourDbContext dbContext)
        {
            _dbContext = dbContext;
            _userService = new User_Service(_dbContext);

            Huyens = new ObservableCollection<Huyen>(_dbContext.Huyens.ToList());
            QuyMo = new ObservableCollection<quy_mo>(_dbContext.quy_mo.ToList());

            LoggedInUser = loggedInUser;  // Gán LoggedInUser sẽ kích hoạt cập nhật SelectedHuyen và danh sách Xas
        }

        private void LoadXas()
        {
            if (SelectedHuyen != null)
            {
                Xas = new ObservableCollection<Xa>(_userService.GetXasByHuyenId(SelectedHuyen.id_Huyen));
            }
            else
            {
                Xas = new ObservableCollection<Xa>();
            }
            OnPropertyChanged(nameof(Xas));
        }

        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(param => SaveChanges(), param => CanSaveChanges());
                }
                return _saveCommand;
            }
        }

        private bool CanSaveChanges()
        {
            return LoggedInUser != null;
        }

        private void SaveChanges()
        {
            try
            {
                _userService.UpdateUser(LoggedInUser);
                MessageBox.Show("Lưu thay đổi thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
