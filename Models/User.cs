using Library_System_Management.Models;



namespace Library_System_Management.Models
{
    public enum UserRole
    {
        Member = 0,
        Librarian = 1,
        Admin = 2,
    
    }

    public class User() : IExportable
    {
        public User(string username, string password) : this()
        {
            Username = username;
            Password = password;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != typeof(User)) return false;
            var user = (User)obj;
            return Username == user.Username && Password == user.Password;
        }


        public bool IsUser(User user)
        {
            return this.Equals(user);
        }

        
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public UserRole Role { get; set; } // Admin or Member

        public string UserRoleToString => Role.ToString();

        public string ExportClassName => "User";
    }
}