using System.Windows;
using Library_System_Management.Database;
using Library_System_Management.Models;
using Microsoft.Data.Sqlite;

namespace Library_System_Management.Services
{
    public static class AuthService
    {
        public static User? Login(string username, string password)
        {
            var loginUser = new User(username, password);
            if (loginUser == null) throw new ArgumentNullException(nameof(loginUser));
            var users = DatabaseManager.SelectAll<User>();
            return users.FirstOrDefault(user => user.IsUser(loginUser));
        }

        public static List<User> GetUsers()
        {
            return DatabaseManager.SelectAll<User>();
        }

        public static bool CreateNewUser(User newUser)
        {
            var users = DatabaseManager.SelectAll<User>();
            var tmpUser = users.FirstOrDefault(user => user.Username == newUser.Username);
            if (tmpUser != null)
            {
                MessageBox.Show("User with that username already exists! , reporting action", "User already exists",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                ReportingService.ReportEvent(SeverityLevel.LOW,
                    "Trying to add new user with existing username : " + newUser.Username);
                return false;
            }

            if (!SessionHelperService.IsEnoughPermission(newUser.Role))
            {
                MessageBox.Show("can't add new user with more permission of current user! , reporting action", "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                ReportingService.ReportEvent(SeverityLevel.MEDIUM,
                    "Trying to add new user with more permission of current user : " + newUser.Username);
                return false;
            }
            DatabaseManager.Insert(newUser);
            MessageBox.Show("User added successfully", "action complete",
                MessageBoxButton.OK, MessageBoxImage.Information);
            ReportingService.ReportEvent(SeverityLevel.INFO,$"User {newUser.Username} added successfully with role {newUser.UserRoleToString}");
            return true;
        }

        public static User? GetUserById(int id)
        {
            var users = DatabaseManager.SelectAll<User>();
            var user = users.FirstOrDefault(u => u.UserID == id);
            if (user == null) return null;
            if (SessionHelperService.IsEnoughPermission(user.Role)) return user;
            MessageBox.Show("Cant get User with more Permission then current user , reporting action","Warning",MessageBoxButton.OK,MessageBoxImage.Warning);
            ReportingService.ReportEvent(SeverityLevel.MEDIUM,"Trying to get User with more Permission then current user ");
            return null;

        }
    }
}