using System;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using SDG.Unturned;
using UnityEngine;

namespace NEXIS.Toolkit
{
    public class CommandAddKit : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public bool AllowFromConsole => false;

        public string Name => "addkit";

        public string Help => "Create a new kit based on all items you currently have";

        public List<string> Aliases => new List<string>() { };

        public string Syntax => "/addkit <name> [cost]";

        public List<string> Permissions => new List<string>() { "addkit" };

        #endregion

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length >= 1)
            {
                decimal cost = 1;
                if (command.Length == 2 && Convert.ToDecimal(command[1]) > 0.00m)
                    cost = Convert.ToDecimal(command[1]);

                // search for kit by input name
                var kitName = Toolkit.Instance.KitList.Find(x => x.Name == command[0]);

                // if kit already exists with same name
                if (kitName != null)
                {
                    UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_kit_exists"), Color.red);
                    return;
                }

                // create new kit object
                Kits kit = new Kits();
                kit.Name = command[0];
                kit.Cost = cost;
                kit.Items = new List<KitItem>() { };

                // save inventory list as new kit
                // TODO :: this should probably be simplified using InventorySearch
                for (byte page = 0; page < PlayerInventory.PAGES; page++)
                {
                    if (page == 7)
                        break;

                    var count = player.Inventory.getItemCount(page);

                    for (byte index = 0; index < count; index++)
                    {
                        Item item = new Item(player.Player.inventory.getItem(page, index).item.id, false);
                        byte[] metadata = player.Inventory.getItem(page, index).item.metadata;

                        string md = null;
                        for (var data = 0; data < item.metadata.Length; data++)
                        {
                            md += metadata[data];
                            if ((data + 1) != item.metadata.Length)
                                md += ",";
                        }

                        kit.Items.Add(new KitItem(item.id, 1, md));
                    }
                }

                // add new kit to KitList
                Toolkit.Instance.KitList.Add(kit);
                UnturnedChat.Say(player, "Added kit " + kit.Name + ", costing " + String.Format("{0:C}", kit.Cost) + " credits!");
            }
            else
            {
                UnturnedChat.Say(caller, Help, Color.white);
                UnturnedChat.Say(caller, "Syntax: " + Syntax, Color.yellow);
            }
        }
    }
}