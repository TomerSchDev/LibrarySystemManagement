using System.Windows;
using LibrarySystemModels.Services;
using LibrarySystemModels.Models;

namespace Library_System_Management.Views.PopUpDialogs;

public partial class AddNewUser : Window
{
    public User? User;
    public AddNewUser()
    {
        InitializeComponent();
        User = null;
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

        User = AuthService.CreateUser(username, password, role);
        DialogResult = true;
        Close();
    }
}