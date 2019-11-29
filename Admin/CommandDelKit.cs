using System;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using SDG.Unturned;
using UnityEngine;

namespace NEXIS.Toolkit
{
    public class CommandDelKit : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public bool AllowFromConsole => true;

        public string Name => "delkit";

        public string Help => "Delete an existing kit (non-recoverable)";

        public List<string> Aliases => new List<string>() { };

        public string Syntax => "/delkit <name>";

        public List<string> Permissions => new List<string>() { "delkit" };

        #endregion

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length == 1)
            {
                // search for kit by input name
                var kit = Toolkit.Instance.KitList.Find(x => x.Name == command[0]);

                // if kit name does not exist
                if (kit == null)
                {
                    UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_kit_noexist"), Color.red);
                    return;
                }

                Toolkit.Instance.KitList.Remove(kit);
                UnturnedChat.Say(player, "Removed kit: " + kit.Name);
            }
            else
            {
                UnturnedChat.Say(caller, Help, Color.white);
                UnturnedChat.Say(caller, "Syntax: " + Syntax, Color.yellow);
            }
        }
    }
}