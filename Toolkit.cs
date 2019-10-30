using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Rocket.Unturned;
using Rocket.Core.Plugins;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using Rocket.Unturned.Chat;
using Rocket.API;
using Rocket.API.Collections;
using SDG.Unturned;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Logger = Rocket.Core.Logging.Logger;
using Rocket.Core.Steam;
using Steamworks;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using System.Xml;
using System.Threading;

namespace NEXIS.Toolkit
{
    public class Toolkit : RocketPlugin<ToolkitConfiguration>
    {
        #region Fields

        public static Toolkit Instance;

        #endregion

        #region Overrides

        protected override void Load()
        {
            Instance = this;
            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected += Events_OnPlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerDeath += Events_OnPlayerDeath;
            UnturnedPlayerEvents.OnPlayerChatted += Events_OnPlayerChatted;
            UnturnedPlayerEvents.OnPlayerUpdateStat += Events_OnPlayerUpdateStat;
            Logger.Log("Loaded!", ConsoleColor.DarkGreen);
        }

        protected override void Unload()
        {
            U.Events.OnPlayerConnected -= Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected -= Events_OnPlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerDeath -= Events_OnPlayerDeath;
            UnturnedPlayerEvents.OnPlayerChatted -= Events_OnPlayerChatted;
            UnturnedPlayerEvents.OnPlayerUpdateStat -= Events_OnPlayerUpdateStat;
            Logger.Log("Unloaded!", ConsoleColor.Yellow);
        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList() {
                    {"toolkit_disabled", "Toolkit is currently unavailable"},
                    {"toolkit_death_bleeding", "{0} couldn't find a medkit and bled to death. Everyone is disappointed."},
                    {"toolkit_death_zombie", "{0} his {1} ripped off and was beaten with it by a zombie!"},
                    {"toolkit_death_vehicle", "{0}'s vehicle exploded and so did his body. He's dead."},
                    {"toolkit_death_sentry", "A sentry blew a hole in {0}'s {1} and died pooping himself."},
                    {"toolkit_death_landmine", "{0} stepped on a landmine and blew his {1} into pieces! BRUH"},
                    {"toolkit_death_grenade", "A grenade blew off {0}'s {1} and everyone laughed! xD"}
                };
            }
        }

        #endregion

        #region Events

        public void Events_OnPlayerConnected(UnturnedPlayer player)
        {

        }

        public void Events_OnPlayerDisconnected(UnturnedPlayer player)
        {

        }

        public void Events_OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            switch (cause)
            {
                case EDeathCause.BLEEDING:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_bleeding", player.CharacterName), Color.red);
                    break;
                case EDeathCause.ZOMBIE:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_zombie", player.CharacterName, ReturnLimb(limb)), Color.red);
                    break;
                case EDeathCause.VEHICLE:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_vehicle", player.CharacterName), Color.red);
                    break;
                case EDeathCause.SENTRY:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_sentry", player.CharacterName, ReturnLimb(limb)), Color.red);
                    break;
                case EDeathCause.LANDMINE:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_landmine", player.CharacterName, ReturnLimb(limb)), Color.red);
                    break;
                case EDeathCause.GRENADE:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_grenade", player.CharacterName, ReturnLimb(limb)), Color.red);
                    break;
                case EDeathCause.:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_grenade", player.CharacterName, ReturnLimb(limb)), Color.red);
                    break;
            }
        }

        public void Events_OnPlayerChatted(UnturnedPlayer player, ref Color color, string message, EChatMode chatMode, ref bool cancel)
        {

        }

        public void Events_OnPlayerUpdateStat(UnturnedPlayer player, EPlayerStat stat)
        {

        }

        #endregion

        public void FixedUpdate()
        {
            if (Instance.State != PluginState.Loaded) return;

        }

        public string ReturnLimb(ELimb limb)
        {
            switch (limb)
            {
                case ELimb.LEFT_ARM:
                    return "left arm";
                case ELimb.LEFT_BACK:
                case ELimb.RIGHT_BACK:
                    return "back";
                case ELimb.LEFT_FOOT:
                    return "left foot";
                case ELimb.RIGHT_FOOT:
                    return "right foot";
                case ELimb.LEFT_FRONT:
                case ELimb.RIGHT_FRONT:
                    return "chest";
                case ELimb.LEFT_HAND:
                    return "left hand";
                case ELimb.RIGHT_HAND:
                    return "right hand";
                case ELimb.LEFT_LEG:
                    return "left leg";
                case ELimb.RIGHT_LEG:
                    return "right leg";
                case ELimb.SKULL:
                    return "head";
                case ELimb.SPINE:
                    return "spine";
                default:
                    return "somewhere";
            }
        }
    }
}
