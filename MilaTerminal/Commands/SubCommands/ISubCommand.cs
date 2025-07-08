using Rocket.API;
using System.Collections.Generic;

namespace MilaTerminal.Commands.SubCommands
{
    public interface ISubCommand
    {
        string Name { get; }
        string Help { get; }
        string Syntax { get; }
        List<string> Aliases { get; }
        void Execute(IRocketPlayer caller, string[] args);
    }
}