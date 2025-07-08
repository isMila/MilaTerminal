using Rocket.API;
using System;
using System.Collections.Generic;
using System.IO;
using MilaTerminal.Services;
using UnityEngine;

namespace MilaTerminal.Commands.SubCommands
{
    public class MkdirSubCommand : ISubCommand
    {
        public string Name => "mkdir";
        public string Help => "Creates a new directory in the current location.";
        public string Syntax => "<folder_name>";
        public List<string> Aliases => new List<string> { "md" };

        public void Execute(IRocketPlayer caller, string[] args)
        {
            if (args.Length != 1)
            {
                MessagingService.Reply(caller, "syntax_error", Color.yellow, Name, Syntax);
                return;
            }

            string newDirectoryPath = Path.GetFullPath(Path.Combine(MilaTerminal.CurrentWorkingDirectory, args[0]));

            if (!newDirectoryPath.StartsWith(MilaTerminal.ServerRootPath, StringComparison.OrdinalIgnoreCase))
            {
                MessagingService.Reply(caller, "security_error_path", Color.red);
                return;
            }

            if (Directory.Exists(newDirectoryPath) || File.Exists(newDirectoryPath))
            {
                MessagingService.Reply(caller, "mkdir_exists", Color.red, args[0]);
                return;
            }

            try
            {
                Directory.CreateDirectory(newDirectoryPath);
                MessagingService.Reply(caller, "mkdir_success", Color.green, Path.GetFileName(newDirectoryPath));
            }
            catch (Exception)
            {
                MessagingService.Reply(caller, "mkdir_fail", Color.red);
            }
        }
    }
}