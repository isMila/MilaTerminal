using Rocket.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MilaTerminal.Services;
using UnityEngine;

namespace MilaTerminal.Commands.SubCommands
{
    public class LsSubCommand : ISubCommand
    {
        public string Name => "ls";
        public string Help => "Lists the contents of the current directory or a specified path.";
        public string Syntax => "[optional_path]";
        public List<string> Aliases => new List<string> { "dir" };

        public void Execute(IRocketPlayer caller, string[] args)
        {
            try
            {
                string targetPath = (args.Length == 0)
                    ? MilaTerminal.CurrentWorkingDirectory
                    : Path.GetFullPath(Path.Combine(MilaTerminal.CurrentWorkingDirectory, args[0]));

                if (!targetPath.StartsWith(MilaTerminal.ServerRootPath, StringComparison.OrdinalIgnoreCase))
                {
                    MessagingService.Reply(caller, "security_error_path", Color.red);
                    return;
                }

                if (!Directory.Exists(targetPath))
                {
                    MessagingService.Reply(caller, "error_not_found", Color.red, targetPath);
                    return;
                }

                var output = new StringBuilder();
                output.AppendLine(MilaTerminal.Instance.Translate("ls_header", targetPath));

                string[] directories = Directory.GetDirectories(targetPath);
                foreach (string dir in directories)
                {
                    output.AppendLine(MilaTerminal.Instance.Translate("ls_folder", Path.GetFileName(dir)));
                }

                string[] files = Directory.GetFiles(targetPath);
                foreach (string file in files)
                {
                    output.AppendLine(MilaTerminal.Instance.Translate("ls_file", Path.GetFileName(file)));
                }

                if (directories.Length == 0 && files.Length == 0)
                {
                    output.AppendLine(MilaTerminal.Instance.Translate("ls_empty"));
                }

                output.Append(MilaTerminal.Instance.Translate("ls_footer"));

                MessagingService.Reply(caller, output.ToString());
            }
            catch (Exception)
            {
                MessagingService.Reply(caller, "error_generic", Color.red);
            }
        }
    }
}