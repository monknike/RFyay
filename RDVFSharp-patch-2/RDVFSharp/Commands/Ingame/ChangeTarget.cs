using FChatSharpLib.Entities.Plugin.Commands;
using RDVFSharp.Errors;
using System.Collections.Generic;

namespace RDVFSharp.Commands
{
    public class ChangeTarget : Action
    {
        public override void ExecuteCommand(string character, IEnumerable<string> args, string channel)
        {

            if (Plugin.CurrentTeamBattlefield.IsActive && !Plugin.CurrentTeamBattlefield.GetActor().IsRestrained && !Plugin.CurrentTeamBattlefield.GetActor().IsRestraining)
            {
                base.ExecuteCommand(character, args, channel);
            }

            else
            {
                Plugin.FChatClient.SendMessageInChannel("You can't change targets currently.", Plugin.Channel);
            }

        }
    }
}
