using System;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using SDG.Unturned;
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
                // amount to sell
                byte amount = 1;
                if (command.Length == 2 && Convert.ToByte(command[1]) > 0)
                    amount = Convert.ToByte(command[1]);

                // convert input string to ushort
                ushort inputId = Convert.ToUInt16(command[0]);

                // search inventory for item id
                List<InventorySearch> search = player.Inventory.search(inputId, true, true);

                if (search != null)
                {
                    // search for item in shop by input id
                    var item = Toolkit.Instance.ItemList.Find(x => x.ID == inputId);

                    // if item has not been added to the shop
                    if (item == null)
                    {
                        UnturnedChat.Say(player, Toolkit.Instance.Translations.Instance.Translate("toolkit_sell_noexist_db"), Color.red);
                        return;
                    }

                    // if amount trying to be sold exceeds total items found
                    if (amount > search.Count)
                    {
                        UnturnedChat.Say(player, Toolkit.Instance.Translations.Instance.Translate("toolkit_sell_amount"), Color.red);
                        return;
                    }

                    // loop through all found items
                    for (int i = 0; i < search.Count; i++)
                    {
                        if (i >= amount)
                            break;

                        // remove the item and pay the player
                        player.Inventory.removeItem(search[i].page, player.Inventory.getIndex(search[i].page, search[i].jar.x, search[i].jar.y));
                        Toolkit.Instance.Balances[player.CSteamID.ToString()] = Decimal.Add(Toolkit.Instance.Balances[player.CSteamID.ToString()], item.SellPrice);
                    }

                    player.TriggerEffect(81); // money effect
                    UnturnedChat.Say(player, Toolkit.Instance.Translations.Instance.Translate("toolkit_sell", item.Name, String.Format("{0:C}", item.SellPrice * amount), amount), Color.green);
                
                }
                else
                    UnturnedChat.Say(player, Toolkit.Instance.Translations.Instance.Translate("toolkit_sell_noexist"), Color.red);
            }
            else
            {
                UnturnedChat.Say(caller, Help, Color.white);
                UnturnedChat.Say(caller, "Syntax: " + Syntax, Color.yellow);
            }
        }
    }
}
