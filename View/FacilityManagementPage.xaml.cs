using System.Windows.Controls;
using WpfApp1.ViewModel;
using WpfApp1.Model;

namespace WpfApp1.View
{
    public partial class FacilityManagementPage : Page
    {
        public FacilityManagementPage()
        {
            InitializeComponent();

            // Tạo một đối tượng Coso_Service
            var cosoService = new Coso_Service(new YourDbContext());

            // Truyền Coso_Service vào ViewModel
            DataContext = new FacilityManagementViewModel(cosoService);
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
