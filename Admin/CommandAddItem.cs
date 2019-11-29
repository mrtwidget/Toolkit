using System;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace NEXIS.Toolkit.Admin
{
    public class CommandAddItem : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public bool AllowFromConsole => true;

        public string Name => "additem";

        public string Help => "Add new item to Shop";

        public List<string> Aliases => new List<string>() { };

        public string Syntax => "/additem <id> <name> <buyprice> <sellprice>";

        public List<string> Permissions => new List<string>() { "additem" };

        #endregion

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length == 4)
            {
                // create new warp
                var i = new Items();
                i.ID = Convert.ToUInt16(command[0]);
                i.Name = command[1];
                i.BuyPrice = Convert.ToDecimal(command[2]);
                i.SellPrice = Convert.ToDecimal(command[3]);

                Toolkit.Instance.ItemList.Add(i);

                UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_admin_item_added", i.ID, i.Name, String.Format("{0:C}", i.BuyPrice), String.Format("{0:C}", i.SellPrice)), Color.green);
            }
            else
            {
                UnturnedChat.Say(caller, Help, Color.white);
                UnturnedChat.Say(caller, "Syntax: " + Syntax, Color.yellow);
            }
        }
    }
}
