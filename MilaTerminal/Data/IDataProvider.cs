using System.Collections.Generic;
using MilaTerminal.Models;

namespace MilaTerminal.Data
{
    public interface IDataProvider
    {
        void Initialize(string pluginDirectory);
        UserData GetUser(string username);
        List<UserData> GetAllUsers();
        bool UserExists(string username);
        void CreateUser(UserData user);
        void UpdateUser(UserData user);
        bool DeleteUser(string username);
    }
}