using System;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace NEXIS.Toolkit.Players
{
    public class CommandExchange : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public bool AllowFromConsole => false;

        public string Name => "exchange";

        public string Help => "Exchange your experience for credits";

        public List<string> Aliases => new List<string>() { "ex" };

        public string Syntax => "/exchange <xp>";

        public List<string> Permissions => new List<string>() { "exchange" };

        #endregion

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (!Toolkit.Instance.Configuration.Instance.ExchangeEnabled)
            {
                UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_exchange_disabled"), Color.red);
                return;
            }

            if (command.Length == 1 && Toolkit.Instance.IsDigits(command[0]))
            {
                uint exp = Convert.ToUInt32(command[0]);

                if (player.Experience < exp)
                {
                    UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_exchange_experience"), Color.red);
                    return;
                }

                // remove experience from player
                player.Experience = player.Experience - exp;
                // convert xp into credits and payout
                decimal amount = exp / Toolkit.Instance.Configuration.Instance.ExchangeXPPerCredit;
                Toolkit.Instance.Balances[player.CSteamID.ToString()] = Decimal.Add(Toolkit.Instance.Balances[player.CSteamID.ToString()], amount);
                if (Toolkit.Instance.Configuration.Instance.UIBalanceEnabled)
                        Toolkit.Instance.UpdateUI(player);
                player.TriggerEffect(81); // money effect
                UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_exchange_complete", exp, String.Format("{0:C}", amount)), Color.green);
            }
            else
            {
                UnturnedChat.Say(caller, Help, Color.white);
                UnturnedChat.Say(caller, "Syntax: " + Syntax, Color.yellow);
            }
        }
    }
}
