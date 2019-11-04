using System;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace NEXIS.Toolkit.Players
{
    public class CommandBuy : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public bool AllowFromConsole => false;

        public string Name => "buy";

        public string Help => "Buy an item that spawns in your inventory";

        public List<string> Aliases => new List<string>() { };

        public string Syntax => "/buy <id> [amount]";

        public List<string> Permissions => new List<string>() { "buy" };

        #endregion

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length >= 1 && Toolkit.Instance.IsDigits(command[0]))
            {
                if (command.Length == 1 || !Toolkit.Instance.IsDigits(command[1]))
                    command[1] = "1";

                player.GiveItem(Convert.ToUInt16(command[0]), Convert.ToByte(command[1]));
                UnturnedChat.Say(caller, "Item added to your inventory!", Color.green);
            }
            else
            {
                UnturnedChat.Say(caller, Help, Color.white);
                UnturnedChat.Say(caller, "Syntax: " + Syntax, Color.yellow);
            }
        }
    }
}
