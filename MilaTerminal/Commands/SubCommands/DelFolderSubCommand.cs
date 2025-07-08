using Rocket.API;
using System;
using System.Collections.Generic;
using System.IO;
using MilaTerminal.Services;
using UnityEngine;

namespace MilaTerminal.Commands.SubCommands
{
    public class DelfolderSubCommand : ISubCommand
    {
        public string Name => "delf";
        public string Help => "Deletes a folder and all its contents. USE WITH CAUTION!";
        public string Syntax => "<folder_name>";
        public List<string> Aliases => new List<string> { "rmdir" };

        public void Execute(IRocketPlayer caller, string[] args)
        {
            if (args.Length != 1)
            {
                MessagingService.Reply(caller, "syntax_error", Color.yellow, Name, Syntax);
                return;
            }

            string targetDirectory = Path.GetFullPath(Path.Combine(MilaTerminal.CurrentWorkingDirectory, args[0]));

            if (targetDirectory.Equals(MilaTerminal.Instance.Directory, StringComparison.OrdinalIgnoreCase))
            {
                MessagingService.Reply(caller, "folder_protected", Color.red);
                return;
            }

            if (!targetDirectory.StartsWith(MilaTerminal.ServerRootPath, StringComparison.OrdinalIgnoreCase))
            {
                MessagingService.Reply(caller, "security_error_path", Color.red);
                return;
            }

            string rocketFolder = Path.Combine(MilaTerminal.ServerRootPath, "Rocket");
            if (targetDirectory.Equals(MilaTerminal.ServerRootPath, StringComparison.OrdinalIgnoreCase) ||
                targetDirectory.Equals(rocketFolder, StringComparison.OrdinalIgnoreCase))
            {
                MessagingService.Reply(caller, "folder_critical", Color.red, args[0]);
                return;
            }

            if (!Directory.Exists(targetDirectory))
            {
                MessagingService.Reply(caller, "error_not_found", Color.red, args[0]);
                return;
            }

            try
            {
                MessagingService.Reply(caller, "delete_folder_warning", Color.yellow, Path.GetFileName(targetDirectory));

                Directory.Delete(targetDirectory, true);

                MessagingService.Reply(caller, "delete_folder_success", Color.green, Path.GetFileName(targetDirectory));
            }
            catch (Exception)
            {
                MessagingService.Reply(caller, "delete_folder_fail", Color.red);
            }
        }
    }
}