using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Model;
using WpfApp1.View;

namespace WpfApp1.ViewModel
{
    public class FacilityManagementViewModel : INotifyPropertyChanged
    {
        private readonly Coso_Service _facilityService;
        private Co_so _selectedFacility;

        public ObservableCollection<Co_so> Facilities { get; set; }
        public ObservableCollection<Con_giong_vat_nuoi> ConGiongVatNuois { get; set; }
        public ObservableCollection<animal> Animals { get; set; }
        public ObservableCollection<Tinh_phoi_uu_trung_aptrung> TinhPhois { get; set; }
        public ObservableCollection<Gen_dv> Gens { get; set; }
        public ObservableCollection<Huyen> Huyens { get; set; }
        public ObservableCollection<Xa> Xas { get; set; }
        public ObservableCollection<Loai_hinh_user> LoaiHinhs { get; set; }
        public ObservableCollection<Chung_nhan> ChungNhans { get; set; }

        public string NewName { get; set; }
        public int? NewConGiongVatNuoiId { get; set; }
        public int? NewAnimalId { get; set; }
        public int? NewTinhPhoiId { get; set; }
        public int? NewGenId { get; set; }
        public bool? NewThucAnChanNuoi { get; set; }
        public int? NewChungNhanId { get; set; }
        private int? _newHuyenId;

        public int? NewHuyenId
        {
            get => _newHuyenId;
            set
            {
                if (_newHuyenId != value)
                {
                    _newHuyenId = value;
                    LoadXasForHuyen(_newHuyenId);
                    OnPropertyChanged(nameof(NewHuyenId));
                }
            }
        }

        public int? NewXaId { get; set; }
        public int? NewLoaiHinhId { get; set; }

        public Co_so SelectedFacility
        {
            get => _selectedFacility;
            set
            {
                // Kiểm tra nếu giá trị mới khác giá trị hiện tại
                if (_selectedFacility != value)
                {
                    _selectedFacility = value;

                    if (_selectedFacility != null)
                    {
                        // Load thông tin cơ sở
                        LoadFacilityToFields(_selectedFacility);
                    }
                    else
                    {
                        // Reset nếu không có cơ sở được chọn
                        ResetFields();
                    }

                    // Gọi sự kiện thay đổi
                    OnPropertyChanged(nameof(SelectedFacility));
                }
            }
        }





        public FacilityManagementViewModel(Coso_Service facilityService)
        {
            _facilityService = facilityService;
            LoadDataFromDatabase();
        }

        public ICommand UpdateFacilityCommand => new RelayCommand(_ => UpdateFacility());
        public ICommand DeleteFacilityCommand => new RelayCommand(_ => DeleteFacility());
        public ICommand SearchFacilityCommand => new RelayCommand(_ => SearchFacility());
        public ICommand ResetFieldsCommand => new RelayCommand(_ => ResetFields());
        public ICommand OpenAddFacilityWindowCommand => new RelayCommand(_ => OpenAddFacilityWindow());

        private void OpenAddFacilityWindow()
        {
            // Tạo một cửa sổ AddFacilityWindow mới
            var addFacilityWindow = new AddFacilityWindow
            {
                DataContext = new AddFacilityViewModel(_facilityService) // Truyền DataContext
            };

            // Hiển thị cửa sổ
            addFacilityWindow.ShowDialog();

            // Sau khi đóng cửa sổ, tải lại danh sách cơ sở
            LoadDataFromDatabase();
        }

        private void UpdateFacility()
        {
            if (SelectedFacility != null)
            {
                SelectedFacility.name = NewName;
                SelectedFacility.id_con_giong_vat_nuoi = NewConGiongVatNuoiId;
                SelectedFacility.id_animal = NewAnimalId;
                SelectedFacility.id_tinh_phoi = NewTinhPhoiId;
                SelectedFacility.id_gen = NewGenId;
                SelectedFacility.id_thuc_an_chan_nuoi = NewThucAnChanNuoi;
                SelectedFacility.id_chung_nhan = NewChungNhanId;
                SelectedFacility.id_huyen = NewHuyenId;
                SelectedFacility.id_xa = NewXaId;
                SelectedFacility.id_loai_hinh = NewLoaiHinhId;

                _facilityService.UpdateCoso(SelectedFacility);
                LoadDataFromDatabase(); // Reload DataGrid
                MessageBox.Show("Cập nhật cơ sở thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Vui lòng chọn cơ sở để cập nhật.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteFacility()
        {
            if (SelectedFacility != null)
            {
                _facilityService.DeleteCoso(SelectedFacility.id);
                Facilities.Remove(SelectedFacility);
                MessageBox.Show("Xóa cơ sở thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                ResetFields();
            }
            else
            {
                MessageBox.Show("Vui lòng chọn cơ sở để xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SearchFacility()
        {
            Facilities = new ObservableCollection<Co_so>(_facilityService.SearchCoso(
                name: NewName,
                idConGiongVatNuoi: NewConGiongVatNuoiId,
                idAnimal: NewAnimalId,
                idTinhPhoi: NewTinhPhoiId,
                idLoaiHinh: NewLoaiHinhId,
                idXa: NewXaId,
                idHuyen: NewHuyenId,
                idGen: NewGenId,
                idThucAnChanNuoi: NewThucAnChanNuoi,
                idChungNhan: NewChungNhanId
            ).ToList());

            OnPropertyChanged(nameof(Facilities));
        }

        private void ResetFields()
        {
            SelectedFacility = null;

            NewName = string.Empty;
            NewConGiongVatNuoiId = null;
            NewAnimalId = null;
            NewTinhPhoiId = null;
            NewGenId = null;
            NewThucAnChanNuoi = null;
            NewChungNhanId = null;
            NewHuyenId = null;
            NewXaId = null;
            NewLoaiHinhId = null;

            ConGiongVatNuois = new ObservableCollection<Con_giong_vat_nuoi>(_facilityService.GetAllConGiongVatNuois());
            Animals = new ObservableCollection<animal>(_facilityService.GetAllAnimals());
            Huyens = new ObservableCollection<Huyen>(_facilityService.GetAllHuyens());
            Xas = new ObservableCollection<Xa>();
            LoaiHinhs = new ObservableCollection<Loai_hinh_user>(_facilityService.GetAllLoaiHinhs());
            ChungNhans = new ObservableCollection<Chung_nhan>(_facilityService.GetAllChungNhans());

            Facilities = new ObservableCollection<Co_so>(_facilityService.GetAllCoso());

            OnPropertyChanged(nameof(NewName));
            OnPropertyChanged(nameof(NewConGiongVatNuoiId));
            OnPropertyChanged(nameof(NewAnimalId));
            OnPropertyChanged(nameof(NewTinhPhoiId));
            OnPropertyChanged(nameof(NewGenId));
            OnPropertyChanged(nameof(NewThucAnChanNuoi));
            OnPropertyChanged(nameof(NewChungNhanId));
            OnPropertyChanged(nameof(NewHuyenId));
            OnPropertyChanged(nameof(NewXaId));
            OnPropertyChanged(nameof(NewLoaiHinhId));

            OnPropertyChanged(nameof(ConGiongVatNuois));
            OnPropertyChanged(nameof(Animals));
            OnPropertyChanged(nameof(Huyens));
            OnPropertyChanged(nameof(Xas));
            OnPropertyChanged(nameof(LoaiHinhs));
            OnPropertyChanged(nameof(ChungNhans));
            OnPropertyChanged(nameof(Facilities));
        }





        private void LoadDataFromDatabase()
        {
            Facilities = new ObservableCollection<Co_so>(_facilityService.GetAllCoso());

            // Cập nhật giao diện
            OnPropertyChanged(nameof(Facilities));
        }


        private void LoadXasForHuyen(int? huyenId)
        {
            if (huyenId.HasValue)
            {
                Xas = new ObservableCollection<Xa>(_facilityService.GetXasByHuyenId(huyenId.Value));
            }
            else
            {
                Xas = new ObservableCollection<Xa>();
            }
            OnPropertyChanged(nameof(Xas));
        }

        private void LoadFacilityToFields(Co_so facility)
        {
            NewName = facility.name;
            NewConGiongVatNuoiId = facility.id_con_giong_vat_nuoi;
            NewAnimalId = facility.id_animal;
            NewTinhPhoiId = facility.id_tinh_phoi;
            NewGenId = facility.id_gen;
            NewThucAnChanNuoi = facility.id_thuc_an_chan_nuoi;
            NewChungNhanId = facility.id_chung_nhan;
            NewHuyenId = facility.id_huyen;
            NewXaId = facility.id_xa;
            NewLoaiHinhId = facility.id_loai_hinh;

            OnPropertyChanged(nameof(NewName));
            OnPropertyChanged(nameof(NewConGiongVatNuoiId));
            OnPropertyChanged(nameof(NewAnimalId));
            OnPropertyChanged(nameof(NewTinhPhoiId));
            OnPropertyChanged(nameof(NewGenId));
            OnPropertyChanged(nameof(NewThucAnChanNuoi));
            OnPropertyChanged(nameof(NewChungNhanId));
            OnPropertyChanged(nameof(NewHuyenId));
            OnPropertyChanged(nameof(NewXaId));
            OnPropertyChanged(nameof(NewLoaiHinhId));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
