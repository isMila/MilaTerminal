using Rocket.API;
using System;
using System.Collections.Generic;
using System.IO;
using MilaTerminal.Services;
using UnityEngine;

namespace MilaTerminal.Commands.SubCommands
{
    public class DelFileSubCommand : ISubCommand
    {
        public string Name => "delfile";
        public string Help => "Deletes a specific file.";
        public string Syntax => "<file_name>";
        public List<string> Aliases => new List<string> { "rm" };

        public void Execute(IRocketPlayer caller, string[] args)
        {
            if (args.Length != 1)
            {
                MessagingService.Reply(caller, "syntax_error", Color.yellow, Name, Syntax);
                return;
            }

            string targetFile = Path.GetFullPath(Path.Combine(MilaTerminal.CurrentWorkingDirectory, args[0]));

            if (targetFile.StartsWith(MilaTerminal.Instance.Directory, StringComparison.OrdinalIgnoreCase))
            {
                MessagingService.Reply(caller, "file_protected", Color.red);
                return;
            }

            if (!targetFile.StartsWith(MilaTerminal.ServerRootPath, StringComparison.OrdinalIgnoreCase))
            {
                MessagingService.Reply(caller, "security_error_path", Color.red);
                return;
            }

            if (Directory.Exists(targetFile))
            {
                MessagingService.Reply(caller, "error_file_is_folder", Color.red, args[0]);
                return;
            }

            if (!File.Exists(targetFile))
            {
                MessagingService.Reply(caller, "error_not_found", Color.red, args[0]);
                return;
            }

            try
            {
                File.Delete(targetFile);
                MessagingService.Reply(caller, "delete_file_success", Color.green, Path.GetFileName(targetFile));
            }
            catch (Exception)
            {
                MessagingService.Reply(caller, "delete_file_fail", Color.red);
            }
        }
    }
}