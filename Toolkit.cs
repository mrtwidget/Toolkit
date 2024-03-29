﻿using System;
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
using Logger = Rocket.Core.Logging.Logger;
using Steamworks;

namespace NEXIS.Toolkit
{
    public class Toolkit : RocketPlugin<ToolkitConfiguration>
    {
        #region Fields

        public static Toolkit Instance;
        public List<TPA> TPArequests;
        public Dictionary<string, decimal> Balances;
        public Credits Credits;
        public Warps Warps;
        public List<Warps> WarpList;
        public Items Items;
        public List<Items> ItemList;
        public Vehicles Vehicles;
        public List<Vehicles> VehicleList;
        public int DayRequests;
        public Kits Kits;
        public List<Kits> KitList;
        public Messages Messages;
        public List<Messages> MessageList;
        public DateTime? LastMessage;
        public int LastMessageIndex = 0;

        // Custom message colors
        public static Color DeathColor = new Color(50, 0, 200);
        public static Color HeadshotColor = new Color(1, 50, 256);
        public static Color PVPColor = new Color(1, 0, 200);

        #endregion

        #region Overrides

        protected override void Load()
        {
            Instance = this;
            Balances = new Dictionary<string, decimal>();
            TPArequests = new List<TPA>();
            DayRequests = 0;

            // load credits
            Credits = new Credits();
            Credits.Load();

            // load warps
            Warps = new Warps();
            Warps.Load();

            // load items
            Items = new Items();
            Items.Load();

            // load vehicles
            Vehicles = new Vehicles();
            Vehicles.Load();

            // load kits
            Kits = new Kits();
            Kits.Load();

            // load messages
            Messages = new Messages();
            Messages.Load();

            U.Events.OnPlayerConnected += Events_OnPlayerConnected;
            U.Events.OnPlayerDisconnected += Events_OnPlayerDisconnected;
            UnturnedPlayerEvents.OnPlayerDeath += Events_OnPlayerDeath;
            UnturnedPlayerEvents.OnPlayerChatted += Events_OnPlayerChatted;
            UnturnedPlayerEvents.OnPlayerUpdateStat += Events_OnPlayerUpdateStat;

            // load connected players if loaded on-the-fly
            if (Provider.clients.Count > 0)
            {
                Logger.Log(Provider.clients.Count + " are currently connected. Loading...", ConsoleColor.Yellow);

                // loop through all players
                for (int i = 0; i < Provider.clients.Count; i++)
                {
                    SteamPlayer plr = Provider.clients[i];
                    if (plr == null) continue;

                    UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(plr);
                    Events_OnPlayerConnected(player);
                }
            }

            Logger.Log("Loaded!", ConsoleColor.DarkGreen);
        }

        protected override void Unload()
        {
            // save credits
            Credits.Update();

            // update shop
            Items.Update();
            Vehicles.Update();

            // update kits
            Kits.Update();

            // update warps
            Warps.Update();

            // update messages
            Messages.Update();

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
                    {"toolkit_tpa_disabled", "Sorry, but TPA is currently disabled"},
                    {"toolkit_day", "Time changed to Day per player requests!"},
                    {"toolkit_day_request", "Players are requesting Daytime. {0} more players need to say \"day\" in chat to change it"},
                    {"toolkit_admin_warp_added", "Added {0} warp costing {1} credits"},
                    {"toolkit_admin_warp_deleted", "Permanently deleted the {0} warp!"},
                    {"toolkit_admin_warp_node_added", "Warp node added to {0}"},
                    {"toolkit_admin_item_noexist", "That item does not exist in the Shop!"},
                    {"toolkit_admin_vehicle_noexist", "That vehicle does not exist in the Shop!"},
                    {"toolkit_admin_item_deleted", "Removed item {0}({1}) from Shop!"},
                    {"toolkit_admin_vehicle_deleted", "Removed vehicle {0}({1}) from Shop!"},
                    {"toolkit_admin_item_updated", "Updated item {0}({1}), Buy Price: {2}, Sell Price: {3}"},
                    {"toolkit_admin_vehicle_updated", "Updated vehicle {0}({1}), Buy Price: {2}"},
                    {"toolkit_warp", "You warpped to {0} costing -{1} credits!"},
                    {"toolkit_warp_noexist", "That warp location does not exist!"},
                    {"toolkit_warp_vehicle", "You can't warp when in a vehicle!"},
                    {"toolkit_tpa_info", "You can teleport to another player by typing: /tpa <name>"},
                    {"toolkit_warp_info", "You can warp by typing: /warp <location> or see a list of warps by typing: /warps"},
                    {"toolkit_buy_info", "Buy items by typing: /buy <id>, Sell items: /sell <id>, Price: /cost <id>"},
                    {"toolkit_buy_vehicle_info", "You can buy vehicles by typing: /vbuy <id>, or /vcost <id> for the price"},
                    {"toolkit_admin_item_added", "Added {1}({0}) to Shop - Buy Price: {2}, Sell Price: {3}"},
                    {"toolkit_admin_vehicle_added", "Added {1}({0}) to Shop - Buy Price: {2}"},
                    {"toolkit_admin_addmessage", "New message added!"},
                    {"toolkit_player_buy_noexist", "That ID does not exist in the Shop!"},
                    {"toolkit_player_buy_amount", "You can't buy more than {0} items at once!"},
                    {"toolkit_player_buy_insufficient_credits", "You do not have enough credits to afford this!"},
                    {"toolkit_player_buy", "You purchased {2} {0}s, costing {1} credits"},
                    {"toolkit_player_buy_cost", "The cost of a {1}({0}) is - Buy Price: {2}, Sell Price: {3}"},
                    {"toolkit_player_buy_cost_vehicle", "The cost of a {1}({0}) is - Buy Price: {2}"},
                    {"toolkit_player_connected", "{0} has connected to the server"},
                    {"toolkit_player_disconnected", "{0} gave up and left the server"},
                    {"toolkit_insufficient_credits", "You don't have enough credits for that!"},
                    {"toolkit_tpa_request", "{0} has sent a request to teleport to your location. Type: /tpa accept, or /tpa deny"},
                    {"toolkit_tpa_request_caller", "You have sent a teleport request to {0}. Waiting for them to respond..."},
                    {"toolkit_tpa_request_null", "You have no teleport requests waiting..."},
                    {"toolkit_tpa_player_null", "Could not find a player with that name. Try again."},
                    {"toolkit_tpa_request_deny", "{0} has denied your teleport request"},
                    {"toolkit_tpa_request_deny_caller", "You have denied {0}'s teleport request"},
                    {"toolkit_tpa_request_accept", "{0} has accepted your teleport request!"},
                    {"toolkit_tpa_request_accept_caller", "{0} has been teleported to your location!"},
                    {"toolkit_kit_exists", "A kit with that name already exists!"},
                    {"toolkit_kit_noexist", "A kit with that name does not exist!"},
                    {"toolkit_kit_received", "You received the {0} kit, costing {1} credits!"},
                    {"toolkit_kit_insufficient_credits", "You do not have enough credits to purchase this kit!"},
                    {"toolkit_sell_noexist", "You don't have that item to sell!"},
                    {"toolkit_sell_noexist_db", "That item does not exist in the Shop database! Damn lazy admins!"},
                    {"toolkit_sell_amount", "You don't have that many items to sell!"},
                    {"toolkit_sell", "You sold {2} {0}s and have been credited {1}!"},
                    {"toolkit_pay_disabled", "Oh noes! This command is currently disabled."},
                    {"toolkit_pay_player_amount", "You can't pay something nothing. What are you even trying to do? xD"},
                    {"toolkit_pay_credits", "You don't have enough credits!"},
                    {"toolkit_pay_giver", "You payed {0} {1} credits!"},
                    {"toolkit_pay_receiver", "{0} payed you {1} credits!"},
                    {"toolkit_pay_player_null", "Can't find a player with that name"},
                    {"toolkit_pay_yourself", "You can't pay yourself. If you could we'd all work from home."},
                    {"toolkit_exchange_disabled", "Sorry, but the exchange is currently disabled"},
                    {"toolkit_exchange_experience", "You don't have that much experience!"},
                    {"toolkit_exchange_complete", "You exchanged {0} experience and received {1} credits!"},
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
                    {"toolkit_player_balance", "You have {0} in credits. Spend them wisely."},
                    {"toolkit_player_zombie_kill", "You received {0} credits for killing a Zombie"},
                    {"toolkit_player_mega_zombie_kill", "You received {0} credits for killing a MEGA Zombie!"},
                    {"toolkit_player_player_kill", "You received {0} credits for killing a player!"},
                    {"toolkit_player_kill_multiplier", "You've been awarded an extra {1} credits for headshotting {0} above {2}m!"}
                };
            }
        }

        #endregion

        #region Events

        public void Events_OnPlayerConnected(UnturnedPlayer player)
        {
            UnturnedChat.Say(Translations.Instance.Translate("toolkit_player_connected", player.CharacterName), Color.gray);

            // load an existing balance or initiate a new one
            if (!Balances.ContainsKey(player.CSteamID.ToString()))
            {
                Balances.Add(player.CSteamID.ToString(), Configuration.Instance.InitialBalance);
                UnturnedChat.Say(player, Translations.Instance.Translate("toolkit_player_initial_balance", player.CharacterName, String.Format("{0:C}", Configuration.Instance.InitialBalance)), Color.yellow);
            }
            else
                UnturnedChat.Say(player, Translations.Instance.Translate("toolkit_player_balance", String.Format("{0:C}", Balances[player.CSteamID.ToString()])), Color.yellow);

            // load balance to UI
            if (Configuration.Instance.UIBalanceEnabled)
                UpdateUI(player);
        }

        public void Events_OnPlayerDisconnected(UnturnedPlayer player)
        {
            UnturnedChat.Say(Translations.Instance.Translate("toolkit_player_disconnected", player.CharacterName), Color.black);
        }

        public void Events_OnPlayerDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            // check for a headshot
            if (cause == EDeathCause.GUN && limb == ELimb.SKULL)
            {
                // calculate distance of shot
                double distance = ReturnKillDistance(player, murderer);

                UnturnedChat.Say(Translations.Instance.Translate("toolkit_death_headshot", player.CharacterName, ReturnMurdererName(murderer), UnturnedPlayer.FromCSteamID(murderer).Player.equipment.asset.itemName.ToString(), distance + "m"), HeadshotColor);

                // if enabled, pay multiplier credits
                if (Configuration.Instance.PayDistanceMultiplier)
                {
                    // check if distance was over minimum amount
                    if (distance >= Configuration.Instance.PayoutMinDistance)
                    {
                        // subtract minimum amount from total and pay player the remainder
                        decimal payout = Convert.ToDecimal(distance - Configuration.Instance.PayoutMinDistance);
                        Balances[murderer.ToString()] = Decimal.Add(Balances[murderer.ToString()], payout);
                        UnturnedChat.Say(player, Translations.Instance.Translate("toolkit_player_kill_multiplier", player.CharacterName, payout, Configuration.Instance.PayoutMinDistance), Color.yellow);
                    }
                }
                return;
            }

            // send death message to chat
            SendDeathMessage(player, cause, limb, murderer);
        }

        public void Events_OnPlayerChatted(UnturnedPlayer player, ref Color color, string message, EChatMode chatMode, ref bool cancel)
        {
            // change time to day if enough players say "day" in global chat
            if (Configuration.Instance.ChangeDaytimeChat)
            {
                if (!message.StartsWith("/") && chatMode == EChatMode.GLOBAL)
                {
                    string msg = message.ToLower();

                    if (msg == "day")
                    {
                        if (DayRequests >= (Provider.clients.Count / 2))
                        {
                            DayRequests = 0;
                            LightingManager.time = 365;
                            UnturnedChat.Say(Translations.Instance.Translate("toolkit_day"), Color.white);
                        }
                        else
                            UnturnedChat.Say(Translations.Instance.Translate("toolkit_day_request", (Provider.clients.Count / 2) - DayRequests), Color.white);

                        DayRequests++;
                    }
                }
            }

            // show suggestions if players type certain keywords
            if (Configuration.Instance.EnableChatSuggestions)
            {
                if (!message.StartsWith("/") && (chatMode == EChatMode.GLOBAL || chatMode == EChatMode.LOCAL))
                {
                    string msg = message.ToLower();

                    if ((msg.Contains("what") || msg.Contains("how")) && (msg.Contains("warp") || msg.Contains("warps")))
                        UnturnedChat.Say(player, Translations.Instance.Translate("toolkit_warp_info"), Color.white);
                    else if ((msg.Contains("what") || msg.Contains("how")) && (msg.Contains("vehicle") || msg.Contains("car") || msg.Contains("jet") || msg.Contains("heli") || msg.Contains("apc")))
                        UnturnedChat.Say(player, Translations.Instance.Translate("toolkit_buy_vehicle_info"), Color.white);
                    else if ((msg.Contains("what") || msg.Contains("how")) && (msg.Contains("buy") || msg.Contains("purchase") || msg.Contains("sell") || msg.Contains("cost") || msg.Contains("price")))
                        UnturnedChat.Say(player, Translations.Instance.Translate("toolkit_buy_info"), Color.white);
                    else if ((msg.Contains("what") || msg.Contains("how")) && (msg.Contains("tpa") || msg.Contains("teleport")))
                        UnturnedChat.Say(player, Translations.Instance.Translate("toolkit_tpa_info"), Color.white);
                }
            }
        }

        /**
         * This function awards players credits for killing zombies and other
         * players. Payout values can be edited in the config
         */
        public void Events_OnPlayerUpdateStat(UnturnedPlayer player, EPlayerStat stat)
        {
            switch (stat)
            {
                // normal zombie
                case EPlayerStat.KILLS_ZOMBIES_NORMAL:
                    if (Configuration.Instance.PayZombieKills)
                    {
                        player.TriggerEffect(81); // money effect
                        Balances[player.CSteamID.ToString()] = Decimal.Add(Balances[player.CSteamID.ToString()], Configuration.Instance.PayoutKillZombie);
                        if (Configuration.Instance.UIBalanceEnabled)
                            UpdateUI(player);
                        UnturnedChat.Say(player, Translations.Instance.Translate("toolkit_player_zombie_kill", String.Format("{0:C}", Configuration.Instance.PayoutKillZombie)), Color.yellow);
                    }
                    break;
                // mega zombie
                case EPlayerStat.KILLS_ZOMBIES_MEGA:
                    if (Configuration.Instance.PayZombieKills)
                    {
                        player.TriggerEffect(81); // money effect
                        Balances[player.CSteamID.ToString()] = Decimal.Add(Balances[player.CSteamID.ToString()], Configuration.Instance.PayoutKillMegaZombie);
                        if (Configuration.Instance.UIBalanceEnabled)
                            UpdateUI(player);
                        UnturnedChat.Say(player, Translations.Instance.Translate("toolkit_player_mega_zombie_kill", String.Format("{0:C}", Configuration.Instance.PayoutKillMegaZombie)), Color.magenta);
                    }
                    break;
                // players
                case EPlayerStat.KILLS_PLAYERS:
                    if (Configuration.Instance.PayPlayerKills)
                    {
                        player.TriggerEffect(81); // money effect
                        Balances[player.CSteamID.ToString()] = Decimal.Add(Balances[player.CSteamID.ToString()], Configuration.Instance.PayoutKillPlayer);
                        if (Configuration.Instance.UIBalanceEnabled)
                            UpdateUI(player);
                        UnturnedChat.Say(player, Translations.Instance.Translate("toolkit_player_player_kill", String.Format("{0:C}", Configuration.Instance.PayoutKillPlayer)), Color.cyan);
                    }
                    break;
            }
        }

        #endregion

        public void FixedUpdate()
        {
            if (Instance.State != PluginState.Loaded) return;

            // show automatted messages if enabled
            if (Configuration.Instance.AutoMessagesEnabled)
            {
                if (LastMessage == null || (DateTime.Now - LastMessage.Value).TotalSeconds > Configuration.Instance.AutoMessageInterval)
                {
                    // random message if enabled
                    if (Configuration.Instance.AutoMessageRandom)
                    {
                        System.Random rnd = new System.Random();
                        int r = rnd.Next(MessageList.Count);

                        // try to avoid repeat messages
                        if (LastMessageIndex == r) r=0;

                        // set message color and send
                        Color color = MessageList[r].Color.ToColor();
                        UnturnedChat.Say(MessageList[r].Message, color);
                        LastMessageIndex = r;
                    }
                    // display messages in order and then repeat
                    else
                    {
                        if (LastMessageIndex == MessageList.Count)
                        {
                            LastMessageIndex = 0;
                            return;
                        }

                        // set message color and send
                        Color color = MessageList[LastMessageIndex].Color.ToColor();
                        UnturnedChat.Say(MessageList[LastMessageIndex].Message, color);
                        LastMessageIndex++;
                    }

                    LastMessage = DateTime.Now;
                }
            }
        }

        /**
         * SEND CUSTOM DEATH MESSAGE
         * This function sends a customized death message to world chat
         * depentant on the cause of death. These messages can be edited in
         * the translation file to customize them. 
         */
        public void SendDeathMessage(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
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

        /**
         * RETURN LIMB NAME
         * This function returns a string of which limb was hit on a player.
         * Using a switch to do this seems cumbersome, but I'm uncertain if
         * there is a better way to do this, so. Here we are.
         */
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

        /**
         * RETURN CHARACTER NAME FROM STEAMID
         * This function returns the character name of a player by converting
         * CSteamID to UnturnedPlayer by using the FromCSteamID() function.
         */
        public string ReturnMurdererName(CSteamID murderer)
        {
            UnturnedPlayer killer = UnturnedPlayer.FromCSteamID(murderer);
            return killer.CharacterName;
        }

        /**
         * CALCULATE DISTANCE OF KILL
         * This function calculates the distance between two players by using
         * the Vector3.Distance() function.
         */
        public double ReturnKillDistance(UnturnedPlayer player, CSteamID murderer)
        {
            return Math.Round((double)Vector3.Distance(player.Position, UnturnedPlayer.FromCSteamID(murderer).Position));
        }

        /**
         * UPDATE USER INTERFACE
         * This function updates the UconomyUI mod with the player's current
         * credit balance. Requires UconomyUI
         */
        public void UpdateUI(UnturnedPlayer player)
        {
            EffectManager.askEffectClearByID(65090, player.CSteamID);
            EffectManager.sendUIEffect(65090, 0, player.CSteamID, true, String.Format("{0:C}", Balances[player.CSteamID.ToString()]));
        }

        /* Check if string contains only digits */
        public bool IsDigits(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }
    }

    /* Convert string into Color */
    public static class ColorExtentions
    {
        public static Color ToColor(this string color)
        {
            return (Color)typeof(Color).GetProperty(color.ToLowerInvariant()).GetValue(null, null);
        }
    }
}