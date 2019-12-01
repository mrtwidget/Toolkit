using System;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using SDG.Unturned;
using UnityEngine;

namespace NEXIS.Toolkit
{
    public class CommandDelItem : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public bool AllowFromConsole => true;

        public string Name => "delitem";

        public string Help => "Delete an existing item in the shop";

        public List<string> Aliases => new List<string>() { };

        public string Syntax => "/delitem <id>";

        public List<string> Permissions => new List<string>() { "delitem" };

        #endregion

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length == 1)
            {
                // search for item by input id
                var item = Toolkit.Instance.ItemList.Find(x => x.ID == Convert.ToUInt16(command[0]));

                // if item id does not exist
                if (item == null)
                {
                    UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_admin_item_noexist"), Color.red);
                    return;
                }

                Toolkit.Instance.ItemList.Remove(item);
                UnturnedChat.Say(player, Toolkit.Instance.Translations.Instance.Translate("toolkit_admin_item_deleted", item.Name, item.ID), Color.green);
            }
            else
            {
                UnturnedChat.Say(caller, Help, Color.white);
                UnturnedChat.Say(caller, "Syntax: " + Syntax, Color.yellow);
            }
        }
    }
}