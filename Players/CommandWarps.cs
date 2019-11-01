using System;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace NEXIS.Toolkit.Players
{
    public class CommandWarps : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public bool AllowFromConsole => false;

        public string Name => "warps";

        public string Help => "A list of all available warp locations";

        public List<string> Aliases => new List<string>() { };

        public string Syntax => "/warps";

        public List<string> Permissions => new List<string>() { "warps" };

        #endregion

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length == 0)
            {
                string warps = "";
                int count = 0;

                foreach (Warps warp in Toolkit.Instance.WarpList)
                {
                    count++;
                    warps += warp.Name;

                    if (count < Toolkit.Instance.WarpList.Count)
                        warps += ", ";
                }

                UnturnedChat.Say(caller, "Warps: " + warps, Color.green);
                UnturnedChat.Say(caller, "To use type: /warp <location> (partial names work too)", Color.yellow);
            }
            else
            {
                UnturnedChat.Say(caller, Help, Color.white);
                UnturnedChat.Say(caller, "Syntax: " + Syntax, Color.yellow);
            }
        }
    }
}
