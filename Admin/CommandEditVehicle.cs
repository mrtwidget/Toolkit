using System;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using SDG.Unturned;
using UnityEngine;

namespace NEXIS.Toolkit
{
    public class CommandEditVehicle : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public bool AllowFromConsole => true;

        public string Name => "editvehicle";

        public string Help => "Edit an existing vehicle in the shop";

        public List<string> Aliases => new List<string>() { "ev" };

        public string Syntax => "/editvehicle <id> <name> <buyprice>";

        public List<string> Permissions => new List<string>() { "editvehicle" };

        #endregion

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length == 3)
            {
                // search for vehicle by input id
                var vehicle = Toolkit.Instance.VehicleList.Find(x => x.ID == Convert.ToUInt16(command[0]));

                // if vehicle does not exist
                if (vehicle == null)
                {
                    UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_admin_vehicle_noexist"), Color.red);
                    return;
                }

                // remove existing item
                Toolkit.Instance.VehicleList.Remove(vehicle);

                // edit the values and re-add the modified item
                vehicle.Name = command[1];
                vehicle.BuyPrice = Convert.ToDecimal(command[2]);
                Toolkit.Instance.VehicleList.Add(vehicle);

                UnturnedChat.Say(player, Toolkit.Instance.Translations.Instance.Translate("toolkit_admin_vehicle_updated", vehicle.Name, vehicle.ID, vehicle.BuyPrice), Color.green);
            }
            else
            {
                UnturnedChat.Say(caller, Help, Color.white);
                UnturnedChat.Say(caller, "Syntax: " + Syntax, Color.yellow);
            }
        }
    }
}