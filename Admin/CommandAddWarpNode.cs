using System;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace NEXIS.Toolkit.Admin
{
    public class CommandAddWarpNode : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public bool AllowFromConsole => false;

        public string Name => "addwarpnode";

        public string Help => "Adds a warp spawn location to an existing warp";

        public List<string> Aliases => new List<string>() { };

        public string Syntax => "/addwarpnode <warp>";

        public List<string> Permissions => new List<string>() { "addwarpnode" };

        #endregion

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length == 1)
            {
                var warp = Toolkit.Instance.WarpList.Find(x => x.Name.ToUpper().Contains(command[0].ToUpper()));

                if (warp != null)
                {
                    string output = player.Position.ToString();
                    output = output.Replace("(", string.Empty);
                    output = output.Replace(")", string.Empty);

                    warp.Position.Add(output);
                    Toolkit.Instance.Warps.Update();

                    UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_admin_warp_node_added", warp.Name), Color.green);
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
