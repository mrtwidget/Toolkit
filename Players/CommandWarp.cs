using System;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace NEXIS.Toolkit
{
    public class CommandWarp : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public bool AllowFromConsole => false;

        public string Name => "warp";

        public string Help => "Warp to a location on the map by its name";

        public List<string> Aliases => new List<string>() { };

        public string Syntax => "/warp <name>";

        public List<string> Permissions => new List<string>() { "warp" };

        #endregion

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            if (command.Length == 1)
            {
                
            }
            else
            {
                UnturnedChat.Say(caller, Help, Color.gray);
                UnturnedChat.Say(caller, Syntax, Color.white);
            }
        }
    }
}