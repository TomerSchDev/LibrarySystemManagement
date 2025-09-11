using System.Windows;

namespace Library_System_Management.ExportServices;

public static class MessageBoxService
{
    public static void ShowMessage(Exception ex)
    {
        MessageBox.Show(ex.Message,"Error",MessageBoxButton.OK,MessageBoxImage.Error);
    }
}