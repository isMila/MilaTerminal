using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiteDB;
using MilaTerminal.Models;

namespace MilaTerminal.Data.Providers
{
    public class LiteDbDataProvider : IDataProvider, IDisposable
    {
        private LiteDatabase database;
        private ILiteCollection<UserData> users;

        public void Initialize(string pluginDirectory)
        {
            string dbPath = Path.Combine(pluginDirectory, "MilaTerminal.db");
            database = new LiteDatabase(dbPath);
            users = database.GetCollection<UserData>("users");
        }

        public UserData GetUser(string username) => users.FindOne(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        public List<UserData> GetAllUsers() => users.FindAll().ToList();
        public bool UserExists(string username) => users.Exists(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        public void CreateUser(UserData user)
        {
            users.EnsureIndex(x => x.Username, true);
            users.Insert(user);
        }

        public void UpdateUser(UserData user)
        {
            users.Update(user);
        }

        public bool DeleteUser(string username)
        {
            return users.DeleteMany(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)) > 0;
        }
        public void Dispose()
        {
            database?.Dispose();
        }
    }
}