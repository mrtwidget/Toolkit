using System;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace NEXIS.Toolkit.Players
{
    public class CommandCost : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public bool AllowFromConsole => false;

        public string Name => "cost";

        public string Help => "Return the buy price of an item";

        public List<string> Aliases => new List<string>() { };

        public string Syntax => "/cost <id>";

        public List<string> Permissions => new List<string>() { "cost" };

        #endregion

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length == 1 && Toolkit.Instance.IsDigits(command[0]))
            {
                var item = Toolkit.Instance.ItemList.Find(x => x.ID == Convert.ToUInt16(command[0]));

                if (item == null)
                    UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_player_buy_noexist"), Color.red);
                else
                    UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_player_buy_cost", item.ID, item.Name, String.Format("{0:C}", item.BuyPrice), String.Format("{0:C}", item.SellPrice)), Color.green);
            }
            else
            {
                UnturnedChat.Say(caller, Help, Color.white);
                UnturnedChat.Say(caller, "Syntax: " + Syntax, Color.yellow);
            }
        }
    }
}
