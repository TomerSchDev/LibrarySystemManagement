﻿using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Library_System_Management.Models;

namespace Library_System_Management.Database
{
    public static class DatabaseManager
    {
        private static readonly string DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "LibraryDB.sqlite");

        static DatabaseManager()
        {
            var folder = Path.GetDirectoryName(DbPath);
            if (!Directory.Exists(folder) && folder != null)
                Directory.CreateDirectory(folder);
        }

        private static SqliteConnection GetConnection()
        {
            var conn = new SqliteConnection($"Data Source={DbPath}");
            conn.Open();
            return conn;
        }

        // -------------------- Table Initialization --------------------
        public static void InitializeDatabase()
        {
            CreateTable<User>();
            CreateTable<Book>();
            CreateTable<Member>();
            CreateTable<BorrowedBook>();
            CreateTable<Report>();
            SeedDefaultAdmin();
        }

        private static void SeedDefaultAdmin()
        {
            using var conn = GetConnection();
            var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM User;";
            var count = (long)(cmd.ExecuteScalar() ?? 0);
            if (count != 0) return;
            cmd.CommandText = "INSERT INTO User (Username, Password, Role) VALUES (@u, @p, @t);";
            cmd.Parameters.AddWithValue("@u", "admin");
            cmd.Parameters.AddWithValue("@p", "admin123");
            cmd.Parameters.AddWithValue("@t", UserRole.Admin);
            cmd.ExecuteNonQuery();
        }

        // -------------------- Generic CRUD --------------------
        private static void CreateTable<T>() where T : class, new()
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
                    else if (pt == typeof(Enum))
                        colType = "INTEGER";
                    else
                        colType = "TEXT";

                    string extra = p.Name == $"{tableName}ID" ? "PRIMARY KEY AUTOINCREMENT" : "";
                    return $"{p.Name} {colType} {extra}".Trim();
                });

            cmd.CommandText = $"CREATE TABLE IF NOT EXISTS {tableName}({string.Join(",", columns)});";
            cmd.ExecuteNonQuery();
        }

        public static void Insert<T>(T obj) where T : class
        {
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

        public static void Update<T>(T obj) where T : class
        {
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

        public static void Delete<T>(int id) where T : class
        {
            using var conn = GetConnection();
            var tableName = typeof(T).Name;
            var cmd = conn.CreateCommand();
            cmd.CommandText = $"DELETE FROM {tableName} WHERE {tableName}ID=@id;";
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public static List<T> SelectAll<T>() where T : class, new()
        {
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
