using System.Threading.Tasks;
using System.Windows;
using Library_System_Management.ExportServices;
using Library_System_Management.Views.PopUpDialogs;
using LibrarySystemModels.Helpers;
using LibrarySystemModels.Services;
using LibrarySystemModels.Models; // (for User.DefaultUser and model)

namespace Library_System_Management.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var user = await AuthService.LoginAsync(FlowSide.Client, txtUsername.Text, txtPassword.Password);
                if (User.IsDefaultUser(user))
                {
                    MessageBox.Show($"Couldn't find user with this username: {txtUsername.Text}", "Warning",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var dashWindow = new DashboardWindow(user);
                dashWindow.Show();
                Close();
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowMessage(ex);
            }
        }

        private void BtnConnectServer(object sender, RoutedEventArgs e)
        {
            var connection = new ConnectToServerWindow();
            connection.Show();
        }
    }
}