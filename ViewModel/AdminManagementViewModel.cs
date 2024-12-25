using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Model;

namespace WpfApp1.ViewModel
{
    public class XaViewModel : INotifyPropertyChanged
    {
        public int id_xa { get; set; }
        public string name { get; set; }
        public int id_Huyen { get; set; }
        public string HuyenName { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class AdminManagementViewModel : INotifyPropertyChanged
    {
        private Huyen_Xa_Service _huyenXaService;
        private Huyen _selectedHuyen;

        public ObservableCollection<Huyen> Huyens { get; set; }
        public ObservableCollection<XaViewModel> Xas { get; set; }

        public Huyen SelectedHuyen
        {
            get { return _selectedHuyen; }
            set
            {
                _selectedHuyen = value;
                OnPropertyChanged(nameof(SelectedHuyen));
                LoadXas();
            }
        }

        public AdminManagementViewModel()
        {
            _huyenXaService = new Huyen_Xa_Service();
            Huyens = new ObservableCollection<Huyen>(_huyenXaService.dbContext.Huyens.ToList());
            Xas = new ObservableCollection<XaViewModel>();
        }

        private void LoadXas()
        {
            if (SelectedHuyen != null)
            {
                Xas = new ObservableCollection<XaViewModel>(_huyenXaService.dbContext.Xas
                    .Where(x => (int?)x.id_Huyen == SelectedHuyen.id_Huyen) // Ép kiểu an toàn
                    .Select(x => new XaViewModel
                    {
                        id_xa = x.id_xa,
                        name = x.name,
                        id_Huyen = x.id_Huyen ?? 0,
                        HuyenName = x.Huyen.name
                    })
                    .ToList());
            }
            else
            {
                Xas.Clear();
            }
            OnPropertyChanged(nameof(Xas));
        }

        public ICommand AddHuyenCommand => new RelayCommand(param => AddHuyen());
        public ICommand AddXaCommand => new RelayCommand(param => AddXa());
        public ICommand UpdateCommand => new RelayCommand(param => Update());

        private void AddHuyen()
        {
            var newHuyen = new Huyen { name = "New Huyen" };
            _huyenXaService.dbContext.Huyens.Add(newHuyen);
            _huyenXaService.dbContext.SaveChanges();
            Huyens.Add(newHuyen);
            MessageBox.Show("Thêm Huyện thành công!");
        }

        private void AddXa()
        {
            if (SelectedHuyen != null)
            {
                var newXa = new Xa { name = "New Xa", id_Huyen = SelectedHuyen.id_Huyen };
                _huyenXaService.dbContext.Xas.Add(newXa);
                _huyenXaService.dbContext.SaveChanges();
                Xas.Add(new XaViewModel
                {
                    id_xa = newXa.id_xa,
                    name = newXa.name,
                    id_Huyen = newXa.id_Huyen ?? 0,  // Ép kiểu với giá trị mặc định là 0 nếu null
                    HuyenName = SelectedHuyen.name
                });
                MessageBox.Show("Thêm Xã thành công!");
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một huyện.");
            }
        }

        private void Update()
        {
            foreach (var xa in Xas)
            {
                var xaToUpdate = _huyenXaService.dbContext.Xas.FirstOrDefault(x => x.id_xa == xa.id_xa);
                if (xaToUpdate != null)
                {
                    xaToUpdate.name = xa.name;

                    // Cập nhật tên huyện nếu thay đổi
                    var huyenToUpdate = _huyenXaService.dbContext.Huyens.FirstOrDefault(h => h.id_Huyen == xa.id_Huyen);
                    if (huyenToUpdate != null && huyenToUpdate.name != xa.HuyenName)
                    {
                        huyenToUpdate.name = xa.HuyenName;
                    }
                }
            }

            _huyenXaService.dbContext.SaveChanges();

            // Reload dữ liệu để cập nhật ComboBox và DataGrid
            ReloadHuyens();
            LoadXas();
            MessageBox.Show("Cập nhật thông tin thành công!");
        }

        private void ReloadHuyens()
        {
            var updatedHuyens = _huyenXaService.dbContext.Huyens.ToList();
            Huyens.Clear();
            foreach (var huyen in updatedHuyens)
            {
                Huyens.Add(huyen);
            }

            // Đặt lại SelectedHuyen
            if (_selectedHuyen != null)
            {
                SelectedHuyen = Huyens.FirstOrDefault(h => h.id_Huyen == _selectedHuyen.id_Huyen);
            }
        }




        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
