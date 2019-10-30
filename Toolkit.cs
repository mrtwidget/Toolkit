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
        public Dictionary<CSteamID, decimal> Balances;

        // Custom message colors
        public static Color DeathColor = new Color(50, 0, 200);
        public static Color HeadshotColor = new Color(1, 50, 256);
        public static Color PVPColor = new Color(1, 0, 200);

        #endregion

        #region Overrides

        protected override void Load()
        {
            Instance = this;
            Balances = new Dictionary<CSteamID, decimal>();

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
                    {"toolkit_player_connected", "{0} has connected to the server"},
                    {"toolkit_player_disconnected", "{0} gave up and left the server"},
                    {"toolkit_death_acid", "{0} was covered in acid and melted into a puddle of uncool."},
                    {"toolkit_death_animal", "{0} was mauled to death by a wild animal because he tried to pet it."},
                    {"toolkit_death_bleeding", "{0} couldn't find a medkit and bled to death. Everyone is disappointed."},
                    {"toolkit_death_bones", "{0} died from broken bones. How does that even happen?!"},
                    {"toolkit_death_boulder", "A boulder crushed {0} to death. There's not much left..."},
                    {"toolkit_death_breath", "{0} ran out of oxygen and suffocated to death. Very little was lost."},
                    {"toolkit_death_burning", "{0} burned to death like a marshmallow over a campfire."},
                    {"toolkit_death_charge", "{0} was electrocuted by a charge. Shocking!"},
                    {"toolkit_death_food", "{0} refused to eat his vegetables and died of starvation!"},
                    {"toolkit_death_freezing", "{0} forgot about the Laws of Thermodynamics and froze to death!"},
                    {"toolkit_death_grenade", "A grenade blew off {0}'s {1} and everyone laughed! xD"},
                    {"toolkit_death_gun", "{0} was shot in the {1} by and {2} and died!"},
                    {"toolkit_death_infection", "{0} was infected and became a zombie himself!"},
                    {"toolkit_death_landmine", "{0} stepped on a landmine and blew his {1} into pieces! BRUH"},
                    {"toolkit_death_melee", "{2} hit {0} in the {1} until they died!"},
                    {"toolkit_death_missile", "{0} took a missile to the {1} and was incinerated!"},
                    {"toolkit_death_punch", "{2} punched {0} in the {1} until he died. Brutal!"},
                    {"toolkit_death_roadkill", "{0} was run over by a vehicle {1} was driving!"},
                    {"toolkit_death_sentry", "A sentry blew a hole in {0}'s {1}! SENTRY: 1 {0}: 0"},
                    {"toolkit_death_shred", "{0} was shredded into a million pieces of meat confetti!"},
                    {"toolkit_death_spark", "{0} was sparked to death. WTF DOES THAT MEAN?!"},
                    {"toolkit_death_spit", "{0} was spit on and died of humiliation!"},
                    {"toolkit_death_splash", "{0} got too close and was killed by splash damage!"},
                    {"toolkit_death_suicide", "{0} took the selfish way out and killed himself."},
                    {"toolkit_death_vehicle", "{0}'s vehicle exploded and so did his body. He's dead."},
                    {"toolkit_death_water", "{0} refused to hydrate and became a dried up, stinky corpse."},
                    {"toolkit_death_zombie", "{0} had his {1} ripped off and was beaten with it by a zombie!"},
                    {"toolkit_death_headshot", "{1} headshot {0} with a {2} from a distance of {3}!"},
                    {"toolkit_player_initial_balance", "Welcome, {0}! We have given you {1} credits to get started. Buy something!"},
                    {"toolkit_player_balance", "You have {0} credits"},
                    {"toolkit_player_zombie_kill", "You received {0} credits for killing a Zombie"},
                    {"toolkit_player_mega_zombie_kill", "You received {0} credits for killing a MEGA Zombie!"}
                };
            }
        }

        #endregion

        #region Events

        public void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            UnturnedChat.Say(Translations.Instance.Translate("toolkit_player_connected", player.CharacterName), Color.gray);

            if (!Balances.ContainsKey(player.CSteamID))
            {
                Balances.Add(player.CSteamID, Configuration.Instance.InitialBalance);
                UnturnedChat.Say(player, Translations.Instance.Translate("toolkit_player_initial_balance", player.CharacterName, String.Format("{0:C}", Configuration.Instance.InitialBalance)), Color.yellow);
            }
            else
                UnturnedChat.Say(player, Translations.Instance.Translate("toolkit_player_balance", String.Format("{0:C}", Balances[player.CSteamID])), Color.yellow);
        }

        public void Events_OnPlayerDisconnected(UnturnedPlayer player)
        {
            UnturnedChat.Say(Translations.Instance.Translate("toolkit_player_disconnected", player.CharacterName), Color.black);
        }

        public void Events_OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            // headshot?
            if (cause == EDeathCause.GUN && limb == ELimb.SKULL)
            {
                UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_headshot", player.CharacterName, ReturnMurdererName(murderer), UnturnedPlayer.FromCSteamID(murderer).Player.equipment.asset.itemName.ToString(), ReturnKillDistance(player, murderer)), HeadshotColor);
                return;
            }

            switch (cause)
            {
                case EDeathCause.ACID:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_acid", player.CharacterName), DeathColor);
                    break;
                case EDeathCause.ANIMAL:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_animal", player.CharacterName), DeathColor);
                    break;
                case EDeathCause.BLEEDING:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_bleeding", player.CharacterName), DeathColor);
                    break;
                case EDeathCause.BONES:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_bones", player.CharacterName), DeathColor);
                    break;
                case EDeathCause.BOULDER:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_boulder", player.CharacterName), DeathColor);
                    break;
                case EDeathCause.BREATH:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_breath", player.CharacterName), DeathColor);
                    break;
                case EDeathCause.BURNING:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_burning", player.CharacterName), DeathColor);
                    break;
                case EDeathCause.CHARGE:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_charge", player.CharacterName), DeathColor);
                    break;
                case EDeathCause.FOOD:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_food", player.CharacterName), DeathColor);
                    break;
                case EDeathCause.FREEZING:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_freezing", player.CharacterName), DeathColor);
                    break;
                case EDeathCause.GRENADE:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_grenade", player.CharacterName, ReturnLimb(limb)), DeathColor);
                    break;
                case EDeathCause.GUN:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_gun", player.CharacterName, ReturnLimb(limb), ReturnMurdererName(murderer)), PVPColor);
                    break;
                case EDeathCause.INFECTION:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_infection", player.CharacterName), DeathColor);
                    break;
                case EDeathCause.LANDMINE:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_landmine", player.CharacterName, ReturnLimb(limb)), DeathColor);
                    break;
                case EDeathCause.MELEE:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_melee", player.CharacterName, ReturnLimb(limb), ReturnMurdererName(murderer)), PVPColor);
                    break;
                case EDeathCause.MISSILE:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_missile", player.CharacterName, ReturnLimb(limb)), DeathColor);
                    break;
                case EDeathCause.PUNCH:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_punch", player.CharacterName, ReturnLimb(limb), ReturnMurdererName(murderer)), PVPColor);
                    break;
                case EDeathCause.ROADKILL:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_roadkill", player.CharacterName, ReturnMurdererName(murderer)), PVPColor);
                    break;
                case EDeathCause.SENTRY:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_sentry", player.CharacterName, ReturnLimb(limb)), DeathColor);
                    break;
                case EDeathCause.SHRED:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_shred", player.CharacterName), DeathColor);
                    break;
                case EDeathCause.SPARK:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_spark", player.CharacterName), DeathColor);
                    break;
                case EDeathCause.SPIT:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_spit", player.CharacterName), DeathColor);
                    break;
                case EDeathCause.SPLASH:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_splash", player.CharacterName), DeathColor);
                    break;
                case EDeathCause.SUICIDE:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_suicide", player.CharacterName), DeathColor);
                    break;
                case EDeathCause.VEHICLE:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_vehicle", player.CharacterName), DeathColor);
                    break;
                case EDeathCause.WATER:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_water", player.CharacterName), DeathColor);
                    break;
                case EDeathCause.ZOMBIE:
                    UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_zombie", player.CharacterName, ReturnLimb(limb)), DeathColor);
                    break;
            }
        }

        public void Events_OnPlayerChatted(UnturnedPlayer player, ref Color color, string message, EChatMode chatMode, ref bool cancel)
        {

        }

        public void Events_OnPlayerUpdateStat(UnturnedPlayer player, EPlayerStat stat)
        {
            if (!Configuration.Instance.PayZombieKills)
                return;

            if (stat == EPlayerStat.KILLS_ZOMBIES_NORMAL)
            {
                player.TriggerEffect(81); // money effect
                Balances[player.CSteamID] = Decimal.Add(Balances[player.CSteamID], Configuration.Instance.PayoutZombie);
                UnturnedChat.Say(player, Translations.Instance.Translate("toolkit_player_zombie_kill", String.Format("{0:C}", Configuration.Instance.PayoutZombie)), Color.yellow);
            }

            if (stat == EPlayerStat.KILLS_ZOMBIES_MEGA)
            {
                player.TriggerEffect(81); // money effect
                Balances[player.CSteamID] = Decimal.Add(Balances[player.CSteamID], Configuration.Instance.PayoutMegaZombie);
                UnturnedChat.Say(player, Translations.Instance.Translate("toolkit_player_mega_zombie_kill", String.Format("{0:C}", Configuration.Instance.PayoutMegaZombie)), Color.cyan);
            }
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

        public string ReturnMurdererName(CSteamID murderer)
        {
            UnturnedPlayer killer = UnturnedPlayer.FromCSteamID(murderer);
            return killer.CharacterName;
        }

        public string ReturnKillDistance(UnturnedPlayer player, CSteamID murderer)
        {
            return Math.Round((double)Vector3.Distance(player.Position, UnturnedPlayer.FromCSteamID(murderer).Position)).ToString() + "m";
        }
    }
}
