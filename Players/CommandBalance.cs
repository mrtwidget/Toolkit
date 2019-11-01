using System;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace NEXIS.Toolkit
{
    public class CommandBalance : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public bool AllowFromConsole => false;

        public string Name => "balance";

        public string Help => "View your credit balance";

        public List<string> Aliases => new List<string>() { };

        public string Syntax => "/balance";

        public List<string> Permissions => new List<string>() { "balance" };

        #endregion

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length > 0)
                UnturnedChat.Say(caller, Help + ": " + Syntax, Color.gray);
            else
            {
                player.TriggerEffect(81); // money effect
                UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_player_balance", String.Format("{0:C}", Toolkit.Instance.Balances[player.CSteamID.ToString()])), Color.green);
            }
        }
    }
}