using System;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace NEXIS.Toolkit.Players
{
    public class CommandWarp : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public bool AllowFromConsole => false;

        public string Name => "warp";

        public string Help => "Warp to a specific location";

        public List<string> Aliases => new List<string>() { };

        public string Syntax => "/warp <location>";

        public List<string> Permissions => new List<string>() { "warp" };

        #endregion

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length == 1)
            {
                var warp = Toolkit.Instance.WarpList.Find(x => x.Name.ToUpper().Contains(command[0].ToUpper()));

                if (warp != null)
                {
                    if (player.IsInVehicle)
                    {
                        UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("jtools_warp_vehicle"), Color.red);
                        return;
                    }

                    if (Toolkit.Instance.Balances[player.CSteamID.ToString()] >= warp.Price)
                    {
                        Toolkit.Instance.Balances[player.CSteamID.ToString()] = Decimal.Subtract(Toolkit.Instance.Balances[player.CSteamID.ToString()], warp.Price);

                        // randomize position
                        System.Random rand = new System.Random();
                        string pos = warp.Position[rand.Next(warp.Position.Count)];

                        string[] sArray = pos.Split(',');
                        Vector3 warpPos = new Vector3(
                            float.Parse(sArray[0]),
                            float.Parse(sArray[1]),
                            float.Parse(sArray[2]));

                        player.Teleport(warpPos, warp.Rotation);

                        player.TriggerEffect(147); // electricity effect
                        UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_warp", warp.Name, String.Format("{0:C}", warp.Price)), Color.green);
                    }
                    else
                        UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_insufficient_credits"), Color.red);
                }
                else
                    UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_warp_noexist"), Color.red);
            }
            else
            {
                UnturnedChat.Say(caller, Help, Color.white);
                UnturnedChat.Say(caller, "Syntax: " + Syntax, Color.yellow);
            }
        }
    }
}
