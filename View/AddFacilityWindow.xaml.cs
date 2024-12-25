using System.Windows;

namespace WpfApp1.View
{
    public partial class AddFacilityWindow : Window
    {
        public AddFacilityWindow()
        {
            InitializeComponent();
        }

        // Add the CancelButton_Click event handler
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Close the window when the "Cancel" button is clicked
            this.Close();
        }
    }
}
