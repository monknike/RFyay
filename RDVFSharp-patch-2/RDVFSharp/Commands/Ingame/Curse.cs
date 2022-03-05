using System.Collections.Generic;

namespace RDVFSharp.Commands
{
    public class Curse : Action
    {
        public override void ExecuteCommand(string character, IEnumerable<string> args, string channel)
        {
            if (Plugin.CurrentTeamBattlefield.IsActive)

            {
                if (Plugin.CurrentTeamBattlefield.GetActor().CurseUsed == 0)
                {
                    base.ExecuteCommand(character, args, channel);
                }

                else
                {
                    Plugin.FChatClient.SendMessageInChannel("You have already used Curse once in this match!", Plugin.Channel);
                }
            }

            else if (Plugin.CurrentBattlefield.IsActive)

            {
                if (Plugin.CurrentBattlefield.GetActor().CurseUsed == 0)
                {
                    base.ExecuteCommand(character, args, channel);
                }

                else
                {
                    Plugin.FChatClient.SendMessageInChannel("You have already used Curse once in this match!", Plugin.Channel);
                }
            }

            else
            {
                Plugin.FChatClient.SendMessageInChannel("There is no match going on right now!", Plugin.Channel);
            }
        }
    }
}
