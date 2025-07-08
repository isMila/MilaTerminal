using Rocket.API;
using System.Collections.Generic;
using MilaTerminal.Models;
using MilaTerminal.Services;
using UnityEngine;

namespace MilaTerminal.Commands.SubCommands
{
    public class PasswdSubCommand : ISubCommand
    {
        public string Name => "passwd";
        public string Help => "Changes your password.";
        public string Syntax => "<new_password>";
        public List<string> Aliases => new List<string>();

        public void Execute(IRocketPlayer caller, string[] args)
        {
            if (args.Length != 1)
            {
                MessagingService.Reply(caller, "syntax_error", Color.yellow, Name, Syntax);
                return;
            }

            PlayerSession currentSession = AuthService.GetSession(caller);
            if (currentSession == null)
            {
                MessagingService.Reply(caller, "error_get_current_user", Color.red);
                return;
            }

            if (AuthService.ChangePassword(currentSession.Username, args[0]))
            {
                MessagingService.Reply(caller, "password_update_success", Color.green);
            }
            else
            {
                MessagingService.Reply(caller, "password_update_fail", Color.red);
            }
        }
    }
}