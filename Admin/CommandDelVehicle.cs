using System;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using SDG.Unturned;
using UnityEngine;

namespace NEXIS.Toolkit
{
    public class CommandDelVehicle : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public bool AllowFromConsole => true;

        public string Name => "delvehicle";

        public string Help => "Delete an existing vehicle in the shop";

        public List<string> Aliases => new List<string>() { "delv", "dv" };

        public string Syntax => "/delvehicle <id>";

        public List<string> Permissions => new List<string>() { "delvehicle" };

        #endregion

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length == 1)
            {
                // search for vehicle by input id
                var vehicle = Toolkit.Instance.VehicleList.Find(x => x.ID == Convert.ToUInt16(command[0]));

                // if vehicle id does not exist
                if (vehicle == null)
                {
                    UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_admin_vehicle_noexist"), Color.red);
                    return;
                }

                Toolkit.Instance.VehicleList.Remove(vehicle);
                UnturnedChat.Say(player, Toolkit.Instance.Translations.Instance.Translate("toolkit_admin_vehicle_deleted", vehicle.Name, vehicle.ID), Color.green);
            }
            else
            {
                UnturnedChat.Say(caller, Help, Color.white);
                UnturnedChat.Say(caller, "Syntax: " + Syntax, Color.yellow);
            }
        }
    }
}