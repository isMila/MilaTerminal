using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MilaTerminal.Data;
using MilaTerminal.Data.Providers;
using MilaTerminal.Models;
using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;

namespace MilaTerminal.Services
{
    public static class AuthService
    {
        private static IDataProvider dataProvider;
        private static Dictionary<CSteamID, PlayerSession> playerSessions;
        private static readonly TimeSpan sessionTimeout = TimeSpan.FromMinutes(5);

        public static void Initialize()
        {
            string providerType = MilaTerminal.Instance.Configuration.Instance.DatabaseProvider?.ToLower() ?? "litedb";
            if (providerType == "json")
            {
                dataProvider = new JsonDataProvider();
            }
            else
            {
                dataProvider = new LiteDbDataProvider();
            }

            dataProvider.Initialize(MilaTerminal.Instance.Directory);
            playerSessions = new Dictionary<CSteamID, PlayerSession>();

            if (!dataProvider.UserExists("root"))
            {
                CreateUser("root", "mila123", new List<string> { "*" });
            }

            MessagingService.Reply(new ConsolePlayer(), "auth_service_initialized", providerType);
        }

        private static void HashPassword(string password, out string hash, out string salt)
        {
            byte[] saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            salt = Convert.ToBase64String(saltBytes);

            using (var rfc2898 = new Rfc2898DeriveBytes(password, saltBytes, 10000))
            {
                byte[] hashBytes = rfc2898.GetBytes(20);
                hash = Convert.ToBase64String(hashBytes);
            }
        }

        private static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            byte[] saltBytes = Convert.FromBase64String(storedSalt);
            using (var rfc2898 = new Rfc2898DeriveBytes(password, saltBytes, 10000))
            {
                byte[] hashBytes = rfc2898.GetBytes(20);
                string newHash = Convert.ToBase64String(hashBytes);
                return newHash == storedHash;
            }
        }

        public static bool Login(IRocketPlayer caller, string username, string password)
        {
            var user = dataProvider.GetUser(username);
            if (user == null || !VerifyPassword(password, user.PasswordHash, user.Salt))
            {
                return false;
            }

            var session = new PlayerSession(user.Username);
            CSteamID key = (caller is ConsolePlayer || caller == null) ? CSteamID.Nil : ((UnturnedPlayer)caller).CSteamID;

            if (playerSessions.ContainsKey(key))
            {
                playerSessions[key] = session;
            }
            else
            {
                playerSessions.Add(key, session);
            }
            return true;
        }

        public static void Logout(IRocketPlayer caller)
        {
            CSteamID key = (caller is ConsolePlayer || caller == null) ? CSteamID.Nil : ((UnturnedPlayer)caller).CSteamID;
            if (playerSessions.ContainsKey(key))
            {
                playerSessions.Remove(key);
            }
        }

        public static PlayerSession GetSession(IRocketPlayer caller)
        {
            CSteamID key = (caller is ConsolePlayer || caller == null) ? CSteamID.Nil : ((UnturnedPlayer)caller).CSteamID;
            if (!playerSessions.TryGetValue(key, out PlayerSession session))
            {
                return null;
            }

            if (DateTime.UtcNow - session.LastActivityTime > sessionTimeout)
            {
                MessagingService.Reply(caller, "session_expired");
                playerSessions.Remove(key);
                return null;
            }

            session.LastActivityTime = DateTime.UtcNow;
            return session;
        }

        public static UserData GetUserFromSession(PlayerSession session)
        {
            if (session == null) return null;
            return dataProvider.GetUser(session.Username);
        }

        public static bool HasPermissionFor(IRocketPlayer caller, string command)
        {
            var session = GetSession(caller);
            if (session == null) return false;

            var user = GetUserFromSession(session);
            if (user == null) return false;

            if (user.AllowedCommands.Contains("*")) return true;

            var basePerms = new[] { "logout", "passwd", "help", "userlist" };
            if (basePerms.Contains(command, StringComparer.OrdinalIgnoreCase)) return true;

            return user.AllowedCommands.Contains(command, StringComparer.OrdinalIgnoreCase);
        }

        public static List<UserData> GetAllUsers() => dataProvider.GetAllUsers();

        public static bool CreateUser(string username, string password, List<string> permissions)
        {
            if (dataProvider.UserExists(username))
            {
                return false;
            }
            HashPassword(password, out string hash, out string salt);
            var newUser = new UserData
            {
                Username = username,
                PasswordHash = hash,
                Salt = salt,
                AllowedCommands = permissions ?? new List<string>()
            };
            dataProvider.CreateUser(newUser);
            return true;
        }

        public static bool ChangePassword(string username, string newPassword)
        {
            var user = dataProvider.GetUser(username);
            if (user == null) return false;

            HashPassword(newPassword, out string hash, out string salt);
            user.PasswordHash = hash;
            user.Salt = salt;
            dataProvider.UpdateUser(user);
            return true;
        }

        public static bool DeleteUser(IRocketPlayer callerOfDelete, string usernameToDelete)
        {
            if (usernameToDelete.Equals("root", StringComparison.OrdinalIgnoreCase)) return false;

            bool deleted = dataProvider.DeleteUser(usernameToDelete);
            if (deleted)
            {
                foreach (var session in playerSessions.ToList())
                {
                    if (session.Value.Username.Equals(usernameToDelete, StringComparison.OrdinalIgnoreCase))
                    {
                        playerSessions.Remove(session.Key);
                    }
                }
            }
            return deleted;
        }
        public static void Shutdown()
        {
            (dataProvider as IDisposable)?.Dispose();
            MessagingService.Reply(new ConsolePlayer(), "Authentication service shut down.");
        }
        public static bool ModifyUserPermissions(string username, string mode, List<string> permissions)
        {
            var user = dataProvider.GetUser(username);
            if (user == null || user.Username.Equals("root", StringComparison.OrdinalIgnoreCase)) return false;

            switch (mode.ToLower())
            {
                case "add":
                    user.AllowedCommands = user.AllowedCommands.Union(permissions, StringComparer.OrdinalIgnoreCase).ToList();
                    break;
                case "remove":
                    user.AllowedCommands = user.AllowedCommands.Except(permissions, StringComparer.OrdinalIgnoreCase).ToList();
                    break;
                case "set":
                    user.AllowedCommands = permissions;
                    break;
                default:
                    return false;
            }

            dataProvider.UpdateUser(user);
            return true;
        }
    }
}