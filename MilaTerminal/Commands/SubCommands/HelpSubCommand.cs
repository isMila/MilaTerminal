using Rocket.API;
using System.Collections.Generic;
using MilaTerminal.Services;
using UnityEngine;

namespace MilaTerminal.Commands.SubCommands
{
    public class HelpSubCommand : ISubCommand
    {
        public string Name => "help";
        public string Help => "Displays all MilaTerminal commands.";
        public string Syntax => "";
        public List<string> Aliases => new List<string> { "commands" };

        public void Execute(IRocketPlayer caller, string[] args)
        {
            MessagingService.Reply(caller, "help_header", Color.cyan);
            MessagingService.Reply(caller, "help_usage");
            MessagingService.Reply(caller, "-------------------------------------------------", Color.yellow);
            MessagingService.Reply(caller, "help_nav_header", Color.yellow);
            MessagingService.Reply(caller, "help_nav_ls");
            MessagingService.Reply(caller, "help_nav_cd");
            MessagingService.Reply(caller, "help_nav_cat");
            MessagingService.Reply(caller, "help_create_header", Color.yellow);
            MessagingService.Reply(caller, "help_create_mkdir");
            MessagingService.Reply(caller, "help_create_delf");
            MessagingService.Reply(caller, "help_create_delfile");
            MessagingService.Reply(caller, "help_manage_header", Color.yellow);
            MessagingService.Reply(caller, "help_manage_mv");
            MessagingService.Reply(caller, "help_manage_find");
            MessagingService.Reply(caller, "help_manage_zip");
            MessagingService.Reply(caller, "help_footer", Color.yellow);
        }
    }
}