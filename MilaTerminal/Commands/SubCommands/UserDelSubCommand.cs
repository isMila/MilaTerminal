using Rocket.API;
using System;
using System.Collections.Generic;
using MilaTerminal.Models;
using MilaTerminal.Services;
using UnityEngine;

namespace MilaTerminal.Commands.SubCommands
{
    public class UserDelSubCommand : ISubCommand
    {
        public string Name => "userdel";
        public string Help => "Deletes a user (root only).";
        public string Syntax => "<user_to_delete>";
        public List<string> Aliases => new List<string> { "userremove" };

        public void Execute(IRocketPlayer caller, string[] args)
        {
            var session = AuthService.GetSession(caller);
            var user = AuthService.GetUserFromSession(session);

            if (user?.Username.Equals("root", StringComparison.OrdinalIgnoreCase) == false)
            {
                MessagingService.Reply(caller, "userdel_denied", Color.red);
                return;
            }

            if (args.Length != 1)
            {
                MessagingService.Reply(caller, "syntax_error", Color.yellow, Name, Syntax);
                return;
            }

            string userToDelete = args[0];

            if (session.Username.Equals(userToDelete, StringComparison.OrdinalIgnoreCase))
            {
                MessagingService.Reply(caller, "userdel_no_self", Color.red);
                return;
            }

            if (userToDelete.Equals("root", StringComparison.OrdinalIgnoreCase))
            {
                MessagingService.Reply(caller, "userdel_no_root", Color.red);
                return;
            }

            if (AuthService.DeleteUser(caller, userToDelete))
            {
                MessagingService.Reply(caller, "userdel_success", Color.green, userToDelete);
            }
            else
            {
                MessagingService.Reply(caller, "userdel_fail", Color.red, userToDelete);
            }
        }
    }
}