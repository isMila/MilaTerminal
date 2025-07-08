using System;

namespace MilaTerminal.Models
{
    public class PlayerSession
    {
        public string Username { get; set; }
        public DateTime LastActivityTime { get; set; }

        public PlayerSession(string username)
        {
            Username = username;
            LastActivityTime = DateTime.UtcNow;
        }
    }
}