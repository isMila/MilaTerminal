using LiteDB;
using System.Collections.Generic;

namespace MilaTerminal.Models
{
    public class UserData
    {
        public ObjectId Id { get; set; }

        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public List<string> AllowedCommands { get; set; }
    }
}