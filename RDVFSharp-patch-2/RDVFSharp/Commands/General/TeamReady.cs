using FChatSharpLib.Entities.Plugin;
using FChatSharpLib.Entities.Plugin.Commands;
using RDVFSharp.DataContext;
using RDVFSharp.Entities;
using RDVFSharp.Errors;
using System;
using System.Collections.Generic;
using System.Text;

namespace RDVFSharp.Commands
{
    public class TeamReady : BaseCommand<RendezvousFighting>
    {
        public override string Description => "Sets a player as ready.";

        public override void ExecuteCommand(string character ,IEnumerable<string> args, string channel)
        {
            if (Plugin.CurrentTeamBattlefield.IsActive)
            {
                throw new FightInProgress();
            }
            else if (Plugin.FirstTeamFighter?.Name == character || Plugin.SecondTeamFighter?.Name == character || Plugin.ThirdTeamFighter?.Name == character || Plugin.FourthTeamFighter?.Name == character)
            {
                throw new FighterAlreadyExists(character);
            }

            BaseFighter fighter = null;

            using (var context = Plugin.Context)
            {
                fighter = context.Fighters.Find(character);
            }

            if (fighter == null)
            {
                throw new FighterNotRegistered(character);
            }

            var actualTeamFighter = new TeamFighter(fighter, Plugin.CurrentTeamBattlefield);

            if (Plugin.ThirdTeamFighter == null && Plugin.FirstTeamFighter != null && Plugin.SecondTeamFighter != null)
            {
                Plugin.ThirdTeamFighter = actualTeamFighter;
                Plugin.FChatClient.SendMessageInChannel($"{actualTeamFighter.Name} joined the fight!", channel);
            }

            if (Plugin.SecondTeamFighter == null && Plugin.FirstFighter != null)
            {
                Plugin.SecondTeamFighter = actualTeamFighter;
                Plugin.FChatClient.SendMessageInChannel($"{actualTeamFighter.Name} joined the fight!", channel);
            }

            if (Plugin.FirstTeamFighter == null)
            {
                Plugin.FirstTeamFighter = actualTeamFighter;
                Plugin.FChatClient.SendMessageInChannel($"{actualTeamFighter.Name} joined the fight!", channel);
            }

            else
            {
                Plugin.FourthTeamFighter = actualTeamFighter;

                if (!Plugin.CurrentTeamBattlefield.IsActive && (Plugin.FirstTeamFighter != null && Plugin.SecondTeamFighter != null && Plugin.ThirdTeamFighter != null && Plugin.FourthTeamFighter != null))
                {
                    Plugin.FChatClient.SendMessageInChannel($"{actualTeamFighter.Name} accepted the challenge! Let's get it on!", channel);
                    Plugin.FChatClient.SendMessageInChannel(Constants.VCAdvertisement, channel);
                    Plugin.CurrentTeamBattlefield.InitialSetup(Plugin.FirstTeamFighter, Plugin.SecondTeamFighter, Plugin.ThirdTeamFighter, Plugin.FourthTeamFighter);
                }
            }

        }
    }
}
