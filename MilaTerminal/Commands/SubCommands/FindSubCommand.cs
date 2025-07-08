using Rocket.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MilaTerminal.Services;
using UnityEngine;

namespace MilaTerminal.Commands.SubCommands
{
    public class FindSubCommand : ISubCommand
    {
        public string Name => "find";
        public string Help => "Recursively searches for files from the current directory.";
        public string Syntax => "<search_pattern>";
        public List<string> Aliases => new List<string> { "search" };

        public void Execute(IRocketPlayer caller, string[] args)
        {
            if (args.Length != 1)
            {
                MessagingService.Reply(caller, "syntax_error", Color.yellow, Name, Syntax);
                return;
            }

            string pattern = args[0];
            string searchPath = MilaTerminal.CurrentWorkingDirectory;

            try
            {
                var output = new StringBuilder();
                output.AppendLine(MilaTerminal.Instance.Translate("find_searching", pattern, searchPath));

                string[] files = Directory.GetFiles(searchPath, pattern, SearchOption.AllDirectories);

                if (files.Length == 0)
                {
                    output.Append(MilaTerminal.Instance.Translate("find_none_found"));
                }
                else
                {
                    output.AppendLine(MilaTerminal.Instance.Translate("find_found_header", files.Length));
                    foreach (var file in files)
                    {
                        string relativePath = file.Substring(searchPath.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                        output.AppendLine(MilaTerminal.Instance.Translate("find_found_item", relativePath));
                    }
                }
                MessagingService.Reply(caller, output.ToString());
            }
            catch (Exception)
            {
                MessagingService.Reply(caller, "error_generic", Color.red);
            }
        }
    }
}