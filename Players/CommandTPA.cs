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
    public class CommandTPA : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public bool AllowFromConsole => false;

        public string Name => "tpa";

        public string Help => "Teleport to another player location";

        public List<string> Aliases => new List<string>() { };

        public string Syntax => "/tpa <name|accept|deny>";

        public List<string> Permissions => new List<string>() { "tpa" };

        #endregion

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (!Toolkit.Instance.Configuration.Instance.EnableTPA)
            {
                UnturnedChat.Say(player, Toolkit.Instance.Translations.Instance.Translate("toolkit_tpa_disabled"), Color.red);
                return;
            }

            if (command.Length == 1)
            {
                if (command[0].ToLower() == "accept" || command[0].ToLower() == "a")
                {
                    TPA request = Toolkit.Instance.TPArequests.Find(x => x.PlayerAccepting == player.CSteamID);

                    if (request == null)
                    {
                        UnturnedChat.Say(player, Toolkit.Instance.Translations.Instance.Translate("toolkit_tpa_request_null"), Color.red);
                        return;
                    }

                    if (Toolkit.Instance.Configuration.Instance.EnableTPATimeout && (DateTime.Now - request.TimeRequested).TotalSeconds > Toolkit.Instance.Configuration.Instance.TPATimeout)
                    {
                        Toolkit.Instance.TPArequests.Remove(request);
                        UnturnedChat.Say(player, Toolkit.Instance.Translations.Instance.Translate("toolkit_tpa_request_null"), Color.red);
                        return;
                    }

                    UnturnedPlayer p = UnturnedPlayer.FromCSteamID(request.PlayerRequesting);
                    p.Teleport(player);
                    p.TriggerEffect(147); // electricity effect

                    Toolkit.Instance.TPArequests.Remove(request);

                    UnturnedChat.Say(player, Toolkit.Instance.Translations.Instance.Translate("toolkit_tpa_request_accept_caller", p.CharacterName), Color.green);
                    UnturnedChat.Say(p, Toolkit.Instance.Translations.Instance.Translate("toolkit_tpa_request_accept", player.CharacterName), Color.green);

                    return;
                }

                if (command[0].ToLower() == "deny" || command[0].ToLower() == "d")
                {
                    TPA request = Toolkit.Instance.TPArequests.Find(x => x.PlayerAccepting == player.CSteamID);

                    if (request == null)
                    {
                        UnturnedChat.Say(player, Toolkit.Instance.Translations.Instance.Translate("toolkit_tpa_request_null"), Color.red);
                        return;
                    }

                    if (Toolkit.Instance.Configuration.Instance.EnableTPATimeout && (DateTime.Now - request.TimeRequested).TotalSeconds > Toolkit.Instance.Configuration.Instance.TPATimeout)
                    {
                        Toolkit.Instance.TPArequests.Remove(request);
                        UnturnedChat.Say(player, Toolkit.Instance.Translations.Instance.Translate("toolkit_tpa_request_null"), Color.red);
                        return;
                    }

                    UnturnedPlayer p = UnturnedPlayer.FromCSteamID(request.PlayerRequesting);
                    Toolkit.Instance.TPArequests.Remove(request);

                    UnturnedChat.Say(player, Toolkit.Instance.Translations.Instance.Translate("toolkit_tpa_request_deny_caller", p.CharacterName), Color.red);
                    UnturnedChat.Say(p, Toolkit.Instance.Translations.Instance.Translate("toolkit_tpa_request_deny", player.CharacterName), Color.red);

                    return;
                }

                // remove previous request if one exists
                TPA existing = Toolkit.Instance.TPArequests.Find(x => x.PlayerRequesting == player.CSteamID);
                if (existing != null)
                    Toolkit.Instance.TPArequests.Remove(existing);

                foreach (SteamPlayer plr in Provider.clients)
                {
                    UnturnedPlayer p = UnturnedPlayer.FromSteamPlayer(plr);

                    if (p.CharacterName.ToLower().Contains(command[0].ToLower()))
                    {
                        var t = new TPA();
                        t.PlayerRequesting = player.CSteamID;
                        t.PlayerAccepting = p.CSteamID;
                        t.TimeRequested = DateTime.Now;

                        Toolkit.Instance.TPArequests.Add(t);

                        UnturnedChat.Say(player, Toolkit.Instance.Translations.Instance.Translate("toolkit_tpa_request_caller", p.CharacterName), Color.green);
                        UnturnedChat.Say(p, Toolkit.Instance.Translations.Instance.Translate("toolkit_tpa_request", player.CharacterName), Color.green);

                        return;
                    }
                }

                UnturnedChat.Say(player, Toolkit.Instance.Translations.Instance.Translate("toolkit_tpa_player_null"), Color.red);
            }
            else
            {
                UnturnedChat.Say(caller, Help, Color.white);
                UnturnedChat.Say(caller, "Syntax: " + Syntax, Color.yellow);
            }
        }
    }
}
