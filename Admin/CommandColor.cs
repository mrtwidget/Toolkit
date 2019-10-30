using System;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace NEXIS.Toolkit
{
    public class CommandColor : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public bool AllowFromConsole => false;

        public string Name => "color";

        public string Help => "Admin tool to test RGB colors in chat";

        public List<string> Aliases => new List<string>() { };

        public string Syntax => "/color <blue> <red> <yellow>";

        public List<string> Permissions => new List<string>() { "color" };

        #endregion

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            if (command.Length == 3)
            {
                UnturnedChat.Say(caller, "This is a test message!", new Color(Convert.ToSingle(command[0]), Convert.ToSingle(command[1]), Convert.ToSingle(command[2])));
            }
            else
            {
                UnturnedChat.Say(caller, Syntax, Color.white);
            }
        }
    }
}