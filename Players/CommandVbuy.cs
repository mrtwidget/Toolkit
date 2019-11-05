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
                // see if vehicle exists in shop
                var item = Toolkit.Instance.VehicleList.Find(x => x.ID == Convert.ToUInt16(command[0]));

                if (item == null)
                    UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_player_buy_noexist"), Color.red);
                else
                {
                    // check if player can afford vehicle
                    if (Toolkit.Instance.Balances[player.CSteamID.ToString()] >= item.BuyPrice)
                    {
                        // charge player credits for vehicle
                        Toolkit.Instance.Balances[player.CSteamID.ToString()] = Decimal.Subtract(Toolkit.Instance.Balances[player.CSteamID.ToString()], item.BuyPrice);

                        player.GiveVehicle(Convert.ToUInt16(command[0]));
                        UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_player_buy", item.Name, String.Format("{0:C}", item.BuyPrice), 1), Color.green);
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
