using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Library_System_Management.ExportServices;
using LibrarySystemModels.Helpers;
using LibrarySystemModels.Services;
using LibrarySystemModels.Models;

namespace Library_System_Management.Views;

public partial class UserInfoWindow : Window, INotifyPropertyChanged
{
    public User SelectedUser { get; }
    private bool _showPassword;
    public ObservableCollection<Report> RecordsHistory { get; set; } = new();

    public UserInfoWindow(User user)
    {
        InitializeComponent();
        SelectedUser = user;
        _showPassword = false;
        DataContext = this;
        Loaded += UserInfoWindow_Loaded;
    }

    private async void UserInfoWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            await RefreshAsync();
        }
        catch (Exception exception)
        {
            MessageBoxService.ShowMessage(exception);
        }
    }

    public bool ShowPassword
    {
        get => _showPassword;
        set
        {
            if (_showPassword == value) return;
            _showPassword = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(DisplayPassword));
        }
    }

    public string DisplayPassword
        => ShowPassword
            ? EncryptionService.DecryptPassword(SelectedUser)
            : new string('●', EncryptionService.DecryptPassword(SelectedUser)?.Length ?? 8);

    private async Task RefreshAsync()
    {
        RecordsHistory.Clear();
        try
        {
            var res = await ReportingService.GetReportsByUserAsync(FlowSide.Client, SelectedUser);
            if (res.ActionResult)
            {
                foreach (var record in res.Data)
                    RecordsHistory.Add(record);
            }
            else
            {
                MessageBox.Show(res.Message,"Warning",MessageBoxButton.OK,MessageBoxImage.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error loading reports", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        OnPropertyChanged(nameof(RecordsHistory));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void BtnExportReports_Click(object sender, RoutedEventArgs e)
    {
        ExportDialog.ExportWindow([.. RecordsHistory]);
    }

    private void BtnExportUser_Click(object sender, RoutedEventArgs e)
    {
        ExportDialog.ExportWindow([SelectedUser]);
    }
}
