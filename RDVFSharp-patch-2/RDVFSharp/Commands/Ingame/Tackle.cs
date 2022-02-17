using System.Collections.Generic;

namespace RDVFSharp.Commands
{
    public class Tackle : Action
    {
        public override void ExecuteCommand(string character, IEnumerable<string> args, string channel)
        {

            if (Plugin.CurrentBattlefield.IsActive)


            {
                if (!Plugin.CurrentBattlefield.InGrabRange)
                {
                    base.ExecuteCommand(character, args, channel);
                }

                else
                {
                    Plugin.FChatClient.SendMessageInChannel("You can't use Tackle when you already are in grappling range.", Plugin.Channel);
                }
            }


            else if (Plugin.CurrentTeamBattlefield.IsActive)

            {       
                if (Plugin.CurrentTeamBattlefield.GetTarget().IsGrabbable != Plugin.CurrentTeamBattlefield.GetActor().IsGrabbable && Plugin.CurrentTeamBattlefield.GetTarget().IsGrabbable < 20)
                {
                    base.ExecuteCommand(character, args, channel);
                }
                else if (Plugin.CurrentTeamBattlefield.GetActor().IsGrabbable == 0 && Plugin.CurrentTeamBattlefield.GetTarget().IsGrabbable < 20)
                {
                    base.ExecuteCommand(character, args, channel);
                }

                else
                {
                    Plugin.FChatClient.SendMessageInChannel("You can't use Tackle here.", Plugin.Channel);
                }
            }

            else
            {
                Plugin.FChatClient.SendMessageInChannel("There is no fight going on right now.", Plugin.Channel);
            }

        }
    }
}
