using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WpfApp1.Model;

namespace WpfApp1.ViewModel
{
    public class AddFacilityViewModel : INotifyPropertyChanged
    {
        private readonly Coso_Service _facilityService;

        // Properties for new facility fields
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
                    LoadXasForHuyen(_newHuyenId); // Load Xas based on selected Huyen
                    OnPropertyChanged(nameof(NewHuyenId));
                }
            }
        }
        private void LoadXasForHuyen(int? huyenId)
        {
            if (huyenId.HasValue)
            {
                // Fetch Xas dynamically based on HuyenId
                Xas = new ObservableCollection<Xa>(_facilityService.GetXasByHuyenId(huyenId.Value));
            }
            else
            {
                // Clear Xas if no Huyen is selected
                Xas = new ObservableCollection<Xa>();
            }
            OnPropertyChanged(nameof(Xas)); // Notify UI of the change
        }

        public int? NewXaId { get; set; }
        public int? NewLoaiHinhId { get; set; }

        // Collections for ComboBoxes
        public ObservableCollection<Con_giong_vat_nuoi> ConGiongVatNuois { get; set; }
        public ObservableCollection<animal> Animals { get; set; }
        public ObservableCollection<Tinh_phoi_uu_trung_aptrung> TinhPhois { get; set; }
        public ObservableCollection<Gen_dv> Gens { get; set; }
        public ObservableCollection<Huyen> Huyens { get; set; }
        public ObservableCollection<Xa> Xas { get; set; } = new ObservableCollection<Xa>();
        public ObservableCollection<Loai_hinh_user> LoaiHinhs { get; set; }
        public ObservableCollection<Chung_nhan> ChungNhans { get; set; }
       

        public ICommand SaveNewFacilityCommand { get; }

        public AddFacilityViewModel(Coso_Service facilityService)
        {
            _facilityService = facilityService;

            // Initialize collections
            ConGiongVatNuois = new ObservableCollection<Con_giong_vat_nuoi>();
            Animals = new ObservableCollection<animal>();
            TinhPhois = new ObservableCollection<Tinh_phoi_uu_trung_aptrung>();
            Gens = new ObservableCollection<Gen_dv>();
            Huyens = new ObservableCollection<Huyen>();
            Xas = new ObservableCollection<Xa>();
            LoaiHinhs = new ObservableCollection<Loai_hinh_user>();
            ChungNhans = new ObservableCollection<Chung_nhan>();

            // Load data into collections
            LoadData();

            SaveNewFacilityCommand = new RelayCommand(_ => SaveNewFacility());
        }

        private void LoadData()
        {
            // Load data from the service into collections
            ConGiongVatNuois = new ObservableCollection<Con_giong_vat_nuoi>(_facilityService.GetAllConGiongVatNuois());
            Animals = new ObservableCollection<animal>(_facilityService.GetAllAnimals());
            TinhPhois = new ObservableCollection<Tinh_phoi_uu_trung_aptrung>(_facilityService.GetAllTinhPhois());
            Gens = new ObservableCollection<Gen_dv>(_facilityService.GetAllGens());
            Huyens = new ObservableCollection<Huyen>(_facilityService.GetAllHuyens());
            LoaiHinhs = new ObservableCollection<Loai_hinh_user>(_facilityService.GetAllLoaiHinhs());
            ChungNhans = new ObservableCollection<Chung_nhan>(_facilityService.GetAllChungNhans());

            // Notify UI of data changes
            OnPropertyChanged(nameof(ConGiongVatNuois));
            OnPropertyChanged(nameof(Animals));
            OnPropertyChanged(nameof(TinhPhois));
            OnPropertyChanged(nameof(Gens));
            OnPropertyChanged(nameof(Huyens));
            OnPropertyChanged(nameof(Xas));
            OnPropertyChanged(nameof(LoaiHinhs));
            OnPropertyChanged(nameof(ChungNhans));
        }

        private void SaveNewFacility()
        {
            if (string.IsNullOrEmpty(NewName))
            {
                MessageBox.Show("Tên cơ sở không được để trống.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Create a new facility object
            var newFacility = new Co_so
            {
                name = NewName,
                id_con_giong_vat_nuoi = NewConGiongVatNuoiId,
                id_animal = NewAnimalId,
                id_tinh_phoi = NewTinhPhoiId,
                id_gen = NewGenId,
                id_thuc_an_chan_nuoi = NewThucAnChanNuoi,
                id_chung_nhan = NewChungNhanId,
                id_huyen = NewHuyenId,
                id_xa = NewXaId,
                id_loai_hinh = NewLoaiHinhId
            };

            // Add to the database or collection
            _facilityService.AddCoso(newFacility);

            MessageBox.Show("Thêm cơ sở thành công!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);

            // Close the current window
            Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive)?.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
