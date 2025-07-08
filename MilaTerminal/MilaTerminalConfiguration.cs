using Rocket.API;

namespace MilaTerminal
{
    public class MilaTerminalConfiguration : IRocketPluginConfiguration
    {
        public string DatabaseProvider { get; set; }

        public void LoadDefaults()
        {
            DatabaseProvider = "litedb";
        }
    }
}