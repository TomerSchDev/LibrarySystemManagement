using System.Collections.ObjectModel;
using System.Windows;
using LibrarySystemModels.Helpers;
using LibrarySystemModels.Services;
using LibrarySystemModels.Models;
using Library_System_Management.ExportServices;

namespace Library_System_Management.Views
{
    public partial class ReportsWindow : Window
    {
        public ObservableCollection<Report> Reports { get; set; } = new();

        public ReportsWindow()
        {
            InitializeComponent();
            DataContext = this;
            Loaded += ReportsWindow_Loaded;
        }

        private async void ReportsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await LoadReportsAsync();
            }
            catch (Exception ex)
            {
                MessageBoxService.ShowMessage(ex);
            }
        }

        private async Task LoadReportsAsync()
        {
            Reports.Clear();
            var result = await ReportingService.GetReportsWithPermissionAsync(FlowSide.Client);
            if (result.ActionResult)
            {
                foreach (var r in result.Data)
                    Reports.Add(r);
            }
            else
            {
                MessageBox.Show("Failed to load reports: " + result.Message,
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            DgReports.ItemsSource = Reports;
        }
    }
}