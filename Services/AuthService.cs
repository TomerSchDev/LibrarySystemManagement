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
        
    }
}