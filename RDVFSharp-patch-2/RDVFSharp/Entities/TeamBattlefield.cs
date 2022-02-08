using RDVFSharp.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RDVFSharp
{
    public class TeamBattlefield
    {
        public List<TeamFighter> TeamFighters { get; set; }
        public TeamFighter Fighter { get; set; }
        public string Stage { get; set; }
        public bool DisplayGrabbed { get; set; }
        public WindowController WindowController { get; set; }
        private int currentTeamFighter = 0;
        private int SetTarget = 0;
        public bool InGrabRange { get; set; }
        public RendezvousFighting Plugin { get; }

        public bool IsInFight(string character)
        {
            return TeamFighters.Any(x => x.Name.ToLower() == character.ToLower());
        }

        public TeamFighter GetFighter(string character)
        {
            return TeamFighters.FirstOrDefault(x => x.Name.ToLower() == character.ToLower());
        }

        public TeamFighter GetFighterTarget(string character)
        {
            return TeamFighters.FirstOrDefault(x => x.Name.ToLower() != character.ToLower());
        }


        public bool IsActive { get; set; }

        public TeamBattlefield(RendezvousFighting plugin)
        {
            Plugin = plugin;
            WindowController = new WindowController();
            TeamFighters = new List<TeamFighter>();
            Stage = PickStage();
            InGrabRange = false;
            DisplayGrabbed = true;
            IsActive = false;
        }

        public void InitialSetup(TeamFighter firstTeamFighter, TeamFighter secondTeamFighter, TeamFighter thirdTeamFighter, TeamFighter fourthTeamFighter)
        {
            TeamFighters.Clear();
            TeamFighters.Add(firstTeamFighter);
            TeamFighters.Add(secondTeamFighter);
            TeamFighters.Add(thirdTeamFighter);
            TeamFighters.Add(fourthTeamFighter);

            PickInitialActor();
            WindowController.Hit.Add("Game started!");
            WindowController.Hit.Add("FIGHTING STAGE: " + Stage + " - " + GetActor().Name + " goes first!");
            WindowController.Hit.Add("The initial target for " + GetActor().Name + " is " + GetTarget().Name + ", and vice versa.");
            WindowController.Hit.Add("The initial target for " + GetPartner().Name + " is " + GetOther().Name + ", and vice versa.");
            WindowController.Hit.Add("To change targets, use the !changetarget command");
            WindowController.Hit.Add("The teams are " + GetActor().Name + " and " + GetPartner().Name + " vs " + GetTarget().Name + " and " + GetOther().Name + "!");
            OutputTeamFighterstatus(); // Creates the fighter status blocks (HP/Mana/Stamina)
            OutputTeamFighterstats(); // Creates the fighter stat blocks (STR/DEX/END/INT/WIL)
            WindowController.Info.Add("[url=http://www.f-list.net/c/rendezvous%20fight/]Visit this page for game information[/url]");
            IsActive = true;
            WindowController.UpdateOutput(this);
        }

        public BaseTeamFight EndFight(TeamFighter victor1, TeamFighter victor2, TeamFighter loser1, TeamFighter loser2)
        {
            var fightResult = new BaseTeamFight()
            {
                Room = Plugin.Channel,
                Winner1Id = victor1.Name,
                Winner2Id = victor2.Name,
                Loser1Id = loser1.Name,
                Loser2Id = loser2.Name,
                FinishDate = DateTime.UtcNow
            };

            using (var context = Plugin.Context)
            {
                context.Add(fightResult);
                context.SaveChanges();
            }
            

            Plugin.ResetTeamFight();

            return fightResult;
        }

        public bool IsThisCharactersTurn(string characterName)
        {
            return GetActor().Name == characterName;
        }

        public bool IsAbleToAttack(string characterName)
        {
            return IsActive && IsThisCharactersTurn(characterName);
        }
        
        public void TakeAction(string actionMade)
        {
            var action = actionMade;
            var actor = GetActor();
            var roll = Utils.RollDice(20);
            while (actor.LastRolls.IndexOf(roll) != -1)
            {
                roll = Utils.RollDice(20);
            }
            actor.LastRolls.Add(roll);
            if (actor.LastRolls.Count > 5)
            {
                actor.LastRolls.RemoveAt(0);
            }
            Console.WriteLine(actor.LastRolls);
            var luck = 0; //Actor's average roll of the fight.

            WindowController.Action.Add(action);

            // Update tracked sum of all rolls and number of rolls the actor has made. Then calculate average value of actor's rolls in this fight.
            actor.RollTotal += roll;
            actor.RollsMade += 1;
            if (actor.RollsMade > 0)
            {
                luck = (int)Math.Round((double)actor.RollTotal / actor.RollsMade);
            }
            Type fighterType = actor.GetType();
            MethodInfo theMethod = fighterType.GetMethod("Action" + action);
            theMethod.Invoke(actor, new object[] { roll });

            WindowController.Info.Add("Raw Dice Roll: " + roll);
            WindowController.Info.Add(actor.Name + "'s Average Dice Roll: " + luck);
            if (roll == 20) WindowController.Info.Add("\n" + "[eicon]d20crit[/eicon]" + "\n");//Test to see if this works. Might add more graphics in the future.

            TurnUpKeep(); //End of turn upkeep (Stamina regen, check for being stunned/knocked out, etc.)
            OutputTeamFighterstatus(); // Creates the fighter status blocks (HP/Mana/Stamina)
                                       //Battlefield.outputTeamFighterstats();
            WindowController.Hint.Add(GetActor().Name + "'s next target is " + GetTarget().Name);
            WindowController.UpdateOutput(this); //Tells the window controller to format and dump all the queued up messages to the results screen.
        }

        //public bool AddFighter(ArenaSettings settings)
        //{
        //    try
        //    {
        //        TeamFighters.Add(new Fighter(this, settings)); //TODO
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //        return false;
        //    }
        //    return true;
        //}

        public void ClearTeamFighters()
        {
            TeamFighters.Clear();
        }

        public void ChangeTarget()
        {
            if
                (TeamFighters[currentTeamFighter].SetTarget == 1) SetTarget = 1;
            else if
                (TeamFighters[currentTeamFighter].SetTarget == 0) SetTarget = 0;
            else
                SetTarget = 0;
        }


        public TeamFighter GetTarget()
        {
            if (currentTeamFighter == 0)
                return TeamFighters[3 - currentTeamFighter - SetTarget];

            else if (currentTeamFighter == 1)
                return TeamFighters[3 - currentTeamFighter + SetTarget];

            else if (currentTeamFighter == 2)
                return TeamFighters[3 - currentTeamFighter - SetTarget];

            else
                return TeamFighters[3 - currentTeamFighter + SetTarget];
        }


        public TeamFighter GetActor()
        {
            return TeamFighters[currentTeamFighter];
        }

        public TeamFighter GetPartner()
        {
            if (currentTeamFighter == 0)
                return TeamFighters[currentTeamFighter + 1];

            else if (currentTeamFighter == 1)
                return TeamFighters[currentTeamFighter - 1];

            else if (currentTeamFighter == 2)
                return TeamFighters[currentTeamFighter + 1];
            else
                return TeamFighters[currentTeamFighter - 1];
        }

        public TeamFighter GetOther()
        {
            if (SetTarget == 0 && currentTeamFighter == 0)
                return TeamFighters[currentTeamFighter + 2];

            else if (SetTarget == 1 && currentTeamFighter == 0)
                return TeamFighters[currentTeamFighter + 3];

            else if (SetTarget == 0 && currentTeamFighter == 1)
                return TeamFighters[currentTeamFighter + 2];

            else if (SetTarget == 1 && currentTeamFighter == 1)
                return TeamFighters[currentTeamFighter + 1];

            else if (SetTarget == 0 && currentTeamFighter == 2)
                return TeamFighters[currentTeamFighter - 2];

            else if (SetTarget == 1 && currentTeamFighter == 2)
                return TeamFighters[currentTeamFighter - 1];

            else if (SetTarget == 0 && currentTeamFighter == 3)
                return TeamFighters[currentTeamFighter - 2];

            else
                return TeamFighters[currentTeamFighter - 3];
        }

        public void OutputTeamFighterstatus()
        {
            for (int i = 0; i < TeamFighters.Count; i++)
            {
                WindowController.Status.Add(TeamFighters[i].GetStatus());
            }
        }

        public void OutputTeamFighterstats()
        {
            for (int i = 0; i < TeamFighters.Count; i++)
            {
                WindowController.Status.Add(TeamFighters[i].GetStatBlock());
            }
        }

        public void NextFighter()
        {
            
            if
                (currentTeamFighter < 2) currentTeamFighter += 2;

            else if
                (currentTeamFighter < 3) currentTeamFighter -= 1; 

            else
             currentTeamFighter = 0; 

            if (TeamFighters[currentTeamFighter].IsDazed)
            {
                TeamFighters[currentTeamFighter].IsDazed = false;
                NextFighter();
            }

            if (TeamFighters[currentTeamFighter].IsStunned > 1)
            {
                TeamFighters[currentTeamFighter].IsStunned -= 1;
                NextFighter();
            }
        }

        public void PickInitialActor()
        {
            currentTeamFighter = Utils.GetRandomNumber(0, TeamFighters.Count);
        }

        public string PickStage()
        {
            var stages = new List<string>() {
            "The Pit",
            "RF:Wrestling Ring",
            "Arena",
            "Subway",
            "Skyscraper Roof",
            "Forest",
            "Cafe",
            "Street road",
            "Alley",
            "Park",
            "RF:MMA Hexagonal Cage",
            "Hangar",
            "Swamp",
            "RF:Glass Box",
            "RF:Free Space",
            "Magic Shop",
            "Locker Room",
            "Library",
            "Pirate Ship",
            "Bazaar",
            "Supermarket",
            "Night Club",
            "Docks",
            "Hospital",
            "Dark Temple",
            "Restaurant",
            "Graveyard",
            "Zoo",
            "Slaughterhouse",
            "Junkyard",
            "Theatre",
            "Circus",
            "Castle",
            "Museum",
            "Beach",
            "Bowling Club",
            "Concert Stage",
            "Wild West Town",
            "Movie Set"
            };

            return stages[Utils.GetRandomNumber(0, stages.Count - 1)];
        }

        public void TurnUpKeep()
        {
            for (var i = 0; i < TeamFighters.Count; i++)
            {
                TeamFighters[i].UpdateCondition();
            }

            TeamFighters[currentTeamFighter].Regen();
            NextFighter();
            ChangeTarget();
        }
    }
}
