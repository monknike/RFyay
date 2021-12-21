﻿using System.Collections.Generic;

namespace RDVFSharp.Commands
{
    public class Submission : Action
    {
        public override void ExecuteCommand(string character, IEnumerable<string> args, string channel)
        {
            if (Plugin.CurrentTeamBattlefield.GetTarget().IsRestrained && Plugin.CurrentTeamBattlefield.GetActor().IsGrabbable == Plugin.CurrentTeamBattlefield.GetTarget().IsGrabbable)
            {
                base.ExecuteCommand(character, args, channel);
            }


            else if (Plugin.CurrentBattlefield.GetTarget().IsRestrained)
            {
                base.ExecuteCommand(character, args, channel);
            }

            
            else 
            {
                Plugin.FChatClient.SendMessageInChannel("You can only use Submission if you are grappling your opponent.", Plugin.Channel);
            }
        }
    }
}
