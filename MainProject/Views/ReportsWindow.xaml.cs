using System.Collections.ObjectModel;
using System.Windows;
using Library_System_Management.Models;
using Library_System_Management.Services;

namespace Library_System_Management.Views;

public partial class ReportsWindow : Window
{
    public ReportsWindow()
    {
        InitializeComponent();
        DgReports.ItemsSource=new ObservableCollection<Report>(ReportingService.GetReportsWithPermission());
        LoadReports();
        DataContext = this;
    }

    private void LoadReports()
    {
        DgReports.ItemsSource = null;
        DgReports.ItemsSource=new ObservableCollection<Report>(ReportingService.GetReportsWithPermission());
        DataContext = this;
    }
}