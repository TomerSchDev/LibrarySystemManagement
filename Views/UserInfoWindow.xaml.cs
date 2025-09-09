using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using Library_System_Management.Models;
using Library_System_Management.Models.ViewModels;
using Library_System_Management.Services;

namespace Library_System_Management.Views;

public partial class UserInfoWindow : Window, INotifyPropertyChanged
{
    public User SelectedUser { get; }

    private bool _showPassword;
    private INotifyPropertyChanged _notifyPropertyChangedImplementation;
    
    
    public ObservableCollection<Report>? RecordsHistory { get; set; }
    public UserInfoWindow(User user)
    {
        InitializeComponent();
        SelectedUser = user;
        _showPassword = false;
        DataContext = this;
        Refresh();
        
    }
    public bool ShowPassword
    {
        get => _showPassword;
        set
        {
            if (_showPassword == value) return;
            _showPassword = value;
            OnPropertyChanged(null);
            OnPropertyChanged(nameof(DisplayPassword));
        }
    }

    private void Refresh()
    {
        RecordsHistory = null;
        RecordsHistory = new ObservableCollection<Report>(ReportingService.GetReportsByUser(SelectedUser));
    }

    public string DisplayPassword => ShowPassword ? SelectedUser.Password : new string('●', SelectedUser.Password?.Length ?? 8);

  

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void BtnExportReports_Click(object sender, RoutedEventArgs e)
    {
        if (RecordsHistory != null) ExportDialog.ExportWindow([..RecordsHistory]);
    }

    private void BtnExportUser_Click(object sender, RoutedEventArgs e)
    {
        ExportDialog.ExportWindow([SelectedUser]);
    }
}