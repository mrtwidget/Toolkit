using System;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace NEXIS.Toolkit.Players
{
    public class CommandKits : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public bool AllowFromConsole => false;

        public string Name => "kits";

        public string Help => "A list of all available kits for purchase";

        public List<string> Aliases => new List<string>() { };

        public string Syntax => "/kits";

        public List<string> Permissions => new List<string>() { "kits" };

        #endregion

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length == 0)
            {
                string kits = "";
                int count = 0;

                foreach (Kits kit in Toolkit.Instance.KitList)
                {
                    count++;
                    kits += kit.Name + "($" + kit.Cost + ")";

                    if (count < Toolkit.Instance.KitList.Count)
                        kits += ", ";
                }

                UnturnedChat.Say(caller, "Kits: " + kits, Color.green);
                UnturnedChat.Say(caller, "To use type: /kit <name> (partial names work too)", Color.yellow);
            }
            else
            {
                UnturnedChat.Say(caller, Help, Color.white);
                UnturnedChat.Say(caller, "Syntax: " + Syntax, Color.yellow);
            }
        }
    }
}