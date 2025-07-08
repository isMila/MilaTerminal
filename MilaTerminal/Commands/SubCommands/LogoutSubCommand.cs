using Rocket.API;
using System.Collections.Generic;
using MilaTerminal.Services;
using UnityEngine;

namespace MilaTerminal.Commands.SubCommands
{
    public class LogoutSubCommand : ISubCommand
    {
        public string Name => "logout";
        public string Help => "Closes the current session.";
        public string Syntax => "";
        public List<string> Aliases => new List<string>();

        public void Execute(IRocketPlayer caller, string[] args)
        {
            AuthService.Logout(caller);
            MessagingService.Reply(caller, "logout_success", Color.green);
        }
    }
}