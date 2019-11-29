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
                // amount to buy
                byte amount = 1;
                if (command.Length == 2 && Convert.ToByte(command[1]) > 0)
                    amount = Convert.ToByte(command[1]);

                // search for item in shop by input id
                var item = Toolkit.Instance.ItemList.Find(x => x.ID == Convert.ToUInt16(command[0]));

                // if item has not been added to the shop
                if (item == null)
                {
                    UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_player_buy_noexist"), Color.red);
                    return;
                }

                // check if player can afford item (* amount)
                if (Toolkit.Instance.Balances[player.CSteamID.ToString()] >= (item.BuyPrice * amount))
                {
                    // give player the item and charge them
                    player.GiveItem(Convert.ToUInt16(command[0]), amount);
                    Toolkit.Instance.Balances[player.CSteamID.ToString()] = Decimal.Subtract(Toolkit.Instance.Balances[player.CSteamID.ToString()], (item.BuyPrice * amount));
                    
                    player.TriggerEffect(81); // money effect
                    UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_player_buy", item.Name, String.Format("{0:C}", item.BuyPrice * amount), amount), Color.green);
                }
                else
                {
                    // player cannot afford
                    player.TriggerEffect(45); // explode effect lol
                    UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_player_buy_insufficient_credits"), Color.red);
                }
            }
            else
            {
                UnturnedChat.Say(caller, Help, Color.white);
                UnturnedChat.Say(caller, "Syntax: " + Syntax, Color.yellow);
            }
        }
    }
}
