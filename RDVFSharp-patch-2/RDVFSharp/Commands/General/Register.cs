using FChatSharpLib.Entities.Plugin;
using FChatSharpLib.Entities.Plugin.Commands;
using RDVFSharp.DataContext;
using RDVFSharp.Entities;
using RDVFSharp.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDVFSharp.Commands
{
    public class Register : BaseCommand<RendezvousFighting>
    {
        public override string Description => "Registers a player in the game.";

        public override void ExecuteCommand(string character, IEnumerable<string> args, string channel)
        {
            using (var context = Plugin.Context)
            {
                var fighter = context.Fighters.Find(character);
                if (fighter != null)
                {
                    throw new FighterAlreadyExists(character);
                }

                int[] statsArray;

                try
                {
                    statsArray = Array.ConvertAll(args.ToArray(), int.Parse);

                    if (statsArray.Length != 5)
                    {
                        throw new Exception();
                    }
                }
                catch (Exception)
                {
                    throw new ArgumentException("Invalid arguments. All stats must be numbers. Example: !register 5 8 8 1 2");
                }

                var createdFighter = new BaseFighter()
                {
                    Name = character,
                    Strength = statsArray[0],
                    Dexterity = statsArray[1],
                    Resilience = statsArray[2],
                    Spellpower = statsArray[3],
                    Willpower = statsArray[4]
                };

                if (createdFighter.AreStatsValid)
                {
                    context.Fighters.Add(createdFighter);
                    context.SaveChanges();
                    Plugin.FChatClient.SendMessageInChannel($"Welcome among us, {character}!", channel);
                }
                else
                {
                    throw new Exception(string.Join(", ", createdFighter.GetStatsErrors()));
                }
            }

        }
        public override void ExecutePrivateCommand(string character, IEnumerable<string> args)
        {
            using (var context = Plugin.Context)
            {
                var fighter = context.Fighters.Find(character);
                if (fighter != null)
                {
                    { Plugin.FChatClient.SendPrivateMessage("Error: Fighter already exists! If you want to change stats, use !restat instead", character); };
                }


                int[] statsArray;

                
                {
                    statsArray = Array.ConvertAll(args.ToArray(), int.Parse);

                    if (statsArray.Length != 5)
                    {
                        { Plugin.FChatClient.SendPrivateMessage("All stats must have a value attached to them. !register STR DEX RES SPW WIL, e.g. !register 6 8 8 0 2", character); }; ;
                    }
                }
                

                var createdFighter = new BaseFighter()
                {
                    Name = character,
                    Strength = statsArray[0],
                    Dexterity = statsArray[1],
                    Resilience = statsArray[2],
                    Spellpower = statsArray[3],
                    Willpower = statsArray[4]
                };

                if (createdFighter.AreStatsValid)
                {
                    context.Fighters.Add(createdFighter);
                    context.SaveChanges();
                    Plugin.FChatClient.SendPrivateMessage($"Welcome among us, {character}!", character);
                }
                else
                {
                    { Plugin.FChatClient.SendPrivateMessage("Error: Fighter stats incorrectly formatted. Remember, you have 24 stat points, each stat between 0 and 10, and written as !register STR DEX RES SPW WIL. E.g. !register 6 8 8 0 2", character); };
                }
            }

        }
    }
}
