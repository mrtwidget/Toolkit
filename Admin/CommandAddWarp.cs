using System;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace NEXIS.Toolkit.Admin
{
    public class CommandAddWarp : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public bool AllowFromConsole => false;

        public string Name => "addwarp";

        public string Help => "Adds new warp location from your current position";

        public List<string> Aliases => new List<string>() { };

        public string Syntax => "/addwarp <name> <price>";

        public List<string> Permissions => new List<string>() { "addwarp" };

        #endregion

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length == 2)
            {
                // get player position and trim
                string p = player.Position.ToString();
                p = p.Replace("(", string.Empty);
                p = p.Replace(")", string.Empty);

                // create new warp
                var w = new Warps();
                    w.Name = command[0];
                    w.Position = new List<string> { p };
                    w.Rotation = player.Rotation;
                    w.Price = Convert.ToDecimal(command[1]);

                Toolkit.Instance.WarpList.Add(w);
                UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_admin_warp_added", w.Name, String.Format("{0:C}", w.Price)), Color.green);
            }
            else
            {
                UnturnedChat.Say(caller, Help, Color.white);
                UnturnedChat.Say(caller, "Syntax: " + Syntax, Color.yellow);
            }
        }
    }
}
