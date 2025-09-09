using System.Windows;
using Library_System_Management.Helpers;
using Library_System_Management.Models;
using Library_System_Management.Services;
using Library_System_Management.Services.ExportServices;

namespace Library_System_Management.Views;

public partial class ExportDialog : Window
{
    private readonly IEnumerable<IExportable> _data;

    private string FileName => txtFileName.Text.Trim();
    private string? SelectedExportService => cbExportTypes.SelectedItem as string;
    public static void ExportWindow(List<IExportable> data)
    {
        var exportWindow = new ExportDialog(data);
        exportWindow.ShowDialog();
    }
    
    public ExportDialog(IEnumerable<IExportable> data)
    {
        InitializeComponent();
        _data = data;
        cbExportTypes.ItemsSource = ExporterService.GetAllExportServices();
        cbExportTypes.SelectedIndex = 0;
        txtFileName.Text=_data.First().ExportClassName+"s";
    }

   
    private void BtnExport_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(FileName))
        {
            MessageBox.Show("Please enter a file name.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (SelectedExportService != null) ExporterService.Export(_data, FileName, SelectedExportService);
        MessageBox.Show("Data exported successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        DialogResult = true;
    }
}
