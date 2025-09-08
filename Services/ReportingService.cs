using System.Windows;
using Library_System_Management.Database;
using Library_System_Management.Models;

namespace Library_System_Management.Services;

public static class ReportingService
{
    public static void ReportEvent(SeverityLevel severityLevel, string message)
    {
        var user = SessionHelperService.GetCurrentUser();
        if (user == null)
        {
            MessageBox.Show("No Active User", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        DatabaseManager.Insert(new Report(severityLevel, user.Username, user.Role.ToString(), DateTime.Now, message));
    }

    public static List<Report> GetReports()
    {
        return DatabaseManager.SelectAll<Report>();
    }
}