using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using LibrarySystemModels.Services;
using Library_System_Management.Views.PopUpDialogs;
using LibrarySystemModels.Helpers;
using LibrarySystemModels.Models;
using System.Threading.Tasks;

namespace Library_System_Management.Views
{
    public partial class UsersWindow : Window
    {
        // Recommended: back the DataGrid with an ObservableCollection for UI updates
        public ObservableCollection<User> Users { get; set; } = new();

        public UsersWindow()
        {
            InitializeComponent();
            DataContext = this; // For XAML data binding support
            Loaded += UsersWindow_Loaded;
        }

        private async void UsersWindow_Loaded(object sender, RoutedEventArgs e)
        {
            await LoadTablesAsync();
        }

        private async Task LoadTablesAsync()
        {
            try
            {
                Users.Clear();
                var res = await AuthService.GetUsersAsync(FlowSide.Client);
                if (res.ActionResult)
                {
                    foreach (var user in res.Data)
                        Users.Add(user);
                }
                else
                {
                    MessageBox.Show(res.Message, "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                dgUsers.ItemsSource = Users;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                dgUsers.ItemsSource = null;
            }
        }

        private void dgUsers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgUsers.SelectedItem is not User user) return;
            var userInfo = new UserInfoWindow(user);
            userInfo.ShowDialog();
        }

        private async void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var addUser = new AddNewUser();
                if (addUser.ShowDialog() != true) return;
                var user = addUser.User;
                if (user == null) return;
                var success = await AuthService.CreateNewUserAsync(FlowSide.Client, user);
                if (!success)
                {
                    MessageBox.Show("Failed to add new user.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                await LoadTablesAsync();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var res = await AuthService.GetUsersAsync(FlowSide.Client);
                if (res.ActionResult)
                    ExportDialog.ExportWindow([..res.Data.ToList()]);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
