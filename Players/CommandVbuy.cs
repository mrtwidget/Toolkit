using System;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace NEXIS.Toolkit.Players
{
    public class CommandVbuy : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public bool AllowFromConsole => false;

        public string Name => "vbuy";

        public string Help => "Buy a vehicle that spawns to your location";

        public List<string> Aliases => new List<string>() { };

        public string Syntax => "/vbuy <id>";

        public List<string> Permissions => new List<string>() { "vbuy" };

        #endregion

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length == 1 && Toolkit.Instance.IsDigits(command[0]))
            {
                // search for vehicle in shop by input id
                var item = Toolkit.Instance.VehicleList.Find(x => x.ID == Convert.ToUInt16(command[0]));

                // if vehicle has not been added to the shop
                if (item == null)
                {
                    UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_player_buy_noexist"), Color.red);
                    return;
                }

                // check if player can afford vehicle
                if (Toolkit.Instance.Balances[player.CSteamID.ToString()] >= item.BuyPrice)
                {
                    // give player the vehicle and charge them
                    player.GiveVehicle(Convert.ToUInt16(command[0]));
                    Toolkit.Instance.Balances[player.CSteamID.ToString()] = Decimal.Subtract(Toolkit.Instance.Balances[player.CSteamID.ToString()], item.BuyPrice);

                    player.TriggerEffect(81); // money effect
                    UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_player_buy", item.Name, String.Format("{0:C}", item.BuyPrice), 1), Color.green);
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
