using System;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace NEXIS.Toolkit
{
    public class CommandToolkit : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public bool AllowFromConsole => true;

        public string Name => "toolkit";

        public string Help => "Admin control for Toolkit plugin";

        public List<string> Aliases => new List<string>() { "tk" };

        public string Syntax => "/toolkit help";

        public List<string> Permissions => new List<string>() { "toolkit" };

        #endregion

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length > 0)
            {
                switch (command[0])
                {
                    case "help":
                        // show help info
                        UnturnedChat.Say(player, "Toolkit Commands:", Color.yellow);
                        UnturnedChat.Say(player, "/toolkit info : Show database information", Color.white);
                        UnturnedChat.Say(player, "/toolkit update : Updates all database JSON files", Color.white);
                        break;
                    case "info":
                        // show database info
                        UnturnedChat.Say(player, "Toolkit Database Info:", Color.yellow);
                        UnturnedChat.Say(player, "Credits: " + Toolkit.Instance.Balances.Count + 
                            ", Warps: " + Toolkit.Instance.WarpList.Count +
                            ", Items: " + Toolkit.Instance.ItemList.Count +
                            ", Vehicles: " + Toolkit.Instance.VehicleList.Count +
                            ", Kits: " + Toolkit.Instance.KitList.Count +
                            ", Messages: " + Toolkit.Instance.MessageList.Count, Color.white);
                        UnturnedChat.Say(player, "Toolkit Config Settings:", Color.yellow);
                        UnturnedChat.Say(player, "Initial Credits: " + String.Format("{0:C}", Toolkit.Instance.Configuration.Instance.InitialBalance) +
                            ", PayZombieKill: " + Toolkit.Instance.Configuration.Instance.PayZombieKills +
                            ", PayPlayerKill: " + Toolkit.Instance.Configuration.Instance.PayPlayerKills +
                            ", PayDistanceX: " + Toolkit.Instance.Configuration.Instance.PayDistanceMultiplier + "(min " + Toolkit.Instance.Configuration.Instance.PayoutMinDistance + "m)" +
                            ", Payouts: " + "(Plr: " + String.Format("{0:C}", Toolkit.Instance.Configuration.Instance.PayoutKillPlayer) + ", Norm: " + String.Format("{0:C}", Toolkit.Instance.Configuration.Instance.PayoutKillZombie) + ", Mega: " + String.Format("{0:C}", Toolkit.Instance.Configuration.Instance.PayoutKillMegaZombie) + ")"
                            , Color.white);
                        UnturnedChat.Say(player, ", TPA: " + "Enabled: " + Toolkit.Instance.Configuration.Instance.EnableTPA + ", TPA Timeout: " + Toolkit.Instance.Configuration.Instance.EnableTPATimeout + "(" + Toolkit.Instance.Configuration.Instance.TPATimeout + ")" +
                            ", DayInChat: " + Toolkit.Instance.Configuration.Instance.ChangeDaytimeChat +
                            ", MaxBuyAmt: " + Toolkit.Instance.Configuration.Instance.MaxBuyAmount +
                            ", AutoMsg: (Enabled: " + Toolkit.Instance.Configuration.Instance.AutoMessagesEnabled + ", Random: " + Toolkit.Instance.Configuration.Instance.AutoMessageRandom + ", Interval: " + Toolkit.Instance.Configuration.Instance.AutoMessageInterval + ", Default color: " + Toolkit.Instance.Configuration.Instance.AutoMessageDefaultColor + ")"
                            , Color.white);
                        break;
                    case "update":
                        // update all json files
                        Toolkit.Instance.Credits.Update();
                        Toolkit.Instance.Items.Update();
                        Toolkit.Instance.Vehicles.Update();
                        Toolkit.Instance.Kits.Update();
                        Toolkit.Instance.Warps.Update();
                        Toolkit.Instance.Messages.Update();
                        UnturnedChat.Say(player, "Toolkit database files updated!", Color.green);
                        break;
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