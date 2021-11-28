using FChatSharpLib.Entities.Plugin.Commands;
using RDVFSharp.Errors;
using System.Collections.Generic;


namespace RDVFSharp.Commands
{
    public class Aoe : Action

    {
        public override void ExecuteCommand(string character, IEnumerable<string> args, string channel)
    {
        if (Plugin.CurrentTeamBattlefield.IsActive)
        {
            base.ExecuteCommand(character, args, channel);
        }

        if (Plugin.CurrentBattlefield.IsActive)
        {
            Plugin.FChatClient.SendMessageInChannel("This cannot be used in 1v1 fights.", Plugin.Channel);
        }
    }
}
}
