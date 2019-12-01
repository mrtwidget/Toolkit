using System;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using SDG.Unturned;
using UnityEngine;

namespace NEXIS.Toolkit
{
    public class CommandEditItem : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public bool AllowFromConsole => true;

        public string Name => "edititem";

        public string Help => "Edit an existing item in the shop";

        public List<string> Aliases => new List<string>() { "ei" };

        public string Syntax => "/edititem <id> <name> <buyprice> <sellprice>";

        public List<string> Permissions => new List<string>() { "edititem" };

        #endregion

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length == 4)
            {
                // search for item by input id
                var item = Toolkit.Instance.ItemList.Find(x => x.ID == Convert.ToUInt16(command[0]));

                // if item does not exist
                if (item == null)
                {
                    UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_admin_item_noexist"), Color.red);
                    return;
                }

                // remove existing item
                Toolkit.Instance.ItemList.Remove(item);

                // edit the values and re-add the modified item
                item.Name = command[1];
                item.BuyPrice = Convert.ToDecimal(command[2]);
                item.SellPrice = Convert.ToDecimal(command[3]);
                Toolkit.Instance.ItemList.Add(item);

                UnturnedChat.Say(player, Toolkit.Instance.Translations.Instance.Translate("toolkit_admin_item_updated", item.Name, item.ID, item.BuyPrice, item.SellPrice), Color.green);
            }
            else
            {
                UnturnedChat.Say(caller, Help, Color.white);
                UnturnedChat.Say(caller, "Syntax: " + Syntax, Color.yellow);
            }
        }
    }
}