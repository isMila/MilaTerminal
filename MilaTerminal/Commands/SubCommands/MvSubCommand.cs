using Rocket.API;
using System;
using System.Collections.Generic;
using System.IO;
using MilaTerminal.Services;
using UnityEngine;

namespace MilaTerminal.Commands.SubCommands
{
    public class MvSubCommand : ISubCommand
    {
        public string Name => "mv";
        public string Help => "Moves or renames a file or folder.";
        public string Syntax => "<source> <destination>";
        public List<string> Aliases => new List<string> { "move", "rename" };

        public void Execute(IRocketPlayer caller, string[] args)
        {
            if (args.Length != 2)
            {
                MessagingService.Reply(caller, "syntax_error", Color.yellow, Name, Syntax);
                return;
            }

            string sourcePath = Path.GetFullPath(Path.Combine(MilaTerminal.CurrentWorkingDirectory, args[0]));
            string destPath = Path.GetFullPath(Path.Combine(MilaTerminal.CurrentWorkingDirectory, args[1]));

            if (!sourcePath.StartsWith(MilaTerminal.ServerRootPath) || !destPath.StartsWith(MilaTerminal.ServerRootPath))
            {
                MessagingService.Reply(caller, "security_error_path", Color.red);
                return;
            }

            try
            {
                if (File.Exists(sourcePath))
                {
                    File.Move(sourcePath, destPath);
                    MessagingService.Reply(caller, "mv_success_file", Color.green, destPath);
                }
                else if (Directory.Exists(sourcePath))
                {
                    Directory.Move(sourcePath, destPath);
                    MessagingService.Reply(caller, "mv_success_folder", Color.green, destPath);
                }
                else
                {
                    MessagingService.Reply(caller, "error_not_found", Color.red, args[0]);
                }
            }
            catch (Exception)
            {
                MessagingService.Reply(caller, "mv_fail", Color.red);
            }
        }
    }
}