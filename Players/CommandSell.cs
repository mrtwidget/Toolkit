using System;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace NEXIS.Toolkit.Players
{
    public class CommandSell : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public bool AllowFromConsole => false;

        public string Name => "sell";

        public string Help => "Sell items in your inventory for credits";

        public List<string> Aliases => new List<string>() { };

        public string Syntax => "/sell <id> [amount]";

        public List<string> Permissions => new List<string>() { "sell" };

        #endregion

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length >= 1 && Toolkit.Instance.IsDigits(command[0]))
            {
                UnturnedChat.Say(caller, "Sorry, this is not functioning yet. :(", Color.red);
            }
            else
            {
                UnturnedChat.Say(caller, Help, Color.white);
                UnturnedChat.Say(caller, "Syntax: " + Syntax, Color.yellow);
            }
        }
    }
}
