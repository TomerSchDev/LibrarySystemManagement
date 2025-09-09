using System.Windows;
using Library_System_Management.Services;
using Library_System_Management.Models;

namespace Library_System_Management.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var user = AuthService.GetUserByUsername(txtUsername.Text);
            if (user == null)
            {
                MessageBox.Show("Couldn't find user with this username : " + txtUsername.Text, "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            MessageBox.Show("Login successful!");
            if (user.ValidatePassword(txtPassword.Password))
            {
                var dashboard = new DashboardWindow(user);
                dashboard.Show();
                Close();
            }
            else
            {
                MessageBox.Show("Invalid credentials!");
            }
        }
    }
}