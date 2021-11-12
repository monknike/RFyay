﻿using FChatSharpLib.Entities.Plugin.Commands;
using RDVFSharp.Errors;
using System.Collections.Generic;

namespace RDVFSharp.Commands
{
    public class Status : BaseCommand<RendezvousFighting>
    {
        public override string Description => "Gets the status of an ongoing fight.";

        public override void ExecuteCommand(string character, IEnumerable<string> args, string channel)
        {
            if (Plugin.CurrentTeamBattlefield.IsActive)
            {
                Plugin.FChatClient.SendPrivateMessage(Plugin.CurrentTeamBattlefield.WindowController.LastMessageSent, character);
            }

            else if (Plugin.CurrentBattlefield.IsActive)
            {
                Plugin.FChatClient.SendPrivateMessage(Plugin.CurrentBattlefield.WindowController.LastMessageSent, character);
            }
            else
            {
                Plugin.FChatClient.SendPrivateMessage("There's no match going on right now.", character);
            }
        }
    }
}