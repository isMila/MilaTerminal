using Rocket.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using MilaTerminal.Services;
using UnityEngine;

namespace MilaTerminal.Commands.SubCommands
{
    public class ZipSubCommand : ISubCommand
    {
        public string Name => "zip";
        public string Help => "Compresses a folder into a .zip file to create a backup.";
        public string Syntax => "<source_folder> <destination_file.zip>";
        public List<string> Aliases => new List<string> { "compress" };

        public void Execute(IRocketPlayer caller, string[] args)
        {
            if (args.Length != 2)
            {
                MessagingService.Reply(caller, "syntax_error", Color.yellow, Name, Syntax);
                return;
            }

            string sourceFolder = Path.GetFullPath(Path.Combine(MilaTerminal.CurrentWorkingDirectory, args[0]));
            string destFile = Path.GetFullPath(Path.Combine(MilaTerminal.CurrentWorkingDirectory, args[1]));

            if (!Directory.Exists(sourceFolder))
            {
                MessagingService.Reply(caller, "error_not_found", Color.red, args[0]);
                return;
            }

            try
            {
                MessagingService.Reply(caller, "zip_compressing", Path.GetFileName(sourceFolder));
                ZipFile.CreateFromDirectory(sourceFolder, destFile);
                MessagingService.Reply(caller, "zip_success", Color.green);
            }
            catch (Exception)
            {
                MessagingService.Reply(caller, "zip_fail", Color.red);
            }
        }
    }
}