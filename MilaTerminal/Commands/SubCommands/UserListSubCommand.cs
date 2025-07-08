using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using MilaTerminal.Models;
using MilaTerminal.Services;
using UnityEngine;

namespace MilaTerminal.Commands.SubCommands
{
    public class UserListSubCommand : ISubCommand
    {
        public string Name => "userlist";
        public string Help => "Shows a list of all registered users.";
        public string Syntax => "";
        public List<string> Aliases => new List<string> { "users" };

        public void Execute(IRocketPlayer caller, string[] args)
        {
            var session = AuthService.GetSession(caller);
            if (session == null)
            {
                MessagingService.Reply(caller, "error_get_current_user", Color.red);
                return;
            }

            List<UserData> allUsers = AuthService.GetAllUsers();

            MessagingService.Reply(caller, "user_list_header", Color.cyan);

            bool isRoot = session.Username.Equals("root", StringComparison.OrdinalIgnoreCase);

            foreach (var user in allUsers)
            {
                if (isRoot)
                {
                    string permissions = user.AllowedCommands.Contains("*")
                        ? MilaTerminal.Instance.Translate("user_list_perms_all")
                        : (user.AllowedCommands.Any() ? string.Join(", ", user.AllowedCommands) : MilaTerminal.Instance.Translate("user_list_perms_none"));

                    MessagingService.Reply(caller, "user_list_format", user.Username, permissions);
                }
                else
                {
                    MessagingService.Reply(caller, "user_list_format_simple", user.Username);
                }
            }
            MessagingService.Reply(caller, "user_list_footer", Color.cyan);
        }
    }
}