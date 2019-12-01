using System;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;

namespace NEXIS.Toolkit
{
    public class CommandAddMessage : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public bool AllowFromConsole => false;

        public string Name => "addmessage";

        public string Help => "Add a new message to the automatted messages. Don't forget to use quotes!";

        public List<string> Aliases => new List<string>() { "addmsg", "addm", "am" };

        public string Syntax => "/addmessage <message> [color]";

        public List<string> Permissions => new List<string>() { "addmessage" };

        #endregion

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            if (command.Length >= 1)
            {
                Messages msg = new Messages();
                msg.Message = command[0];
                msg.ID = Toolkit.Instance.MessageList.Count +1;

                // set color to default unless argument is passed
                if (command.Length == 2)
                    msg.Color = command[1];
                else
                    msg.Color = Toolkit.Instance.Configuration.Instance.AutoMessageDefaultColor;

                Toolkit.Instance.MessageList.Add(msg);
                UnturnedChat.Say(caller, Toolkit.Instance.Translations.Instance.Translate("toolkit_admin_addmessage"), Color.green);
            }
            else
            {
                UnturnedChat.Say(caller, Help, Color.white);
                UnturnedChat.Say(caller, "Syntax: " + Syntax, Color.yellow);
            }
        }
    }
}