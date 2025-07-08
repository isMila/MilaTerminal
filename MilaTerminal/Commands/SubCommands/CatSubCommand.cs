using Rocket.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MilaTerminal.Services;
using UnityEngine;

namespace MilaTerminal.Commands.SubCommands
{
    public class CatSubCommand : ISubCommand
    {
        public string Name => "cat";
        public string Help => "Displays the content of a text file.";
        public string Syntax => "<file>";
        public List<string> Aliases => new List<string> { "type" };

        private const int MaxLinesToShow = 100;

        public void Execute(IRocketPlayer caller, string[] args)
        {
            if (args.Length != 1)
            {
                MessagingService.Reply(caller, "syntax_error", Color.yellow, Name, Syntax);
                return;
            }

            string targetFile = Path.GetFullPath(Path.Combine(MilaTerminal.CurrentWorkingDirectory, args[0]));

            if (!targetFile.StartsWith(MilaTerminal.ServerRootPath, StringComparison.OrdinalIgnoreCase))
            {
                MessagingService.Reply(caller, "security_error_path", Color.red);
                return;
            }

            if (!File.Exists(targetFile))
            {
                MessagingService.Reply(caller, "error_not_found", Color.red, args[0]);
                return;
            }

            try
            {
                var output = new StringBuilder();
                output.AppendLine(MilaTerminal.Instance.Translate("cat_header", Path.GetFileName(targetFile)));

                string[] lines = File.ReadAllLines(targetFile);

                for (int i = 0; i < lines.Length; i++)
                {
                    if (i >= MaxLinesToShow)
                    {
                        output.AppendLine(MilaTerminal.Instance.Translate("cat_too_long", MaxLinesToShow));
                        break;
                    }
                    output.AppendLine(lines[i]);
                }

                output.Append(MilaTerminal.Instance.Translate("cat_footer"));
                MessagingService.Reply(caller, output.ToString());
            }
            catch (Exception)
            {
                MessagingService.Reply(caller, "cat_fail", Color.red);
            }
        }
    }
}