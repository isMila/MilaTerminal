using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MilaTerminal.Models;
using Newtonsoft.Json;

namespace MilaTerminal.Data.Providers
{
    public class JsonDataProvider : IDataProvider
    {
        private string usersFilePath;
        private List<UserData> userCache;

        public void Initialize(string pluginDirectory)
        {
            usersFilePath = Path.Combine(pluginDirectory, "Users.json");
            LoadUsers();
        }

        private void LoadUsers()
        {
            if (File.Exists(usersFilePath))
            {
                string json = File.ReadAllText(usersFilePath);
                userCache = JsonConvert.DeserializeObject<List<UserData>>(json) ?? new List<UserData>();
            }
            else
            {
                userCache = new List<UserData>();
            }
        }

        private void SaveChanges()
        {
            string json = JsonConvert.SerializeObject(userCache, Formatting.Indented);
            File.WriteAllText(usersFilePath, json);
        }

        public UserData GetUser(string username) => userCache.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
        public List<UserData> GetAllUsers() => userCache.ToList();
        public bool UserExists(string username) => userCache.Any(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

        public void CreateUser(UserData user)
        {
            userCache.Add(user);
            SaveChanges();
        }

        public void UpdateUser(UserData user)
        {
            SaveChanges();
        }

        public bool DeleteUser(string username)
        {
            var userToRemove = GetUser(username);
            if (userToRemove == null) return false;

            userCache.Remove(userToRemove);
            SaveChanges();
            return true;
        }
    }
}