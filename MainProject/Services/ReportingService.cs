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
        DatabaseManager.Insert(new Report(severityLevel, user.Username,user.UserID, user.Role, DateTime.Now, message));
    }

    private static List<Report> GetReports()
    {
        return DatabaseManager.SelectAll<Report>();
    }

    public static List<Report> GetReportsByUser(User user)
    {
        return GetReports().Where(r => r.UserId == user.UserID).ToList();
    }
    public static List<Report> GetReportsOfCurrentUser()
    {
        var user = SessionHelperService.GetCurrentUser();
        return user == null ? [] : GetReports().Where(r => r.UserId == user.UserID).ToList();
    }
    public static List<Report> GetReportsWithPermission()
    {
        var user = SessionHelperService.GetCurrentUser();
        
        return user == null ? [] : GetReports().Where(r =>SessionHelperService.IsEnoughPermission(r.Role)).ToList();
    }
}