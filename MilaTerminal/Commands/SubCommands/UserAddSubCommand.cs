using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using MilaTerminal.Models;
using MilaTerminal.Services;
using UnityEngine;

namespace MilaTerminal.Commands.SubCommands
{
    public class UserAddSubCommand : ISubCommand
    {
        public string Name => "useradd";
        public string Help => "Creates a new user (root only).";
        public string Syntax => "<user> <password> [comma_separated_permissions]";
        public List<string> Aliases => new List<string>();

        public void Execute(IRocketPlayer caller, string[] args)
        {
            var session = AuthService.GetSession(caller);
            var user = AuthService.GetUserFromSession(session);

            if (user?.Username.Equals("root", StringComparison.OrdinalIgnoreCase) == false)
            {
                MessagingService.Reply(caller, "useradd_denied", Color.red);
                return;
            }

            if (args.Length < 2)
            {
                MessagingService.Reply(caller, "syntax_error", Color.yellow, Name, Syntax);
                MessagingService.Reply(caller, ">> Example: /mroot useradd webadmin 12345 ls,cd,cat", Color.yellow);
                return;
            }

            string newUser = args[0];
            string newPass = args[1];

            List<string> permissions = args.Length > 2 ? args[2].Split(',').ToList() : new List<string>();

            if (AuthService.CreateUser(newUser, newPass, permissions))
            {
                MessagingService.Reply(caller, "useradd_success", Color.green, newUser);
            }
            else
            {
                MessagingService.Reply(caller, "useradd_exists", Color.red, newUser);
            }
        }
    }
}