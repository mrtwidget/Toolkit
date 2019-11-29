using System;
using System.Linq;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using UnityEngine;
using SDG.Unturned;
using System.Text;

namespace NEXIS.Toolkit.Players
{
    public class CommandKit : IRocketCommand
    {
        #region Properties

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public bool AllowFromConsole => false;

        public string Name => "kit";

        public string Help => "Purchase a pre-made kit of items";

        public List<string> Aliases => new List<string>() { };

        public string Syntax => "/kit <name>";

        public List<string> Permissions => new List<string>() { "kit" };

        #endregion

        public void Execute(IRocketPlayer caller, params string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;

            if (command.Length == 1)
            {
                Item i;

                // search for kit by input name
                Kits kit = Toolkit.Instance.KitList.Where(k => k.Name.ToLower() == command[0].ToLower()).FirstOrDefault();

                // if kit name does not exist
                if (kit == null)
                {
                    UnturnedChat.Say(player, Toolkit.Instance.Translations.Instance.Translate("toolkit_kit_noexist"), Color.red);
                    return;
                }

                // if player cannot afford the kit
                if (Toolkit.Instance.Balances[player.CSteamID.ToString()] < kit.Cost)
                {
                    UnturnedChat.Say(player, Toolkit.Instance.Translations.Instance.Translate("toolkit_kit_insufficient_credits"), Color.red);
                    return;
                }

                // loop through each item in kit
                foreach (KitItem item in kit.Items)
                {
                    // if item contains metadata, create a new item and
                    // apply passed metadata to it then give to player
                    if (item.Metadata != null)
                    {
                        i = new Item(item.ItemId, false);

                        string[] strings = item.Metadata.Split(',');
                        byte[] md = strings.Select(s => byte.Parse(s)).ToArray();

                        i.metadata = md;
                        player.Inventory.tryAddItemAuto(i, true, true, true, true);
                    }
                    else
                    {
                        player.GiveItem(item.ItemId, item.Amount);
                    }
                }

                // charge player for the kit
                Toolkit.Instance.Balances[player.CSteamID.ToString()] = Decimal.Subtract(Toolkit.Instance.Balances[player.CSteamID.ToString()], kit.Cost);
                UnturnedChat.Say(player, Toolkit.Instance.Translations.Instance.Translate("toolkit_kit_received", kit.Name, String.Format("{0:C}", kit.Cost)), Color.green);
            }
            else
            {
                UnturnedChat.Say(caller, Help, Color.white);
                UnturnedChat.Say(caller, "Syntax: " + Syntax, Color.yellow);
            }
        }
    }
}