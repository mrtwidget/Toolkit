using System;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace NEXIS.Toolkit.Admin
{
    public class CommandDelWarp : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public bool AllowFromConsole => true;

        public string Name => "delwarp";

        public string Help => "Permanently delete a warp location and ALL nodes associated with it";

        public List<string> Aliases => new List<string>() { };

        public string Syntax => "/delwarp <name>";

        public List<string> Permissions => new List<string>() { "delwarp" };

        #endregion

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length == 1)
            {
                Warps warp = Toolkit.Instance.WarpList.Find(x => x.Name.ToUpper().Contains(command[0].ToUpper()));

                if (warp == null)
                {
                    UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_warp_noexist"), Color.red);
                    return;
                }

                Toolkit.Instance.WarpList.Remove(warp);
                Toolkit.Instance.Warps.Update();

                UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_admin_warp_deleted", warp.Name), Color.green);
            }
            else
            {
                UnturnedChat.Say(caller, Help, Color.white);
                UnturnedChat.Say(caller, "Syntax: " + Syntax, Color.yellow);
            }
        }
    }
}
