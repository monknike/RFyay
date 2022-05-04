using FChatSharpLib.Entities.Plugin.Commands;
using RDVFSharp.Errors;
using System.Collections.Generic;

namespace RDVFSharp.Commands
{
    public class Curse : Action
    {
        public override void ExecuteCommand(string character, IEnumerable<string> args, string channel)
        {
            if (Plugin.CurrentBattlefield.GetActor().CurseUsed < 5)
            {
                base.ExecuteCommand(character, args, channel);
            }
            else
            {
                Plugin.FChatClient.SendMessageInChannel("You have already used curse once this match!", Plugin.Channel);
            }
        }
    }
}

