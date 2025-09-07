using System.Windows;
using Library_System_Management.Views;

namespace Library_System_Management
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Navigate to LoginWindow inside the frame
            MainFrame.Navigate(new LoginWindow());
        }

        // Optional: method to navigate between pages
        public void NavigateToPage(System.Windows.Controls.Page page)
        {
            MainFrame.Navigate(page);
        }
    }
}