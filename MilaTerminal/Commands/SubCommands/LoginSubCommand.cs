using Rocket.API;
using System.Collections.Generic;
using MilaTerminal.Services;
using UnityEngine;

namespace MilaTerminal.Commands.SubCommands
{
    public class LoginSubCommand : ISubCommand
    {
        public string Name => "login";
        public string Help => "Logs into MilaTerminal.";
        public string Syntax => "<user> <password>";
        public List<string> Aliases => new List<string>();

        public void Execute(IRocketPlayer caller, string[] args)
        {
            if (args.Length != 2)
            {
                MessagingService.Reply(caller, "syntax_error", Color.yellow, Name, Syntax);
                return;
            }

            if (AuthService.Login(caller, args[0], args[1]))
            {
                MessagingService.Reply(caller, "login_success", Color.green, args[0]);
            }
            else
            {
                MessagingService.Reply(caller, "login_invalid_credentials", Color.red);
            }
        }
    }
}