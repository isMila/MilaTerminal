using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace MilaTerminal.Services
{
    public static class MessagingService
    {
        public const string ChatName = "MilaTerminal";
        public const string ChatIconURL = "https://i.imgur.com/LK914gE.png";

        public static void Reply(IRocketPlayer caller, string translationKey, Color color, params object[] args)
        {
            string message = MilaTerminal.Instance.Translate(translationKey, args);

            if (caller is ConsolePlayer || caller == null)
            {
                Logger.Log($"[{ChatName}] {message}");
            }
            else
            {
                string finalMessage = $"<color=#C71585>[{ChatName}]</color> {message}";
                ChatManager.serverSendMessage(finalMessage, color, null, ((UnturnedPlayer)caller).SteamPlayer(), EChatMode.SAY, ChatIconURL, true);
            }
        }

        public static void Reply(IRocketPlayer caller, string translationKey, params object[] args)
        {
            Reply(caller, translationKey, Color.white, args);
        }
    }
}