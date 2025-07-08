using Rocket.API;
using System;
using System.Collections.Generic;
using System.IO;
using MilaTerminal.Services;
using UnityEngine;

namespace MilaTerminal.Commands.SubCommands
{
    public class CdSubCommand : ISubCommand
    {
        public string Name => "cd";
        public string Help => "Changes the current working directory.";
        public string Syntax => "<path>";
        public List<string> Aliases => new List<string>();

        public void Execute(IRocketPlayer caller, string[] args)
        {
            if (args.Length != 1)
            {
                MessagingService.Reply(caller, ">> Syntax: /mroot {0} <folder_name> or /mroot {0} ..", Color.yellow, Name);
                return;
            }

            string requestedPath = args[0];
            string newPath = Path.GetFullPath(Path.Combine(MilaTerminal.CurrentWorkingDirectory, requestedPath));

            if (!newPath.StartsWith(MilaTerminal.ServerRootPath, StringComparison.OrdinalIgnoreCase))
            {
                MessagingService.Reply(caller, "security_error_path", Color.red);
                return;
            }

            if (!Directory.Exists(newPath))
            {
                MessagingService.Reply(caller, "error_not_found", Color.red, requestedPath);
                return;
            }

            MilaTerminal.CurrentWorkingDirectory = newPath;
            MessagingService.Reply(caller, "cd_success", Color.cyan, MilaTerminal.CurrentWorkingDirectory);
        }
    }
}