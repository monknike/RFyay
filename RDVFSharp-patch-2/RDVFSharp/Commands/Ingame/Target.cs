using FChatSharpLib.Entities.Plugin.Commands;
using RDVFSharp.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RDVFSharp.Commands
{
    public class Target : Action
    {
        public override string Description => "Sets your target.";

        public override void ExecuteCommand(string character, IEnumerable<string> args, string channel)
        {
            if (Plugin.CurrentBattlefield.IsAbleToAttack(character))
            {
                if (args.Count() < 1)
                {
                    return;
                }
                var argsList = args.ToList();

                var characterName = string.Join(' ', argsList.Skip(0));

                var activeFighter = Plugin.CurrentBattlefield.GetFighter(character);

                var NewTarget = Plugin.CurrentBattlefield.GetFighter(characterName);
                if ((NewTarget != null) && (NewTarget.TeamColor != activeFighter.TeamColor))
                {
                    {
                        activeFighter.CurrentTarget = NewTarget;
                        Plugin.FChatClient.SendMessageInChannel($"Target successfully set to {NewTarget.Name} for {activeFighter.Name}.", channel);
                    }
                }

                else if (NewTarget.TeamColor == activeFighter.TeamColor)
                {
                    {
                        Plugin.FChatClient.SendMessageInChannel("You cannot target your own team members.", channel);
                    }
                }

                else
                {
                    throw new FighterNotFound(args.FirstOrDefault());
                }
            }
            else
            {
                Plugin.FChatClient.SendMessageInChannel("This is not your turn.", channel);
            }
        }
    }
}
