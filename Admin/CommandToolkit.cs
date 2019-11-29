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
                        UnturnedChat.Say(player, "Toolkit Database Status:", Color.yellow);
                        UnturnedChat.Say(player, "Credits: " + Toolkit.Instance.Balances.Count + 
                            ", Warps: " + Toolkit.Instance.WarpList.Count +
                            ", Items: " + Toolkit.Instance.ItemList.Count +
                            ", Vehicles: " + Toolkit.Instance.VehicleList.Count +
                            ", Kits: " + Toolkit.Instance.KitList.Count, Color.white);
                        break;
                    case "update":
                        // update all json files
                        Toolkit.Instance.Credits.Update();
                        Toolkit.Instance.Items.Update();
                        Toolkit.Instance.Vehicles.Update();
                        Toolkit.Instance.Kits.Update();
                        Toolkit.Instance.Warps.Update();
                        UnturnedChat.Say(player, "Toolkit files updated!", Color.yellow);
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