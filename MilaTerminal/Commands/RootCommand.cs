using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using MilaTerminal.Commands.SubCommands;
using MilaTerminal.Services;
using UnityEngine;

namespace MilaTerminal.Commands
{
    public class RootCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "mroot";
        public string Help => "Root command for MilaTerminal. Requires authentication.";
        public string Syntax => "<subcommand> [arguments]";
        public List<string> Aliases => new List<string> { "milaterminal", "mt" };
        public List<string> Permissions => new List<string> ();

        private readonly Dictionary<string, ISubCommand> _subCommands;

        public RootCommand()
        {
            _subCommands = new Dictionary<string, ISubCommand>(StringComparer.OrdinalIgnoreCase);

            var subCommandTypes = typeof(MilaTerminal).Assembly.GetTypes()
                .Where(t => t.GetInterface(nameof(ISubCommand)) != null && !t.IsAbstract);

            foreach (var type in subCommandTypes)
            {
                var cmd = (ISubCommand)Activator.CreateInstance(type);
                if (cmd != null)
                {
                    _subCommands.Add(cmd.Name, cmd);
                    foreach (var alias in cmd.Aliases)
                    {
                        _subCommands.Add(alias, cmd);
                    }
                }
            }
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0)
            {
                if (_subCommands.TryGetValue("help", out ISubCommand helpCmd))
                {
                    helpCmd.Execute(caller, new string[0]);
                }
                return;
            }

            string subCommandName = command[0];
            string[] args = command.Skip(1).ToArray();

            var publicCommands = new[] { "login", "help" };
            if (publicCommands.Contains(subCommandName, StringComparer.OrdinalIgnoreCase))
            {
                if (_subCommands.TryGetValue(subCommandName, out ISubCommand publicCmd))
                {
                    publicCmd.Execute(caller, args);
                }
                else
                {
                    MessagingService.Reply(caller, "command_not_found", Color.red, subCommandName);
                }
                return;
            }

            if (AuthService.GetSession(caller) == null)
            {
                MessagingService.Reply(caller, "login_required_prompt_1", Color.yellow);
                MessagingService.Reply(caller, "login_required_prompt_2", Color.yellow);
                return;
            }

            if (!AuthService.HasPermissionFor(caller, subCommandName))
            {
                MessagingService.Reply(caller, "access_denied", Color.red);
                return;
            }

            if (_subCommands.TryGetValue(subCommandName, out ISubCommand privateCmd))
            {
                privateCmd.Execute(caller, args);
            }
            else
            {
                MessagingService.Reply(caller, "command_not_found", Color.red, subCommandName);
            }
        }
    }
}