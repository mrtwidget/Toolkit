using System;
using System.Linq;
using Rocket.API;
using SDG.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace NEXIS.Toolkit.Players
{
    public class CommandPay : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public bool AllowFromConsole => false;

        public string Name => "pay";

        public string Help => "Gives another player some of your credits";

        public List<string> Aliases => new List<string>() { };

        public string Syntax => "/pay <name> <amount>";

        public List<string> Permissions => new List<string>() { "pay" };

        #endregion

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (!Toolkit.Instance.Configuration.Instance.PlayerPayEnabled)
            {
                UnturnedChat.Say(player, Toolkit.Instance.Translations.Instance.Translate("toolkit_pay_disabled"), Color.red);
                return;
            }

            if (command.Length == 2)
            {
                decimal amount = 0;
                amount = Convert.ToDecimal(command[1]);

                if (amount == 0)
                {
                    UnturnedChat.Say(player, Toolkit.Instance.Translations.Instance.Translate("toolkit_pay_player_amount"), Color.red);
                    return;
                }

                if (Toolkit.Instance.Balances[player.CSteamID.ToString()] < amount)
                {
                    UnturnedChat.Say(player, Toolkit.Instance.Translations.Instance.Translate("toolkit_pay_credits"), Color.red);
                    return;
                }

                if (player.CharacterName.ToLower().Contains(command[0].ToLower()))
                {
                    UnturnedChat.Say(player, Toolkit.Instance.Translations.Instance.Translate("toolkit_pay_yourself"), Color.red);
                    return;
                }

                foreach (SteamPlayer plr in Provider.clients)
                {
                    UnturnedPlayer p = UnturnedPlayer.FromSteamPlayer(plr);

                    if (p.CharacterName.ToLower().Contains(command[0].ToLower()))
                    {
                        // remove credits from caller
                        Toolkit.Instance.Balances[player.CSteamID.ToString()] = Decimal.Subtract(Toolkit.Instance.Balances[player.CSteamID.ToString()], amount);
                        // pay the other player
                        Toolkit.Instance.Balances[p.CSteamID.ToString()] = Decimal.Add(Toolkit.Instance.Balances[p.CSteamID.ToString()], amount);

                        if (Toolkit.Instance.Configuration.Instance.UIBalanceEnabled)
                        {
                            Toolkit.Instance.UpdateUI(player);
                            Toolkit.Instance.UpdateUI(p);
                        }

                        player.TriggerEffect(81); // money effect
                        p.TriggerEffect(81); // money effect

                        UnturnedChat.Say(player, Toolkit.Instance.Translations.Instance.Translate("toolkit_pay_giver", p.CharacterName, String.Format("{0:C}", amount)), Color.green);
                        UnturnedChat.Say(p, Toolkit.Instance.Translations.Instance.Translate("toolkit_pay_receiver", player.CharacterName, String.Format("{0:C}", amount)), Color.green);

                        return;
                    }
                }

                UnturnedChat.Say(player, Toolkit.Instance.Translations.Instance.Translate("toolkit_pay_player_null"), Color.red);
            }
            else
            {
                UnturnedChat.Say(caller, Help, Color.white);
                UnturnedChat.Say(caller, "Syntax: " + Syntax, Color.yellow);
            }
        }
    }
}
