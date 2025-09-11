using System.IO;
using LibrarySystemModels.Helpers;
using LibrarySystemModels.Models;
using Microsoft.Data.Sqlite;

namespace LibrarySystemModels.Database
{
    public class LocalDatabaseHandler : IDbHandler
    {
        private string _dbTestPath =
            FileRetriever.RetrieveFIlePath(Path.Combine("Resources", "LibraryDB.sqlite"));
        private bool _initialized = false;

        private static void SetDir(string dbPath)
        {
            var folder = Path.GetDirectoryName(dbPath);
            if (!Directory.Exists(folder) && folder != null)
                Directory.CreateDirectory(folder);
        }
        
        private SqliteConnection GetConnection()
        {
            var conn = new SqliteConnection($"Data Source={_dbTestPath}");
            conn.Open();
            return conn;
        }

       
        // -------------------- Table Initialization --------------------
        public void InitializeDatabase(string? newPath)
        {
            if (!string.IsNullOrEmpty(newPath)) _dbTestPath =newPath ;
            SetDir(_dbTestPath);
            CreateTable<User>();
            CreateTable<Book>();
            CreateTable<Member>();
            CreateTable<BorrowedBook>();
            CreateTable<Report>();
            this._initialized = true;
        }

        public User? GetCurrentUser()
        {
            throw new NotImplementedException("Only using in Client Side With Rest API");
        }

        public User? Login(string username, string password)
        {
            throw new NotImplementedException("Only using in Client Side With Rest API");
        }

        public string Name => "Local";
        

        // -------------------- Generic CRUD --------------------
        private void CreateTable<T>() where T : new()
        {
            using var conn = GetConnection();
            var cmd = conn.CreateCommand();

            var type = typeof(T);
            var tableName = type.Name;

            var columns = type.GetProperties()
                .Select(p =>
                {
                    string colType;
                    var pt = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;

                    if (pt == typeof(int) || pt == typeof(long))
                        colType = "INTEGER";
                    else if (pt == typeof(string))
                        colType = "TEXT";
                    else if (pt == typeof(bool))
                        colType = "INTEGER";
                    else if (pt == typeof(DateTime))
                        colType = "TEXT";
                    else if (pt.IsEnum)
                        colType = "INTEGER";
                    else
                        colType = "TEXT";

                    var extra = p.Name == $"{tableName}ID" ? "PRIMARY KEY AUTOINCREMENT" : "";
                    return $"{p.Name} {colType} {extra}".Trim();
                });

            cmd.CommandText = $"CREATE TABLE IF NOT EXISTS {tableName}({string.Join(",", columns)});";
            cmd.ExecuteNonQuery();
        }

        public void Insert<T>(T obj)where T : new()
        {
            if (!_initialized)InitializeDatabase(null);
            using var conn = GetConnection();
            var cmd = conn.CreateCommand();
            var type = typeof(T);
            var tableName = type.Name;

            var props = type.GetProperties().Where(p => p.Name != $"{tableName}ID").ToList();

            var columns = string.Join(",", props.Select(p => p.Name));
            var parameters = string.Join(",", props.Select(p => $"@{p.Name}"));

            cmd.CommandText = $"INSERT INTO {tableName} ({columns}) VALUES ({parameters});";

            foreach (var prop in props)
            {
                var val = prop.GetValue(obj) ?? DBNull.Value;
                cmd.Parameters.AddWithValue($"@{prop.Name}", val);
            }

            cmd.ExecuteNonQuery();
        }

        public void Update<T>(T obj)where T : new()
        {
            if (!_initialized)InitializeDatabase(null);
            using var conn = GetConnection();
            var cmd = conn.CreateCommand();
            var type = typeof(T);
            var tableName = type.Name;

            var props = type.GetProperties().Where(p => p.Name != $"{tableName}ID").ToList();
            var setClause = string.Join(",", props.Select(p => $"{p.Name}=@{p.Name}"));

            var idProp = type.GetProperty($"{tableName}ID");
            if (idProp == null)
                throw new Exception($"No {tableName}ID property found!");

            cmd.CommandText = $"UPDATE {tableName} SET {setClause} WHERE {idProp.Name}=@id;";

            foreach (var prop in props)
                cmd.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(obj) ?? DBNull.Value);

            cmd.Parameters.AddWithValue("@id", idProp.GetValue(obj));
            cmd.ExecuteNonQuery();
        }
        public async Task InsertAsync<T>(T obj) where T : new()
        {
            await Task.Run(() => Insert(obj));
        }

        public async Task UpdateAsync<T>(T obj) where T : new()
        {
            await Task.Run(() => Update(obj));
        }

        public async Task DeleteAsync<T>(int id) where T : new()
        {
            await Task.Run(() => Delete<T>(id));
        }

        public async Task<List<T>> SelectAllAsync<T>() where T : new()
        {
            return await Task.Run(() => SelectAll<T>());
        }
        public void Delete<T>(int id) where T : new()
        {
            if (!_initialized)InitializeDatabase(null);
            using var conn = GetConnection();
            var tableName = typeof(T).Name;
            var cmd = conn.CreateCommand();
            cmd.CommandText = $"DELETE FROM {tableName} WHERE {tableName}ID=@id;";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public List<T> SelectAll<T>()  where T:new()
        {
            if (!_initialized)InitializeDatabase(null);
            var result = new List<T>();
            using var conn = GetConnection();
            var type = typeof(T);
            var tableName = type.Name;

            var cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT * FROM {tableName};";

            using var reader = cmd.ExecuteReader();
            var props = type.GetProperties().ToList();

            while (reader.Read())
            {
                var obj = new T();
                foreach (var prop in props)
                {
                    if (!prop.CanWrite) continue;
                    if (reader.IsDBNull(reader.GetOrdinal(prop.Name))) continue;
                    var val = reader[prop.Name];
                    var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                    if (targetType == typeof(int))
                        prop.SetValue(obj, Convert.ToInt32(val));
                    else if (targetType == typeof(long))
                        prop.SetValue(obj, Convert.ToInt64(val));
                    else if (targetType == typeof(bool))
                        prop.SetValue(obj, Convert.ToInt32(val) == 1);
                    else if (targetType == typeof(DateTime))
                        prop.SetValue(obj, DateTime.Parse(val.ToString() ?? string.Empty));
                    else if (targetType == typeof(string))
                        prop.SetValue(obj, val.ToString());
                    else if (targetType.IsEnum)
                        prop.SetValue(obj, Enum.Parse(targetType, val.ToString() ?? string.Empty));
                    else
                        prop.SetValue(obj, Convert.ChangeType(val, targetType));
                }
                result.Add(obj);
            }

            return result;
        }
    }
}
