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
            var user = AuthService.Login(txtUsername.Text, txtPassword.Password);
            if(user != null)
            {
                MessageBox.Show("Login successful!");
                
                var dashboard = new DashboardWindow(user);
                dashboard.Show();

                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid credentials!");
            }
        }
    }
}