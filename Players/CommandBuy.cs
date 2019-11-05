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
                byte amount = 1;

                if (command.Length == 2 && Convert.ToByte(command[1]) > 0)
                    amount = Convert.ToByte(command[1]);

                // see if item exists in shop
                var item = Toolkit.Instance.ItemList.Find(x => x.ID == Convert.ToUInt16(command[0]));
                
                if (item == null)
                    UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_player_buy_noexist"), Color.red);
                else
                {
                    // check if player can afford item
                    if (Toolkit.Instance.Balances[player.CSteamID.ToString()] >= (item.BuyPrice * amount))
                    {
                        // charge player credits for item(s)
                        Toolkit.Instance.Balances[player.CSteamID.ToString()] = Decimal.Subtract(Toolkit.Instance.Balances[player.CSteamID.ToString()], (item.BuyPrice * amount));

                        player.GiveItem(Convert.ToUInt16(command[0]), amount);
                        UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_player_buy", item.Name, String.Format("{0:C}", item.BuyPrice)), Color.green);
                    }
                    else
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
