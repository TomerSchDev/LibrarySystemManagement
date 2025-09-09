using System.Windows;
using Library_System_Management.Models;

namespace Library_System_Management.Views;

public partial class AddNewUser : Window
{
    public User? user;
    public AddNewUser()
    {
        InitializeComponent();
        user = new User();
        cmbRoles.ItemsSource =Enum.GetValues<UserRole>();
        DataContext = this;
    }

    private void BtnConfirm_Click(object sender, RoutedEventArgs e)
    {
        var username = TxtUsername.Text;
        var password = TxtPassword.Password;
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show("Username and password are required!");
            return;
        }

        if (cmbRoles.SelectionBoxItem is not UserRole role)
        {
            MessageBox.Show("Error: Please select a role!");
            return;
        }

        user ??= new User();
        user.Username = username;
        user.Password=password;
        user.Role = role;
        DialogResult = true;
        Close();
    }
}