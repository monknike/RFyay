using System.Collections.Generic;

namespace RDVFSharp.Commands
{
    public class Throw : Action
    {
        public override void ExecuteCommand(string character, IEnumerable<string> args, string channel)
        {
            if (Plugin.CurrentTeamBattlefield.IsActive)

            {
                if (Plugin.CurrentTeamBattlefield.GetActor().IsRestrained || Plugin.CurrentTeamBattlefield.GetTarget().IsRestrained)
                {
                    base.ExecuteCommand(character, args, channel);
                }
            }


            else if (Plugin.CurrentBattlefield.IsActive)

            {
                if (Plugin.CurrentBattlefield.GetActor().IsRestrained || Plugin.CurrentBattlefield.GetTarget().IsRestrained)
                {
                    base.ExecuteCommand(character, args, channel);
                }
            }

            else
            {
                Plugin.FChatClient.SendMessageInChannel("You can only use Throw if you are grappling.", Plugin.Channel);
            }
        }
    }
}
