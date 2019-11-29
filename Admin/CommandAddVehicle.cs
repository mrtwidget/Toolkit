using System;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace NEXIS.Toolkit.Admin
{
    public class CommandAddVehicle : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public bool AllowFromConsole => true;

        public string Name => "addvehicle";

        public string Help => "Add new vehicle to Shop";

        public List<string> Aliases => new List<string>() { };

        public string Syntax => "/addvehicle <id> <name> <buyprice>";

        public List<string> Permissions => new List<string>() { "addvehicle" };

        #endregion

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length == 3)
            {
                // create new warp
                var v = new Vehicles();
                v.ID = Convert.ToUInt16(command[0]);
                v.Name = command[1];
                v.BuyPrice = Convert.ToDecimal(command[2]);

                Toolkit.Instance.VehicleList.Add(v);

                UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_admin_vehicle_added", v.ID, v.Name, String.Format("{0:C}", v.BuyPrice)), Color.green);
            }
            else
            {
                UnturnedChat.Say(caller, Help, Color.white);
                UnturnedChat.Say(caller, "Syntax: " + Syntax, Color.yellow);
            }
        }
    }
}
