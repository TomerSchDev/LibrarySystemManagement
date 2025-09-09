using System.Windows;
using System.Windows.Input;
using Library_System_Management.Models;
using Library_System_Management.Services;

namespace Library_System_Management.Views;

public partial class UsersWindow : Window
{
    public UsersWindow()
    {
        InitializeComponent();
        dgUsers.ItemsSource = AuthService.GetUsers();
        LoadTables();
        DataContext = this;
    }

    private void LoadTables()
    {
        dgUsers.ItemsSource = null;
        dgUsers.ItemsSource = AuthService.GetUsers();
    }

    private void dgUsers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (dgUsers.SelectedItem is not User user) return;
        var userInfo = new UserInfoWindow(user);
        userInfo.ShowDialog();
    }

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        var addUser = new AddNewUser();
        if  (addUser.ShowDialog() != true) return;
        var user = addUser.user;
        if (user == null) return;
        if (!AuthService.CreateNewUser(user)) return;
        LoadTables();
    }

    private void BtnExport_Click(object sender, RoutedEventArgs e)
    {
        ExportDialog.ExportWindow([..AuthService.GetUsers()]);
    }
}