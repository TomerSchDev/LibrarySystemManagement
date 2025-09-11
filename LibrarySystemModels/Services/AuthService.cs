using LibrarySystemModels.Helpers;
using LibrarySystemModels.Models;

namespace LibrarySystemModels.Services
{
    public static class AuthService
    {
        private const string AuthServicePath = "api/Auth/";
        private const string UserServicePath = "api/Users/";

        public static async Task<User> LoginAsync(FlowSide flowSide, string username, string password)
        {
            if (flowSide == FlowSide.Client)
            {
                var result = await DataBaseService.Login(username, password);
                return result.ActionResult ? result.Data : User.DefaultUser;
            }

            var user = await GetUserByUsernameAsync(FlowSide.Server, username);
            Console.WriteLine($"User {user.Username} found.");
            var res = EncryptionService.VerifyHash(user, password);
            Console.WriteLine($"Username {user.Username}  with password {password} is {res}");
            return res? user: User.DefaultUser;;
        }

        public static async Task<User> GetUserByUsernameAsync(FlowSide flowSide, string username)
        {
            if (flowSide == FlowSide.Client)
            {
                var url = UserServicePath + "search?username=" + username;
                var result = await DataBaseService.Get<User>(url);
                return result.Data;
            }

            var users = await Task.Run(() => DataBaseService.GetLocalDatabase().SelectAll<User>());
            if (users.Count != 0) return users.FirstOrDefault(u => u.Username == username) ?? User.DefaultUser;

            var user = await SeedDefaultAdminAsync();
            users.Add(user);
            return users.FirstOrDefault(u => u.Username == username)??User.DefaultUser;
        }

        private static async Task<User> SeedDefaultAdminAsync()
        {
            var defUser = CreateUser("Admin", "admin123", UserRole.Admin);
            Console.WriteLine($"User created: {defUser.Username} in seed defaults");
            await Task.Run(() => DataBaseService.GetLocalDatabase().Insert(defUser));
            return defUser;
        }

        public static async Task<ResultResolver<List<User>>> GetUsersAsync(FlowSide flowSide)
        {
            if (flowSide == FlowSide.Client)
            {
                return await DataBaseService.Get<List<User>>(UserServicePath);
            }

            var users = await Task.Run(() => DataBaseService.GetLocalDatabase().SelectAll<User>());
            return new ResultResolver<List<User>>(users, true, "");
        }

        public static async Task<bool> CreateNewUserAsync(FlowSide flowSide, User newUser)
        {
            if (flowSide == FlowSide.Client)
            {
                var result = await DataBaseService.Insert<User, User>(AuthServicePath+"register", newUser);
                return result.ActionResult;
            }

            var users = await Task.Run(() => DataBaseService.GetLocalDatabase().SelectAll<User>());
            var tmpUser = users.FirstOrDefault(user => user.Username == newUser.Username);
            if (tmpUser != null)
            {
                await ReportingService.ReportEventAsync(flowSide, SeverityLevel.LOW, "Trying to add new user with existing username : " + newUser.Username);
                return false;
            }

            if (!SessionHelperService.IsEnoughPermission(flowSide, newUser.Role))
            {
                await ReportingService.ReportEventAsync(flowSide, SeverityLevel.MEDIUM, "Trying to add new user with more permission of current user : " + newUser.Username);
                return false;
            }

            await Task.Run(() => DataBaseService.GetLocalDatabase().Insert(newUser));
            await ReportingService.ReportEventAsync(flowSide, SeverityLevel.INFO, $"User {newUser.Username} added successfully with role {newUser.Role.ToString()}");
            return true;
        }

        public static async Task<User> GetUserByIdAsync(FlowSide flowSide, int id)
        {
            if (flowSide == FlowSide.Client)
            {
                var result = await DataBaseService.Get<User>(UserServicePath + "search?id=" + id);
                return result.Data;
            }

            var users = await Task.Run(() => DataBaseService.GetLocalDatabase().SelectAll<User>());
            var user = users.FirstOrDefault(u => u.UserID == id);
            if (user == null) return User.DefaultUser;
            return (SessionHelperService.IsEnoughPermission(flowSide, user.Role) ? user : null) ?? User.DefaultUser;
        }

        public static User CreateUser(string username, string password, UserRole role)
        {
            var (hash, salt) = EncryptionService.CreateHash(password);
            return new User(username, EncryptionService.Encrypt(password, EncryptionService.MasterKey), hash, salt, role);
        }
        
    }
}
