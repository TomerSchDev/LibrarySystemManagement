using System.Windows;
using Library_System_Management.Database;
using Library_System_Management.Models;
using Library_System_Management.Views;

namespace Library_System_Management.Services;

public static class SessionHelperService
{
    private static User _user;

    static SessionHelperService()
    {
        var initialized = false;
        var currentApp = Application.Current;
        if (currentApp != null)
        {
            var dashBoard=currentApp.Windows.OfType<DashboardWindow>().FirstOrDefault();
            if (dashBoard != null)
            {
                _user = dashBoard.CurrentUser;
                initialized = true;
            }
        }

        if (initialized) return;
        _user = new User("TestAdmin","TestAdmin",UserRole.Admin);
        DatabaseManager.Insert(_user);
    }
   

    public static User? GetCurrentUser()
    {
        return _user;
    }
    public static bool IsEnoughPermission(UserRole minPermission)
    {
        return _user.Role >= minPermission;
    }
}