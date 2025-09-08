using System.Windows;
using Library_System_Management.Models;
using Library_System_Management.Views;

namespace Library_System_Management.Services;

public static class SessionHelperService
{
    private static UserRole? GetActiveUser()
    {
        var dashboard = Application.Current.Windows
            .OfType<DashboardWindow>()
            .FirstOrDefault();

        return dashboard?.CurrentUser.Role;
    }

    public static User? GetCurrentUser()
    {
        var dashboard = Application.Current.Windows
            .OfType<DashboardWindow>()
            .FirstOrDefault();
        return dashboard?.CurrentUser;
    }
    public static bool IsEnoughPermission(UserRole minPermission)
    {
        return GetActiveUser() >= minPermission;
    }
}