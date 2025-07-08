using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using MilaTerminal.Models;
using MilaTerminal.Services;
using UnityEngine;

namespace MilaTerminal.Commands.SubCommands
{
    public class UserModSubCommand : ISubCommand
    {
        public string Name => "usermod";
        public string Help => "Modifies a user's permissions (root only).";
        public string Syntax => "<user> <add|remove|set> <comma_separated_permissions>";
        public List<string> Aliases => new List<string>();

        public void Execute(IRocketPlayer caller, string[] args)
        {
            var session = AuthService.GetSession(caller);
            var user = AuthService.GetUserFromSession(session);

            if (user?.Username.Equals("root", StringComparison.OrdinalIgnoreCase) == false)
            {
                MessagingService.Reply(caller, "usermod_denied", Color.red);
                return;
            }

            if (args.Length != 3)
            {
                MessagingService.Reply(caller, "syntax_error", Color.yellow, Name, Syntax);
                MessagingService.Reply(caller, ">> Example: /mroot usermod webadmin add zip,delfile", Color.yellow);
                return;
            }

            string userToModify = args[0];
            string mode = args[1];
            List<string> permissions = args[2].Split(',').ToList();

            if (userToModify.Equals("root", StringComparison.OrdinalIgnoreCase))
            {
                MessagingService.Reply(caller, "usermod_no_root", Color.red);
                return;
            }

            if (AuthService.ModifyUserPermissions(userToModify, mode, permissions))
            {
                MessagingService.Reply(caller, "usermod_success", Color.green, userToModify);
            }
            else
            {
                MessagingService.Reply(caller, "usermod_fail", Color.red, userToModify);
            }
        }
    }
}