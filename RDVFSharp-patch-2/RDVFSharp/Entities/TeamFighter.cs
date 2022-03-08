using System;
using System.Collections.Generic;
using System.Text;

namespace RDVFSharp.Entities
{
    public class TeamFighter
    {
        public TeamBattlefield TeamBattlefield { get; set; }
        public BaseFighter BaseFighter { get; set; }
        public string Name
        {
            get
            {
                return BaseFighter.Name;
            }
        }

        public int Strength
        {
            get
            {
                var total = BaseFighter.Strength;
                return total;
            }
        }
        public int Dexterity
        {
            get
            {
                var total = BaseFighter.Dexterity;
                return total;
            }
        }
        public int Endurance
        {
            get
            {
                var total = BaseFighter.Resilience;
                return total;
            }
        }
        public int Spellpower
        {
            get
            {
                var total = BaseFighter.Spellpower;
                return total;
            }
        }
        public int Willpower
        {
            get
            {
                var total = BaseFighter.Willpower;
                return total;
            }
        }

        public int HP { get; set; }
        private int MaxHP { get; set; }
        public int HPDOT { get; set; }
        public int ManaDOT { get; set; }
        public int StaminaDOT { get; set; }
        public int ManaDamage { get; set; }
        public int StaminaDamage { get; set; }
        public int Mana { get; set; }
        public int ManaCap { get; set; }
        public int MaxMana { get; set; }
        public int Stamina { get; set; }
        public int StaminaCap { get; set; }
        public int MaxStamina { get; set; }
        public int DizzyValue { get; set; }
        public int ManaBurn { get; set; }
        public int StaminaBurn { get; set; }
        public int HPBurn { get; set; }
        public int DamageEffectMult { get; set; }
        public bool IsUnconscious { get; set; }
        public bool IsDead { get; set; }
        public int CurseUsed { get; set; }
        public bool IsRestrained { get; set; }
        public bool IsRestraining { get; set; }
        public bool IsDazed { get; set; }
        public int IsStunned { get; set; }
        public int IsDisoriented { get; set; }
        public List<string> IsGrappledBy { get; set; }
        public int IsGrabbable { get; set; }
        public int IsFocused { get; set; }
        public int IsEscaping { get; set; }
        public int IsGuarding { get; set; }
        public int IsEvading { get; set; }
        public int IsAggressive { get; set; }
        public int IsExposed { get; set; }
        public bool Fumbled { get; set; }
        public int KoValue { get; set; }
        public int DeathValue { get; set; }
        public int RollTotal { get; set; }
        public int RollsMade { get; set; }
        public List<int> LastRolls { get; set; }
        public bool WantsToLeave { get; set; }
        public int LastKnownHP { get; set; }
        public int LastKnownMana { get; set; }
        public int LastKnownStamina { get; set; }
        public int SetTarget { get; set; }


        public TeamFighter(BaseFighter baseFighter, TeamBattlefield Teambattlefield)
        {
            BaseFighter = baseFighter;
            TeamBattlefield = Teambattlefield;
            KoValue = Constants.DefaultUnconsciousAt;
            DeathValue = Constants.DefaultDeadAt;

            

            MaxHP = BaseFighter.BaseMaxHP;
            MaxMana = BaseFighter.BaseMaxMana;
            ManaCap = MaxMana;
            MaxStamina = BaseFighter.BaseMaxStamina;
            StaminaCap = MaxStamina;

            DizzyValue = (int)(MaxHP * Constants.DefaultDizzyHPMultiplier); //You become dizzy at half health and below.

            ManaBurn = 0;
            StaminaBurn = 0;

            DamageEffectMult = Constants.DefaultGameSpeed;

            HP = 0;
            AddHp(MaxHP);

            Mana = 0;
            AddMana(MaxMana);

            Stamina = 0;
            AddStamina(MaxStamina);

            RollTotal = 0; // Two values that we track in order to calculate average roll, which we will call Luck on the output screen.
            RollsMade = 0; // Luck = rollTotal / rollsMade
            LastRolls = new List<int>();

            LastKnownHP = HP;
            LastKnownMana = Mana;
            LastKnownStamina = Stamina;
            IsGrabbable = 0;
            IsUnconscious = false;
            IsDead = false;
            CurseUsed = 0;
            IsRestrained = false;
            IsRestraining = false;
            IsDazed = false;
            IsStunned = 0;
            IsDisoriented = 0;
            IsGrappledBy = new List<string>();
            IsFocused = 0;
            IsEscaping = 0;//A bonus to escape attempts that increases whenever you fail one.
            IsGuarding = 0;
            IsEvading = 0;
            IsAggressive = 0;
            IsExposed = 0;
            SetTarget = 0;
            Fumbled = false; //A status that gets set when you fumble, so that opponents next action can stun you.
            WantsToLeave = false;
        }

        public void AddHp(int hpToAdd)
        {
            var x = ~~hpToAdd;
            HP += x * DamageEffectMult;
            HP = Utils.Clamp(HP, 0, MaxHP);
        }

        public void AddMana(int manaToAdd)
        {
            var x = ~~manaToAdd;
            Mana += x;
            Mana = Utils.Clamp(Mana, 0, MaxMana);
        }

        public void AddStamina(int staminaToAdd)
        {
            var x = ~~staminaToAdd;
            Stamina += x;
            Stamina = Utils.Clamp(Stamina, 0, MaxStamina);
        }

        public void HitHp(int hpToRemove)
        {
            var x = ~~hpToRemove;
            x *= DamageEffectMult;
            HP -= x;
            HP = Utils.Clamp(HP, 0, MaxHP);
            TeamBattlefield.WindowController.Damage = x;

            if (IsFocused > 0)
            {
                var doubleX = (double)x;
                if (IsRestrained) doubleX *= 1.5;
                if (IsDisoriented > 0) doubleX += IsDisoriented;
                IsFocused = (int)Math.Max(IsFocused - doubleX, 0);
                if (IsFocused == 0) TeamBattlefield.WindowController.Hint.Add(Name + " has lost their focus!");
            }
        }

        public void HitMana(int manaToRemove)
        {
            var x = ~~manaToRemove;
            Mana -= x;
            Mana = Utils.Clamp(Mana, 0, ManaCap);
        }

        public void HitStamina(int staminaToRemove)
        {
            var x = ~~staminaToRemove;
            Stamina -= x;
            Stamina = Utils.Clamp(Stamina, 0, StaminaCap);
        }

        public string PickFatality()
        {
            var fatalities = new List<string>() {
                    "Decapitation",
                    "Strangulation",
                    "Beating to death",
                    "Exposing internal organs",
                    "Blood loss",
                    "Heart damage",
                    "Brain damage",
                    "Breaking Neck",
                    "Breaking bones",
                    "Dismemberment",
                    "Crushing",
                    "Severing the jaw",
                    "Remove top part of a head",
                    "Maceration",
                    "Brutality!",
                    "Slow and sensual death",
                    "Literally fucking to death",
                    "Extremely staged and theatrical finisher" };

            return fatalities[Utils.GetRandomNumber(0, fatalities.Count - 1)];
        }

        public void Regen()
        {
            if (ManaCap > MaxMana)
            {
                ManaCap = Math.Max(ManaCap - ManaBurn, MaxMana);
                ManaBurn = 10;
            }

            if (ManaCap == MaxMana) ManaBurn = 0;

            if (StaminaCap > MaxStamina)
            {
                StaminaCap = Math.Max(StaminaCap - StaminaBurn, MaxStamina);
                StaminaBurn = 10;
            }

            if (StaminaCap == MaxStamina) StaminaBurn = 0;


            if (HPBurn > 0)
            {
                AddHp(-HPDOT);
            }


            if (StaminaDamage > 0)
            {
                AddStamina(-StaminaDOT);
            }

            if (ManaDamage > 0)
            {
                AddMana(-ManaDOT);
            }

            if (IsUnconscious == false)
            {
                //Disable regeneration.
                //    var stamBonus = 6 + Willpower;
                //    this.addStamina(stamBonus);
                //    var manaBonus = 6 + Willpower;
                //    this.addMana(manaBonus);
            }
            else
            {
                IsDazed = true;
            }
        }

        public string GetStatBlock()
        {
            return "[color=cyan]" + Name + " stats: Strength: " + Strength + " Dexterity: " + Dexterity + " Resilience: " + Endurance + " Spellpower: " + Spellpower + " Willpower: " + Willpower + "[/color]";
        }

        public string GetStatus()
        {
            var hpDelta = HP - LastKnownHP;
            var staminaDelta = Stamina - LastKnownStamina;
            var manaDelta = Mana - LastKnownMana;
            double hpPercent = Math.Ceiling((double)(100 * HP / MaxHP));
            var staminaPercent = Math.Ceiling((double)(100 * Stamina / StaminaCap));
            var manaPercent = Math.Ceiling((double)(100 * Mana / ManaCap));

            var message = "[color=orange]" + Name;
            message += "[/color][color=yellow] hit points: ";
            if (HP > DizzyValue)
            {
                message += HP;
            }
            else
            {
                message += "[color=red]" + HP + "[/color]";
            }
            if (hpDelta > 0) message += "[color=cyan] (+" + hpDelta + ")[/color]";
            if (hpDelta < 0) message += "[color=red] (" + hpDelta + ")[/color]";
            message += "|" + this.MaxHP;
            message += " (" + hpPercent + "%)";

            message += "[/color][color=green] stamina: " + Stamina;
            if (staminaDelta > 0) message += "[color=cyan] (+" + staminaDelta + ")[/color]";
            if (staminaDelta < 0) message += "[color=red] (" + staminaDelta + ")[/color]";

            message += "|";
            if (StaminaCap > MaxStamina) message += "[color=cyan]";
            message += StaminaCap;
            if (StaminaCap > MaxStamina) message += "[/color]";
            message += " (" + staminaPercent + "%)";

            message += "[/color][color=blue] mana: " + Mana;
            if (manaDelta > 0) message += "[color=cyan] (+" + manaDelta + ")[/color]";
            if (manaDelta < 0) message += "[color=red] (" + manaDelta + ")[/color]";

            message += "|";
            if (ManaCap > MaxMana) message += "[color=cyan]";
            message += ManaCap;
            if (ManaCap > MaxMana) message += "[/color]";
            message += " (" + manaPercent + "%)[/color]";

            LastKnownHP = HP;
            LastKnownMana = Mana;
            LastKnownStamina = Stamina;

            if (IsRestrained) TeamBattlefield.WindowController.Hint.Add(Name + " is Grappled.");
            if (IsFocused > 0) TeamBattlefield.WindowController.Hint.Add(Name + " is Focused (" + IsFocused + " points). Focus is reduced by taking damage.");
            if (IsFocused > 0) TeamBattlefield.WindowController.Hint.Add(Name + "'s Ranged and Spell attacks have a +" + Math.Ceiling((double)IsFocused / 10) + " bonus to attack and damage because of the Focus.");
            
            TeamBattlefield.DisplayGrabbed = !TeamBattlefield.DisplayGrabbed; //only output it on every two turns
            return message;
        }

        public void UpdateCondition()
        {
            var attacker = TeamBattlefield.GetActor();
            var target = TeamBattlefield.GetTarget();
            var partner = TeamBattlefield.GetPartner();
            var other = TeamBattlefield.GetOther();

            if (IsGrappledBy.Count != 0 && IsRestrained == false) IsRestrained = true;
            if (IsGrappledBy.Count == 0 && IsRestrained == true) IsRestrained = false;

            if (IsEscaping > 0 && !IsRestrained || IsEscaping < 0) IsEscaping = 0;//If you have ane scape bonus, but you're not grappled it should get cancled. And the escape bonus can't be negative either.

            if (IsEscaping > 0)
            {
                TeamBattlefield.WindowController.Hint.Add(Name + " has a +" + IsEscaping + " escape bonus.");
            }

            //if (Stamina < rollDice([20]) && IsFocused > 0) {
            //    TeamBattlefield.WindowController.Hint.Add(Name + " lost their focus/aim because of fatigue!");
            //    IsFocused = 0;
            //}






            if (StaminaDamage > 1)

            {
                TeamBattlefield.WindowController.Hint.Add(Name + " is taking " + HPDOT + " damage to both Stamina and HP for " + (HPBurn - 1) + " turn(s).");
            }

            if (ManaDamage > 1)

            {
                TeamBattlefield.WindowController.Hint.Add(Name + " is taking " + HPDOT + " damage to both Mana and HP for " + (HPBurn - 1) + " turn(s).");
            }

            if (IsGuarding > 0)
            {
                TeamBattlefield.WindowController.Hint.Add(Name + " has a temporary +" + IsGuarding + " bonus to evasion and damage reduction.");
            }

            if (IsEvading > 0)
            {
                TeamBattlefield.WindowController.Hint.Add(Name + " has a temporary +" + IsEvading + " bonus to evasion and damage reduction.");
            }

            if ((attacker.IsGrabbable == target.IsGrabbable) && (attacker.IsGrabbable > 0) && (attacker.IsGrabbable < 20) & this == target)
            {
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " and " + target.Name + " are in grappling range.");
            }
            
            if (IsAggressive > 0)
            {
                TeamBattlefield.WindowController.Hint.Add(Name + " has a temporary +" + IsAggressive + " bonus to accuracy and attack damage.");
            }

            if (IsExposed > 0)
            {
                IsExposed -= 1;
                if (IsExposed == 0) TeamBattlefield.WindowController.Hint.Add(Name + " has recovered from the missed attack and is no longer Exposed!");
            }

            if (HP <= KoValue && IsUnconscious == false)
            {
                IsUnconscious = true;
                //TeamBattlefield.WindowController.Hit.Add(Name + " is permanently Knocked Out (or extremely dizzy, and can not resist)! Feel free to use this opportunity! " + Name + " must not resist! Continue beating them to get a fatality suggestion.");
            }

            if (HP <= DeathValue && IsDead == false)
            {
                IsDead = true;
                IsGrabbable = 0;
                IsStunned = 2147483647;
                CurseUsed = 0;
                IsRestrained = false;
                IsDazed = false;
                IsDisoriented = 0;
                IsGrappledBy = new List<string>();
                IsFocused = 0;
                IsEscaping = 0;//A bonus to escape attempts that increases whenever you fail one.
                IsGuarding = 0;
                IsEvading = 0;
                IsAggressive = 0;
                IsExposed = 0;
                SetTarget = 0;
                HPDOT = 0;
                HPBurn = 0;
                TeamBattlefield.GetActor().IsEscaping = 0;
                TeamBattlefield.GetActor().IsRestrained = false;
                TeamBattlefield.WindowController.Hit.Add(Name + " has been taken out! Eliminate their partner to win the match! (Your target has automatically changed)");
                
            }

            if (target.IsDead == true)

            {

                attacker.RemoveGrappler(target);
                partner.RemoveGrappler(target);
                if (attacker.SetTarget == 1)
                    attacker.SetTarget -= 1;
                if (attacker.SetTarget == 0)
                    attacker.SetTarget += 1;
            }

            if (other.IsDead == true)
            { 
                attacker.RemoveGrappler(other);
                partner.RemoveGrappler(other);
            }

            if (partner.IsDead == true)
            {
                other.RemoveGrappler(partner);
                target.RemoveGrappler(partner);
            }


            if (target.IsDead == true & other.IsDead == true & this == target)
            {
                TeamBattlefield.WindowController.Hit.Add("The fight is over! CLAIM YOUR SPOILS and VICTORY and FINISH YOUR OPPONENT!");
                TeamBattlefield.WindowController.Special.Add("FATALITY SUGGESTION: " + this.PickFatality());
                TeamBattlefield.WindowController.Special.Add("It is just a suggestion, you may not follow it if you don't want to.");
                TeamBattlefield.EndFight(TeamBattlefield.GetActor(), TeamBattlefield.GetPartner(), TeamBattlefield.GetTarget(), TeamBattlefield.GetOther());
            }


        }

        public (int miss, int crit, int targethit) BuildActionTable(int difficulty, int targetDex, int attackerDex, int targetEnergy, int targetEnergyMax)
        {
            var miss = 0;
            var crit = 0;
            var targethit = 0;
            // Modify difficulty by half the difference in DEX rounded down. Each odd point more gives you +1 attack and each even point more gives you +1 defence.
            miss = difficulty + (int)Math.Floor((double)(targetDex - attackerDex) / 2);
            //Opponents who are low on energy are easier to hit. Will use Stamina for physical and Mana for magical attacks.
            miss = (int)Math.Ceiling((double)miss * targetEnergy / targetEnergyMax);
            miss = Math.Max(1, miss);//A roll of 1 is always a miss.
            miss = Math.Min(miss, 19); //A roll of 20 is always a hit, so maximum difficulty is 19.
            crit = 20;
            targethit = 7;
            return (miss, crit, targethit);
        }

        public bool ActionLight(int roll)
        {
            var attacker = this;
            var target = TeamBattlefield.GetTarget();
            var partner = TeamBattlefield.GetPartner();
            var othertarget = TeamBattlefield.GetOther();
            var damage = Utils.RollDice(new List<int>() { 5, 5 }) - 1 + attacker.Strength;
            damage += Math.Min(attacker.Strength, attacker.Spellpower);
            var requiredStam = 5;
            var difficulty = 6;

            if (target.IsDead == false)
            {
                if (target.Fumbled)
            {
                target.IsDazed = true;
                target.Fumbled = false;
            }

            if (attacker.IsRestrained) difficulty += 2; //Up the difficulty if the attacker is restrained.
            if (target.IsRestrained) difficulty -= 4; //Lower it if the target is restrained.
            if (target.IsExposed > 0) difficulty -= 2; // If opponent left themself wide open after a failed strong attack, they'll be easier to hit.


                if (target.IsGuarding > 0)
            {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += target.IsGuarding; 
                    damage -= target.IsGuarding;
            }

            if (attacker.IsEvading > 0)
            {//Apply attack bonus from move/teleport then reset it.
                attacker.IsEvading = 0;
            }
            if (target.IsEvading > 0)
            {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                difficulty += target.IsEvading;
                damage -= target.IsEvading;
                
            }
            if (attacker.IsAggressive > 0)
            {//Apply attack bonus from move/teleport then reset it.
                difficulty -= attacker.IsAggressive;
                damage += attacker.IsAggressive;
                attacker.IsAggressive = 0;
            }

            if (attacker.Stamina < requiredStam)
            {   //Not enough stamina-- reduced effect
                damage *= attacker.Stamina / requiredStam;
                difficulty += (int)Math.Ceiling((double)((requiredStam - attacker.Stamina) / requiredStam) * (20 - difficulty)); // Too tired? You might miss more often.
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " did not have enough stamina, and took penalties to the attack.");
            }

            attacker.HitStamina(requiredStam);

            var attackTable = attacker.BuildActionTable(difficulty, target.Dexterity, attacker.Dexterity, target.Stamina, target.StaminaCap);
            //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
            TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));

            if (roll <= attackTable.miss)
            {   //Miss-- no effect.
                TeamBattlefield.WindowController.Hit.Add(" FAILED! ");
                return false; //Failed attack, if we ever need to check that.
            }

            if (roll >= attackTable.crit)
            { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                TeamBattlefield.WindowController.Hit.Add(" CRITICAL HIT! ");
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " landed a particularly vicious blow!");
                damage += 10;
            }
            else
            { //Normal hit.
                TeamBattlefield.WindowController.Hit.Add(" HIT! ");
            }

            //Deal all the actual damage/effects here.

            if (TeamBattlefield.InGrabRange)
            {// Succesful attacks will beat back the grabber before they can grab you, but not if you're already grappling.
                if (!attacker.IsRestrained && !target.IsRestrained)
                {
                    TeamBattlefield.InGrabRange = false;
                    TeamBattlefield.WindowController.Hit.Add(attacker.Name + " distracted " + target.Name + " with the attack and was able to move out of grappling range!");
                }
            }

            //If you're being grappled and you hit the opponent that will make it a little easier to escape later on.
            if (attacker.IsRestrained) attacker.IsEscaping += (int)Math.Floor((double)damage / 5);
            
                damage = Math.Max(damage, 1);
                target.HitHp(damage);
                target.HitStamina(damage);
                attacker.IsGrabbable = 0;
                target.IsGrabbable = 0;
                return true; //Successful attack, if we ever need to check that.
            }

            else
            {
                if (othertarget.Fumbled)
                {
                    othertarget.IsDazed = true;
                    othertarget.Fumbled = false;
                }

                if (attacker.IsRestrained) difficulty += 2; //Up the difficulty if the attacker is restrained.
                if (othertarget.IsRestrained) difficulty -= 4; //Lower it if the target is restrained.
                if (othertarget.IsExposed > 0) difficulty -= 2; // If opponent left themself wide open after a failed strong attack, they'll be easier to hit.
               
                if (othertarget.IsGuarding > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += othertarget.IsGuarding;
                    damage -= othertarget.IsGuarding;
                }

                if (attacker.IsGuarding > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsGuarding = 0;
                }
                if (othertarget.IsEvading > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += othertarget.IsEvading;
                    damage -= othertarget.IsEvading;
                }
                if (attacker.IsAggressive > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    difficulty -= attacker.IsAggressive;
                    damage += attacker.IsAggressive;
                    attacker.IsAggressive = 0;
                }

                if (attacker.IsEvading > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsEvading = 0;
                }

                if (attacker.Stamina < requiredStam)
                {   //Not enough stamina-- reduced effect
                    damage *= attacker.Stamina / requiredStam;
                    difficulty += (int)Math.Ceiling((double)((requiredStam - attacker.Stamina) / requiredStam) * (20 - difficulty)); // Too tired? You might miss more often.
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " did not have enough stamina, and took penalties to the attack.");
                }

                attacker.HitStamina(requiredStam);

                var attackTable = attacker.BuildActionTable(difficulty, othertarget.Dexterity, attacker.Dexterity, othertarget.Stamina, othertarget.StaminaCap);
                //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
                TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));

                if (roll <= attackTable.miss)
                {   //Miss-- no effect.
                    TeamBattlefield.WindowController.Hit.Add(" FAILED! ");
                    return false; //Failed attack, if we ever need to check that.
                }

                if (roll >= attackTable.crit)
                { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                    TeamBattlefield.WindowController.Hit.Add(" CRITICAL HIT! ");
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " landed a particularly vicious blow!");
                    damage += 10;
                }
                else
                { //Normal hit.
                    TeamBattlefield.WindowController.Hit.Add(" HIT! ");
                }

                //Deal all the actual damage/effects here.

                if (TeamBattlefield.InGrabRange)
                {// Succesful attacks will beat back the grabber before they can grab you, but not if you're already grappling.
                    if (!attacker.IsRestrained && !othertarget.IsRestrained)
                    {
                        TeamBattlefield.InGrabRange = false;
                        TeamBattlefield.WindowController.Hit.Add(attacker.Name + " distracted " + target.Name + " with the attack and was able to move out of grappling range!");
                    }
                }

                //If you're being grappled and you hit the opponent that will make it a little easier to escape later on.
                if (attacker.IsRestrained) attacker.IsEscaping += (int)Math.Floor((double)damage / 5); damage = Math.Max(damage, 1);
                othertarget.HitHp(damage);
                othertarget.HitStamina(damage);
                attacker.IsGrabbable = 0;
                othertarget.IsGrabbable = 0;
                return true; //Successful attack, if we ever need to check that.
            }
        }

        public bool ActionHeavy(int roll)
        {
            var attacker = this;
            var target = TeamBattlefield.GetTarget();
            var partner = TeamBattlefield.GetPartner();
            var othertarget = TeamBattlefield.GetOther();
            var damage = Utils.RollDice(new List<int>() { 5, 5 }) - 1 + attacker.Strength;
            damage *= 2;
            damage += Math.Min(attacker.Strength, attacker.Spellpower);
            var requiredStam = 10;
            var difficulty = 8; //Base difficulty, rolls greater than this amount will hit.

            if (target.IsDead == false)
            {//If opponent fumbled on their previous action they should become stunned.
                if (target.Fumbled)
                {
                    target.IsDazed = true;
                    target.Fumbled = false;
                }

                if (attacker.IsRestrained) difficulty += 2; //Up the difficulty if the attacker is restrained.
                if (target.IsRestrained) difficulty -= 4; //Lower it if the target is restrained.
                if (target.IsExposed > 0) difficulty -= 2; // If opponent left themself wide open after a failed strong attack, they'll be easier to hit.
               
                if (target.IsEvading > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += target.IsEvading;
                    damage -= target.IsEvading;
                }
                if (attacker.IsAggressive > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    difficulty -= attacker.IsAggressive;
                    damage += attacker.IsAggressive;
                    attacker.IsAggressive = 0;
                }
                if (target.IsGuarding > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += target.IsGuarding;
                    damage -= target.IsGuarding;
                }

                if (attacker.IsEvading > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsEvading = 0;
                }

                var critCheck = true;
                if (attacker.Stamina < requiredStam)
                {   //Not enough stamina-- reduced effect
                    critCheck = false;
                    damage *= attacker.Stamina / requiredStam;
                    difficulty += (int)Math.Ceiling((double)((requiredStam - attacker.Stamina) / requiredStam) * (20 - difficulty)); // Too tired? You're likely to miss.
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " did not have enough stamina, and took penalties to the attack.");
                }

                attacker.HitStamina(requiredStam); //Now that stamina has been checked, reduce the attacker's stamina by the appopriate amount.

                var attackTable = attacker.BuildActionTable(difficulty, target.Dexterity, attacker.Dexterity, target.Stamina, target.StaminaCap);
                //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
                TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));

                if (roll <= attackTable.miss)
                {   //Miss-- no effect.
                    TeamBattlefield.WindowController.Hit.Add(" FAILED! ");
                    attacker.IsExposed += 4; //If the fighter misses a big attack, it leaves them open and they have to recover balance which gives the opponent a chance to strike.
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " was left wide open by the failed attack and is now Exposed! Any attacks against " + attacker.Name + " have -2 difficulty to hit, and they can be Grabbed even if fighters are not in grappling range!");
                    return false; //Failed attack, if we ever need to check that.
                }

                if (roll >= attackTable.crit && critCheck == true)
                { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                    TeamBattlefield.WindowController.Hit.Add(" CRITICAL HIT! ");
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " landed a particularly vicious blow!");
                    damage += 10;
                }
                else
                { //Normal hit.
                    TeamBattlefield.WindowController.Hit.Add(" HIT! ");
                }

                //Deal all the actual damage/effects here.

                if (TeamBattlefield.InGrabRange)
                {// Succesful attacks will beat back the grabber before they can grab you, but not if you're already grappling.
                    if (!attacker.IsRestrained && !target.IsRestrained)
                    {
                        TeamBattlefield.InGrabRange = false;
                        TeamBattlefield.WindowController.Hit.Add(attacker.Name + " distracted " + target.Name + " with the attack and was able to move out of grappling range!");
                    }
                }

                //If you're being grappled and you hit the opponent that will make it a little easier to escape later on.
                if (attacker.IsRestrained) attacker.IsEscaping += (int)Math.Floor((double)damage / 5);

                damage = Math.Max(damage, 1);
                target.HitHp(damage);
                attacker.IsGrabbable = 0;
                target.IsGrabbable = 0;
                return true; //Successful attack, if we ever need to check that.
            }
            else
            {
                if (othertarget.Fumbled)
                {
                    othertarget.IsDazed = true;
                    othertarget.Fumbled = false;
                }

                if (attacker.IsRestrained) difficulty += 2; //Up the difficulty if the attacker is restrained.
                if (othertarget.IsRestrained) difficulty -= 4; //Lower it if the target is restrained.
                if (othertarget.IsExposed > 0) difficulty -= 2; // If opponent left themself wide open after a failed strong attack, they'll be easier to hit.
               
                if (othertarget.IsEvading > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += othertarget.IsEvading;
                    damage -= othertarget.IsEvading;
                }
                if (attacker.IsAggressive > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    difficulty -= attacker.IsAggressive;
                    damage += attacker.IsAggressive;
                    attacker.IsAggressive = 0;
                }
                if (othertarget.IsGuarding > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += othertarget.IsGuarding;
                    damage -= othertarget.IsGuarding;
                }

                if (attacker.IsEvading > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsEvading = 0;
                }

                var critCheck = true;
                if (attacker.Stamina < requiredStam)
                {   //Not enough stamina-- reduced effect
                    critCheck = false;
                    damage *= attacker.Stamina / requiredStam;
                    difficulty += (int)Math.Ceiling((double)((requiredStam - attacker.Stamina) / requiredStam) * (20 - difficulty)); // Too tired? You're likely to miss.
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " did not have enough stamina, and took penalties to the attack.");
                }

                attacker.HitStamina(requiredStam); //Now that stamina has been checked, reduce the attacker's stamina by the appopriate amount.

                var attackTable = attacker.BuildActionTable(difficulty, othertarget.Dexterity, attacker.Dexterity, othertarget.Stamina, othertarget.StaminaCap);
                //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
                TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));

                if (roll <= attackTable.miss)
                {   //Miss-- no effect.
                    TeamBattlefield.WindowController.Hit.Add(" FAILED! ");
                    attacker.IsExposed += 4; //If the fighter misses a big attack, it leaves them open and they have to recover balance which gives the opponent a chance to strike.
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " was left wide open by the failed attack and is now Exposed! Any attacks against " + attacker.Name + " have -2 difficulty to hit, and they can be Grabbed even if fighters are not in grappling range!");
                    return false; //Failed attack, if we ever need to check that.
                }

                if (roll >= attackTable.crit && critCheck == true)
                { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                    TeamBattlefield.WindowController.Hit.Add(" CRITICAL HIT! ");
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " landed a particularly vicious blow!");
                    damage += 10;
                }
                else
                { //Normal hit.
                    TeamBattlefield.WindowController.Hit.Add(" HIT! ");
                }

                //Deal all the actual damage/effects here.

                if (TeamBattlefield.InGrabRange)
                {// Succesful attacks will beat back the grabber before they can grab you, but not if you're already grappling.
                    if (!attacker.IsRestrained && !othertarget.IsRestrained)
                    {
                        TeamBattlefield.InGrabRange = false;
                        TeamBattlefield.WindowController.Hit.Add(attacker.Name + " distracted " + othertarget.Name + " with the attack and was able to move out of grappling range!");
                    }
                }

                //If you're being grappled and you hit the opponent that will make it a little easier to escape later on.
                if (attacker.IsRestrained) attacker.IsEscaping += (int)Math.Floor((double)damage / 5);

                damage = Math.Max(damage, 1);
                othertarget.HitHp(damage);
                attacker.IsGrabbable = 0;
                othertarget.IsGrabbable = 0;
                return true; //Successful attack, if we ever need to check that.
                             }
            }

        public bool ActionGrab(int roll)
        {
            var attacker = this;
            var target = TeamBattlefield.GetTarget();
            var partner = TeamBattlefield.GetPartner();
            var othertarget = TeamBattlefield.GetOther();
            var damage = Utils.RollDice(new List<int>() { 5, 5 }) - 1 + attacker.Strength;
            damage /= 2;
            var requiredStam = 5;
            var difficulty = 6; //Base difficulty, rolls greater than this amount will hit.

            
            if (target.IsDead == false)

            {
                if (target.IsExposed > 0) difficulty -= 2; // If opponent left themself wide open after a failed strong attack, they'll be easier to hit.
               

                if (target.IsEvading > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += target.IsEvading;
                    damage -= target.IsEvading;
                    
                }
                if (attacker.IsAggressive > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    difficulty -= attacker.IsAggressive;
                    damage += attacker.IsAggressive;
                    attacker.IsAggressive = 0;
                }
                if (target.IsGuarding > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += target.IsGuarding;
                    damage -= target.IsGuarding;
                }

                if (attacker.IsGuarding > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsGuarding = 0;
                }

                if (attacker.IsEvading > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsEvading = 0;
                }

                var critCheck = true;
                if (attacker.Stamina < requiredStam)
                {   //Not enough stamina-- reduced effect
                    critCheck = false;
                    damage *= attacker.Stamina / requiredStam;
                    difficulty += (int)Math.Ceiling((double)((requiredStam - attacker.Stamina) / requiredStam) * (20 - difficulty)); // Too tired? You're likely to miss.
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " did not have enough stamina, and took penalties to the attack.");
                }

                attacker.HitStamina(requiredStam); //Now that stamina has been checked, reduce the attacker's stamina by the appopriate amount. (We'll hit the attacker up for the rest on a miss or a dodge).

                //If opponent fumbled on their previous action they should become stunned.
                // We put it down here for Grab so it doesn't interfere with the stun from a crit on moving into range.
                if (target.Fumbled)
                {
                    target.IsDazed = true;
                    target.Fumbled = false;
                }

                var attackTable = attacker.BuildActionTable(difficulty, target.Dexterity, attacker.Dexterity, target.Stamina, target.StaminaCap);
                //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
                TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));

                if (roll <= attackTable.miss)
                {   //Miss-- no effect.
                    TeamBattlefield.WindowController.Hit.Add(" FAILED! ");
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " failed to establish a hold!");
                    return false; //Failed attack, if we ever need to check that.
                }

                if (roll >= attackTable.crit && critCheck)
                { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                    TeamBattlefield.WindowController.Hit.Add(" CRITICAL HIT! ");
                    TeamBattlefield.WindowController.Hint.Add("Critical! " + attacker.Name + " found a particularly good hold!");
                    damage += 10;
                }

                //grab can only be used when you are not grappling the target, so we no longer need the old check.
                TeamBattlefield.WindowController.Hit.Add(attacker.Name + " GRABBED " + target.Name + "! ");
                TeamBattlefield.WindowController.Hint.Add(target.Name + " is being grappled! " + attacker.Name + " has reduced difficulty to use melee attacks and can also use the special attacks Throw and Submission.");
                TeamBattlefield.WindowController.Hint.Add(target.Name + " can try to escape the grapple by using Move, Throw, or Teleport.");
                target.IsGrappledBy.Add(attacker.Name);

                //If we managed to grab without being in grab range, we are certainly in grabe range afterwards.


                damage = Math.Max(damage, 1);
                target.HitHp(damage);
                attacker.IsRestraining = true;
                attacker.IsGrabbable = 40;
                target.IsGrabbable = 40;
                if (TeamBattlefield.GetTargetOfTarget() == partner) 
                    {
                    if (target.SetTarget == 0)
                        target.SetTarget += 1;
                    
                    else
                        target.SetTarget -= 1;
                    }
                return true; //Successful attack, if we ever need to check that.
            }

            else
            {
                if (othertarget.IsExposed > 0) difficulty -= 2; // If opponent left themself wide open after a failed strong attack, they'll be easier to hit.
               
                if (othertarget.IsEvading > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += othertarget.IsEvading;
                    damage -= othertarget.IsEvading;
                    
                }
                if (attacker.IsAggressive > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    difficulty -= attacker.IsAggressive;
                    damage += attacker.IsAggressive;
                    attacker.IsAggressive = 0;
                }

                if (othertarget.IsGuarding > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += othertarget.IsGuarding;
                    damage -= othertarget.IsGuarding;
                }

                if (attacker.IsGuarding > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsGuarding = 0;
                }

                if (attacker.IsEvading > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsEvading = 0;
                }

                var critCheck = true;
                if (attacker.Stamina < requiredStam)
                {   //Not enough stamina-- reduced effect
                    critCheck = false;
                    damage *= attacker.Stamina / requiredStam;
                    difficulty += (int)Math.Ceiling((double)((requiredStam - attacker.Stamina) / requiredStam) * (20 - difficulty)); // Too tired? You're likely to miss.
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " did not have enough stamina, and took penalties to the attack.");
                }

                attacker.HitStamina(requiredStam); //Now that stamina has been checked, reduce the attacker's stamina by the appopriate amount. (We'll hit the attacker up for the rest on a miss or a dodge).

                //If opponent fumbled on their previous action they should become stunned.
                // We put it down here for Grab so it doesn't interfere with the stun from a crit on moving into range.
                if (othertarget.Fumbled)
                {
                    othertarget.IsDazed = true;
                    othertarget.Fumbled = false;
                }

                var attackTable = attacker.BuildActionTable(difficulty, othertarget.Dexterity, attacker.Dexterity, othertarget.Stamina, othertarget.StaminaCap);
                //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
                TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));

                if (roll <= attackTable.miss)
                {   //Miss-- no effect.
                    TeamBattlefield.WindowController.Hit.Add(" FAILED! ");
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " failed to establish a hold!");
                    return false; //Failed attack, if we ever need to check that.
                }

                if (roll >= attackTable.crit && critCheck)
                { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                    TeamBattlefield.WindowController.Hit.Add(" CRITICAL HIT! ");
                    TeamBattlefield.WindowController.Hint.Add("Critical! " + attacker.Name + " found a particularly good hold!");
                    damage += 10;
                }

                //grab can only be used when you are not grappling the target, so we no longer need the old check.
                TeamBattlefield.WindowController.Hit.Add(attacker.Name + " GRABBED " + othertarget.Name + "! ");
                TeamBattlefield.WindowController.Hint.Add(othertarget.Name + " is being grappled! " + attacker.Name + " has reduced difficulty to use melee attacks and can also use the special attacks Throw and Submission.");
                TeamBattlefield.WindowController.Hint.Add(othertarget.Name + " can try to escape the grapple by using Move, Throw, or Teleport.");
                othertarget.IsGrappledBy.Add(attacker.Name);

                //If we managed to grab without being in grab range, we are certainly in grabe range afterwards.


                damage = Math.Max(damage, 1);
                othertarget.HitHp(damage);
                attacker.IsRestraining = true;
                attacker.IsGrabbable = 40;
                othertarget.IsGrabbable = 40;
                if (TeamBattlefield.GetTargetOfOther() == partner)
                {
                    if (othertarget.SetTarget == 0)
                        othertarget.SetTarget += 1;

                    else
                        othertarget.SetTarget -= 1;
                }
                return true; //Successful attack, if we ever need to check that.
            }
        
        }

        public bool ActionSubmission(int roll)
        {
            var attacker = this;
            var target = TeamBattlefield.GetTarget();
            var damage = 30;
            var requiredStam = 25;

            var difficulty = 10; //Base difficulty, rolls greater than this amount will hit.
            difficulty += (int)Math.Floor((double)(target.Strength - attacker.Strength) / 2); //Up the difficulty of submission moves based on the relative strength of the combatants.
            //difficulty *= (int)Math.Ceiling((double)2 * target.HP / target.MaxHP);//Multiply difficulty with percentage of opponent's health and 2, so that 50% health yields normal difficulty.
            

            if (target.HP * 100 / target.MaxHP > 50) // If target is above 50% HP this is a bad move.
            {
                damage /= 2;
                difficulty *= 2;
            }

            if (target.IsExposed > 0) difficulty -= 2; // If opponent left themself wide open after a failed strong attack, they'll be easier to hit.
          
            if (target.IsEvading > 0)
            {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                difficulty += target.IsEvading;
                damage -= target.IsEvading;
            }

            if (attacker.IsEvading > 0)
            {//Apply attack bonus from move/teleport then reset it.
                attacker.IsEvading = 0;
            }

            if (attacker.IsAggressive > 0)
            {//Apply attack bonus from move/teleport then reset it.
                difficulty -= attacker.IsAggressive;
                damage += attacker.IsAggressive;
                attacker.IsAggressive = 0;
            }

            var critCheck = true;
            if (attacker.Stamina < requiredStam)
            {   //Not enough stamina-- reduced effect
                critCheck = false;
                damage *= attacker.Stamina / requiredStam;
                difficulty += (int)Math.Ceiling((double)((requiredStam - attacker.Stamina) / requiredStam) * (20 - difficulty)); // Too tired? You're likely to miss.
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " did not have enough stamina, and took penalties to the attack.");
            }

            attacker.HitStamina(requiredStam); //Now that stamina has been checked, reduce the attacker's stamina by the appopriate amount. (We'll hit the attacker up for the rest on a miss or a dodge).

            //If opponent fumbled on their previous action they should become stunned.
            // We put it down here for Grab so it doesn't interfere with the stun from a crit on moving into range.
            if (target.Fumbled)
            {
                target.IsDazed = true;
                target.Fumbled = false;
            }

            var attackTable = attacker.BuildActionTable(difficulty, target.Dexterity, attacker.Dexterity, target.Stamina, target.StaminaCap);
            //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
            TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));

            if (roll <= attackTable.miss)
            {   //Miss-- no effect.
                TeamBattlefield.WindowController.Hit.Add(" FAILED! ");
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " failed to establish a hold!");
                return false; //Failed attack, if we ever need to check that.
            }

            if (roll >= attackTable.crit && critCheck)
            { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                TeamBattlefield.WindowController.Hit.Add(" CRITICAL HIT! ");
                TeamBattlefield.WindowController.Hint.Add("Critical! " + attacker.Name + " found a particularly good hold!");
                damage += 10;
            }

            TeamBattlefield.WindowController.Hit.Add(" SUBMISSION ");
            target.IsEscaping -= 5; //Submission moves make it harder to escape.
            if (target.IsGrappling(attacker))
            {
                attacker.RemoveGrappler(target);
                TeamBattlefield.WindowController.Hint.Add(target.Name + " is in a SUBMISSION hold. " + attacker.Name + " is also no longer at a penalty from being grappled!");
            }
            else
            {
                TeamBattlefield.WindowController.Hint.Add(target.Name + " is in a SUBMISSION hold.");
            }

            //If we managed to make a submission without being in grab range, we are certainly in grabe range afterwards.
            if (!TeamBattlefield.InGrabRange) TeamBattlefield.InGrabRange = true;

            damage = Math.Max(damage, 1);
            target.HitHp(damage);
            return true; //Successful attack, if we ever need to check that.
        }

        public bool ActionTackle(int roll)
        {
            var attacker = this;
            var target = TeamBattlefield.GetTarget();
            var partner = TeamBattlefield.GetPartner();
            var othertarget = TeamBattlefield.GetOther();
            var damage = Utils.RollDice(new List<int>() { 5, 5 }) - 1 + attacker.Strength;
            damage /= 2;
            var requiredStam = 10;
            var difficulty = 8; //Base difficulty, rolls greater than this amount will hit.

            if (target.IsDead == false)

            {
                if (target.IsExposed > 0) difficulty -= 2; // If opponent left themself wide open after a failed strong attack, they'll be easier to hit.
                if (target.IsEvading > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += target.IsEvading;
                    damage -= target.IsEvading;
                    
                }
                if (attacker.IsAggressive > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    difficulty -= attacker.IsAggressive;
                    damage += attacker.IsAggressive;
                    attacker.IsAggressive = 0;
                }
                if (target.IsGuarding > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += target.IsGuarding;
                    damage -= target.IsGuarding;
                }
                
                if (attacker.IsEvading > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsEvading = 0;
                }

                if (attacker.IsGuarding > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsGuarding = 0;
                }

                var critCheck = true;
                if (attacker.Stamina < requiredStam)
                {   //Not enough stamina-- reduced effect
                    critCheck = false;
                    damage *= attacker.Stamina / requiredStam;
                    difficulty += (int)Math.Ceiling((double)((requiredStam - attacker.Stamina) / requiredStam) * (20 - difficulty)); // Too tired? You're likely to miss.
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " did not have enough stamina, and took penalties to the attack.");
                }

                attacker.HitStamina(requiredStam); //Now that stamina has been checked, reduce the attacker's stamina by the appopriate amount. (We'll hit the attacker up for the rest on a miss or a dodge).

                var attackTable = attacker.BuildActionTable(difficulty, target.Dexterity, attacker.Dexterity, target.Stamina, target.StaminaCap);
                //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
                TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));

                if (roll <= attackTable.miss)
                {   //Miss-- no effect.
                    TeamBattlefield.WindowController.Hit.Add(" FAILED!");
                    attacker.IsExposed += 4; //If the fighter misses a big attack, it leaves them open and they have to recover balance which gives the opponent a chance to strike.
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " was left wide open by the failed attack and is now Exposed! Any attacks against " + attacker.Name + " have -2 difficulty to hit, and they can be Grabbed even if fighters are not in grappling range!");                    //If opponent fumbled on their previous action they should become stunned. Tackle is a special case because it stuns anyway if it hits, so we only do this on a miss.
                    if (target.Fumbled)
                    {
                        target.IsDazed = true;
                        target.Fumbled = false;
                    }
                    return false; //Failed attack, if we ever need to check that.
                }

                if (roll >= attackTable.crit && critCheck == true)
                { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                    TeamBattlefield.WindowController.Hint.Add("Critical Hit! " + attacker.Name + " really drove that one home!");
                    damage += 10;
                }

                TeamBattlefield.WindowController.Hit.Add(attacker.Name + " TACKLED " + target.Name + ". " + attacker.Name + " can take another action while their opponent is stunned!");

                //Deal all the actual damage/effects here.


                if (attacker.IsGrabbable == 0 && target.IsGrabbable == 0)
                {
                    attacker.IsGrabbable += 1;
                    target.IsGrabbable += 1;
                }
                if (attacker.IsGrabbable == 0 && target.IsGrabbable == 1)
                {
                    attacker.IsGrabbable += 1;
                }
                if (attacker.IsGrabbable == 1 && target.IsGrabbable == 0)
                {
                    attacker.IsGrabbable += 1;
                    target.IsGrabbable += 2;
                }
                if (attacker.IsGrabbable == 2 && target.IsGrabbable == 1)
                {
                    attacker.IsGrabbable -= 1;
                }
                if (attacker.IsGrabbable == 1 && target.IsGrabbable == 2)
                {
                    attacker.IsGrabbable += 1;
                }
                if (attacker.IsGrabbable == 2 && target.IsGrabbable == 0)
                {
                    attacker.IsGrabbable -= 1;
                    target.IsGrabbable += 1;
                }
                if (attacker.IsGrabbable == 0 && target.IsGrabbable == 2)
                {
                    attacker.IsGrabbable += 2;
                }


                damage = Math.Max(damage, 0);
                if (damage > 0) target.HitHp(damage); //This is to prevent the game displayin that the attacker did 0 damage, which is the normal case.
                target.IsStunned = 2;
                partner.IsDazed = true;
                othertarget.IsDazed = true;
                return true; //Successful attack, if we ever need to check that.

            }

            else
            {
                if (othertarget.IsExposed > 0) difficulty -= 2; // If opponent left themself wide open after a failed strong attack, they'll be easier to hit.
          
                if (othertarget.IsEvading > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += othertarget.IsEvading;
                    damage -= othertarget.IsEvading;

                }
                if (attacker.IsAggressive > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    difficulty -= attacker.IsAggressive;
                    damage += attacker.IsAggressive;
                    attacker.IsAggressive = 0;
                }
                if (othertarget.IsGuarding > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += othertarget.IsGuarding;
                    damage -= othertarget.IsGuarding;
                }

                if (attacker.IsEvading > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsEvading = 0;
                }

                if (attacker.IsGuarding > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsGuarding = 0;
                }

                var critCheck = true;
                if (attacker.Stamina < requiredStam)
                {   //Not enough stamina-- reduced effect
                    critCheck = false;
                    damage *= attacker.Stamina / requiredStam;
                    difficulty += (int)Math.Ceiling((double)((requiredStam - attacker.Stamina) / requiredStam) * (20 - difficulty)); // Too tired? You're likely to miss.
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " did not have enough stamina, and took penalties to the attack.");
                }

                attacker.HitStamina(requiredStam); //Now that stamina has been checked, reduce the attacker's stamina by the appopriate amount. (We'll hit the attacker up for the rest on a miss or a dodge).

                var attackTable = attacker.BuildActionTable(difficulty, othertarget.Dexterity, attacker.Dexterity, othertarget.Stamina, othertarget.StaminaCap);
                //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
                TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));

                if (roll <= attackTable.miss)
                {   //Miss-- no effect.
                    TeamBattlefield.WindowController.Hit.Add(" FAILED!");
                    attacker.IsExposed += 4; //If the fighter misses a big attack, it leaves them open and they have to recover balance which gives the opponent a chance to strike.
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " was left wide open by the failed attack and is now Exposed! Any attacks against " + attacker.Name + " have -2 difficulty to hit, and they can be Grabbed even if fighters are not in grappling range!");                    //If opponent fumbled on their previous action they should become stunned. Tackle is a special case because it stuns anyway if it hits, so we only do this on a miss.
                    if (othertarget.Fumbled)
                    {
                        othertarget.IsDazed = true;
                        othertarget.Fumbled = false;
                    }
                    return false; //Failed attack, if we ever need to check that.
                }

                if (roll >= attackTable.crit && critCheck == true)
                { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                    TeamBattlefield.WindowController.Hint.Add("Critical Hit! " + attacker.Name + " really drove that one home!");
                    damage += 10;
                }

                TeamBattlefield.WindowController.Hit.Add(attacker.Name + " TACKLED " + othertarget.Name + ". " + attacker.Name + " can take another action while their opponent is stunned!");

                //Deal all the actual damage/effects here.


                if (attacker.IsGrabbable == 0 && othertarget.IsGrabbable == 0)
                {
                    attacker.IsGrabbable += 1;
                    othertarget.IsGrabbable += 1;
                }
                if (attacker.IsGrabbable == 0 && othertarget.IsGrabbable == 1)
                {
                    attacker.IsGrabbable += 1;
                }
                if (attacker.IsGrabbable == 1 && othertarget.IsGrabbable == 0)
                {
                    attacker.IsGrabbable += 1;
                    othertarget.IsGrabbable += 2;
                }
                if (attacker.IsGrabbable == 2 && othertarget.IsGrabbable == 1)
                {
                    attacker.IsGrabbable -= 1;
                }
                if (attacker.IsGrabbable == 1 && othertarget.IsGrabbable == 2)
                {
                    attacker.IsGrabbable += 1;
                }
                if (attacker.IsGrabbable == 2 && othertarget.IsGrabbable == 0)
                {
                    attacker.IsGrabbable -= 1;
                    othertarget.IsGrabbable += 1;
                }
                if (attacker.IsGrabbable == 0 && othertarget.IsGrabbable == 2)
                {
                    attacker.IsGrabbable += 2;
                }


                damage = Math.Max(damage, 0);
                if (damage > 0) othertarget.HitHp(damage); //This is to prevent the game displayin that the attacker did 0 damage, which is the normal case.
                othertarget.IsStunned = 2;
                partner.IsDazed = true;
                return true; //Successful attack, if we ever need to check that.
                }
            }


        public bool ActionThrow(int roll)
        {
            var attacker = this;
            var target = TeamBattlefield.GetTarget();
            var damage = Utils.RollDice(new List<int>() { 5, 5 }) - 1 + attacker.Strength;
            damage *= 2;
            var requiredStam = 10;
            var difficulty = 8; //Base difficulty, rolls greater than this amount will hit.

            if (attacker.IsRestrained) difficulty += Math.Max(0, 12 + (int)Math.Floor((double)(target.Strength - attacker.Strength) / 2)); //When grappled, up the difficulty based on the relative strength of the combatants. Minimum of +4 difficulty, maximum of +12.
            if (attacker.IsRestrained) difficulty -= attacker.IsEscaping; //Then reduce difficulty based on how much effort we've put into escaping so far.
            if (target.IsRestrained) difficulty -= 4; //Lower the difficulty considerably if the target is restrained.
            if (target.IsExposed > 0) difficulty -= 2; // If opponent left themself wide open after a failed strong attack, they'll be easier to hit.
          
            if (target.IsEvading > 0)
            {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                difficulty += target.IsEvading;
                damage -= target.IsEvading;
                
            }

            if (target.IsGuarding > 0)
            {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                difficulty += target.IsGuarding;
                damage -= target.IsGuarding;
            }

            if (attacker.IsAggressive > 0)
            {//Apply attack bonus from move/teleport then reset it.
                difficulty -= attacker.IsAggressive;
                damage += attacker.IsAggressive;
                attacker.IsAggressive = 0;
            }
            
            if (attacker.IsEvading > 0)
            {//Apply attack bonus from move/teleport then reset it.
                attacker.IsEvading = 0;
            }

            var critCheck = true;
            if (attacker.Stamina < requiredStam)
            {   //Not enough stamina-- reduced effect
                critCheck = false;
                damage *= attacker.Stamina / requiredStam;
                difficulty += (int)Math.Ceiling((double)((requiredStam - attacker.Stamina) / requiredStam) * (20 - difficulty)); // Too tired? You're likely to miss.
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " did not have enough stamina, and took penalties to the attack.");
            }

            attacker.HitStamina(requiredStam); //Now that stamina has been checked, reduce the attacker's stamina by the appopriate amount. (We'll hit the attacker up for the rest on a miss or a dodge).

            var attackTable = attacker.BuildActionTable(difficulty, target.Dexterity, attacker.Dexterity, target.Stamina, target.StaminaCap);
            //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
            TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));

            if (roll <= attackTable.miss)
            {   //Miss-- no effect.
                TeamBattlefield.WindowController.Hit.Add(" FAILED!");
                if (attacker.IsRestrained) attacker.IsEscaping += 4;//If we fail to escape, it'll be easier next time.
                attacker.IsExposed += 2; //If the fighter misses a big attack, it leaves them open and they have to recover balance which gives the opponent a chance to strike.
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " was left wide open by the failed attack and is now Exposed! Any attacks against " + attacker.Name + " have -2 difficulty to hit, and they can be Grabbed even if fighters are not in grappling range!");                //If opponent fumbled on their previous action they should become stunned. Tackle is a special case because it stuns anyway if it hits, so we only do this on a miss.
                if (target.Fumbled)
                {
                    target.IsDazed = true;
                    target.Fumbled = false;
                }
                return false; //Failed attack, if we ever need to check that.
            }

            if (roll >= attackTable.crit && critCheck == true)
            { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                TeamBattlefield.WindowController.Hint.Add("Critical Hit! " + attacker.Name + " really drove that one home!");
                damage += 10;
            }

            if (attacker.IsGrappling(target))
            {

                attacker.IsRestraining = false;
                target.RemoveGrappler(attacker);
                target.IsRestrained = false;
                TeamBattlefield.InGrabRange = false;//A throw will put the fighters out of grappling range.
                if (target.IsGrappling(attacker))
                {
                    attacker.RemoveGrappler(target);
                    target.IsRestraining = false;
                    attacker.IsRestrained = false;
                    TeamBattlefield.WindowController.Hit.Add(attacker.Name + " gained the upper hand and THREW " + target.Name + "! " + attacker.Name + " can make another move! " + attacker.Name + " is no longer at a penalty from being grappled!");
                }
                else
                {
                    target.IsRestrained = false;
                    damage += Math.Max(0, 10 - target.IsEscaping);
                    TeamBattlefield.WindowController.Hit.Add(attacker.Name + " THREW " + target.Name + " and dealt bonus damage!");
                }
                //Battlefield.WindowController.Hint.Add(target.Name + ", you are no longer grappled. You should make your post, but you should only emote being hit, do not try to perform any other actions.");
            }
            else if (target.IsGrappling(attacker))
            {
                target.IsRestraining = false; 
                attacker.RemoveGrappler(target);
                attacker.IsRestrained = false;
                TeamBattlefield.InGrabRange = false;//A throw will put the fighters out of grappling range.
                TeamBattlefield.WindowController.Hit.Add(attacker.Name + " found a hold and THREW " + target.Name + " off! " + attacker.Name + " is no longer at a penalty from being grappled!");
                //Battlefield.WindowController.Hint.Add(target.Name + ", you should make your post, but you should only emote being hit, do not try to perform any other actions.");
            }
            else
            {
                TeamBattlefield.InGrabRange = true;//A regular tackle will put you close enough to your opponent to initiate a grab.
                TeamBattlefield.WindowController.Hit.Add(attacker.Name + " TACKLED " + target.Name + ". " + attacker.Name + " can take another action while their opponent is stunned!");
                //Battlefield.WindowController.Hint.Add(target.Name + ", you should make your post, but you should only emote being hit, do not try to perform any other actions.");
            }

            //Deal all the actual damage/effects here.

            damage = Math.Max(damage, 1);
            target.HitHp(damage);
            attacker.IsGrabbable = 0;
            target.IsGrabbable = 0;
            return true; //Successful attack, if we ever need to check that.
        }


        public bool ActionRanged(int roll)
        {
            var attacker = this;
            var target = TeamBattlefield.GetTarget();
            var partner = TeamBattlefield.GetPartner();
            var othertarget = TeamBattlefield.GetOther();
            var damage = Utils.RollDice(new List<int>() { 5, 5 }) - 1;
            damage *= 2;
            damage += (attacker.Strength + attacker.Dexterity);
            damage += Math.Min(attacker.Strength, attacker.Spellpower);
            var requiredStam = 10;
            var difficulty = 10; //Base difficulty, rolls greater than this amount will hit.


            if (target.IsDead == false)

            {
                //If opponent fumbled on their previous action they should become stunned.
                if (target.Fumbled)
                {
                    target.IsDazed = true;
                    target.Fumbled = false;
                }

                if (attacker.IsRestrained) difficulty += 4; //Up the difficulty considerably if the attacker is restrained.
                if (target.IsRestrained) difficulty += 4; //Ranged attacks during grapple are hard.
                if (target.IsRestrained) difficulty -= 2; //Lower the difficulty slightly if the target is restrained.
                if (attacker.IsFocused > 0) difficulty -= (int)Math.Ceiling((double)attacker.IsFocused / 10); //Lower the difficulty considerably if the attacker is focused
          
                if (attacker.IsFocused > 0) damage += (int)Math.Ceiling((double)attacker.IsFocused / 10); //Focus gives bonus damage.

                if (target.IsEvading > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += (int)Math.Ceiling((double)target.IsEvading / 2);//Half effect on ranged attacks.
                    damage -= (int)Math.Ceiling((double)target.IsEvading / 2);
                    
                }
                if (attacker.IsAggressive > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    difficulty -= attacker.IsAggressive;
                    damage += attacker.IsAggressive;
                    attacker.IsAggressive = 0;
                }
                if (target.IsGuarding > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += (int)Math.Ceiling((double)target.IsGuarding / 2);
                    damage -= (int)Math.Ceiling((double)target.IsGuarding / 2); ;
                }

                if (attacker.IsEvading > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsEvading = 0;
                }

                if (attacker.IsGuarding > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsGuarding = 0;
                }

                var critCheck = true;
                if (attacker.Stamina < requiredStam)
                {   //Not enough stamina-- reduced effect
                    critCheck = false;
                    damage *= attacker.Stamina / requiredStam;
                    difficulty += (int)Math.Ceiling((double)((requiredStam - attacker.Stamina) / requiredStam) * (20 - difficulty)); // Too tired? You're likely to miss.
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " did not have enough stamina, and took penalties to the attack.");
                }

                attacker.HitStamina(requiredStam); //Now that stamina has been checked, reduce the attacker's stamina by the appopriate amount.

                var attackTable = attacker.BuildActionTable(difficulty, target.Dexterity, attacker.Dexterity, target.Stamina, target.StaminaCap);
                //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
                TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));

                if (roll <= attackTable.miss)
                {   //Miss-- no effect.
                    TeamBattlefield.WindowController.Hit.Add(" FAILED!");
                    return false; //Failed attack, if we ever need to check that.
                }

                if (roll >= attackTable.crit && critCheck == true)
                { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                    TeamBattlefield.WindowController.Hit.Add(" CRITICAL HIT! ");
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " hit somewhere that really hurts!");
                    damage += 10;
                }
                else
                { //Normal hit.
                    TeamBattlefield.WindowController.Hit.Add(" HIT! ");
                }

                //Deal all the actual damage/effects here.

                if (TeamBattlefield.InGrabRange)
                {// Succesful attacks will beat back the grabber before they can grab you, but not if you're already grappling.
                    if (!attacker.IsRestrained && !target.IsRestrained)
                    {
                        TeamBattlefield.InGrabRange = false;
                        TeamBattlefield.WindowController.Hit.Add(attacker.Name + " distracted " + target.Name + " with the attack and was able to move out of grappling range!");
                    }
                }

                //If you're being grappled and you hit the opponent that will make it a little easier to escape later on.
                if (attacker.IsRestrained) attacker.IsEscaping += (int)Math.Floor((double)damage / 5);

                damage = Math.Max(damage, 1);
                target.HitHp(damage);
                attacker.IsGrabbable = 0;
                target.IsGrabbable = 0;
                return true; //Successful attack, if we ever need to check that.
            }

            else

            {
                if (othertarget.Fumbled)
                {
                    othertarget.IsDazed = true;
                    othertarget.Fumbled = false;
                }

                if (attacker.IsRestrained) difficulty += 4; //Up the difficulty considerably if the attacker is restrained.
                if (othertarget.IsRestrained) difficulty += 4; //Ranged attacks during grapple are hard.
                if (othertarget.IsRestrained) difficulty -= 2; //Lower the difficulty slightly if the target is restrained.
                if (attacker.IsFocused > 0) difficulty -= (int)Math.Ceiling((double)attacker.IsFocused / 10); //Lower the difficulty considerably if the attacker is focused
          
                if (attacker.IsFocused > 0) damage += (int)Math.Ceiling((double)attacker.IsFocused / 10); //Focus gives bonus damage.

                if (othertarget.IsEvading > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += (int)Math.Ceiling((double)othertarget.IsEvading / 2);//Half effect on ranged attacks.
                    damage -= (int)Math.Ceiling((double)othertarget.IsEvading / 2);
                    
                }
                if (attacker.IsAggressive > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    difficulty -= attacker.IsAggressive;
                    damage += attacker.IsAggressive;
                    attacker.IsAggressive = 0;
                }
                if (othertarget.IsGuarding > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += othertarget.IsGuarding;
                    damage -= othertarget.IsGuarding;
                }

                if (attacker.IsEvading > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsEvading = 0;
                }

                if (attacker.IsGuarding > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsGuarding = 0;
                }

                var critCheck = true;
                if (attacker.Stamina < requiredStam)
                {   //Not enough stamina-- reduced effect
                    critCheck = false;
                    damage *= attacker.Stamina / requiredStam;
                    difficulty += (int)Math.Ceiling((double)((requiredStam - attacker.Stamina) / requiredStam) * (20 - difficulty)); // Too tired? You're likely to miss.
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " did not have enough stamina, and took penalties to the attack.");
                }

                attacker.HitStamina(requiredStam); //Now that stamina has been checked, reduce the attacker's stamina by the appopriate amount.

                var attackTable = attacker.BuildActionTable(difficulty, othertarget.Dexterity, attacker.Dexterity, othertarget.Stamina, othertarget.StaminaCap);
                //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
                TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));

                if (roll <= attackTable.miss)
                {   //Miss-- no effect.
                    TeamBattlefield.WindowController.Hit.Add(" FAILED!");
                    return false; //Failed attack, if we ever need to check that.
                }

                if (roll >= attackTable.crit && critCheck == true)
                { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                    TeamBattlefield.WindowController.Hit.Add(" CRITICAL HIT! ");
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " hit somewhere that really hurts!");
                    damage += 10;
                }
                else
                { //Normal hit.
                    TeamBattlefield.WindowController.Hit.Add(" HIT! ");
                }

                //Deal all the actual damage/effects here.

                if (TeamBattlefield.InGrabRange)
                {// Succesful attacks will beat back the grabber before they can grab you, but not if you're already grappling.
                    if (!attacker.IsRestrained && !othertarget.IsRestrained)
                    {
                        TeamBattlefield.InGrabRange = false;
                        TeamBattlefield.WindowController.Hit.Add(attacker.Name + " distracted " + othertarget.Name + " with the attack and was able to move out of grappling range!");
                    }
                }

                //If you're being grappled and you hit the opponent that will make it a little easier to escape later on.
                if (attacker.IsRestrained) attacker.IsEscaping += (int)Math.Floor((double)damage / 5);

                damage = Math.Max(damage, 1);
                othertarget.HitHp(damage);
                attacker.IsGrabbable = 0;
                othertarget.IsGrabbable = 0;
                return true; //Successful attack, if we ever need to check that.
            }
        }

        public bool ActionMagic(int roll)
        {
            var attacker = this;
            var target = TeamBattlefield.GetTarget();
            var partner = TeamBattlefield.GetPartner();
            var othertarget = TeamBattlefield.GetOther();
            var damage = Utils.RollDice(new List<int>() { 5, 5 }) - 1 + attacker.Spellpower;
            damage *= 2;
            damage += Math.Min(attacker.Strength, attacker.Spellpower);
            var requiredMana = 10;
            var difficulty = 8; //Base difficulty, rolls greater than this amount will hit.


            if (target.IsDead == false)

            {
                //If opponent fumbled on their previous action they should become stunned.
                if (target.Fumbled)
                {
                    target.IsDazed = true;
                    target.Fumbled = false;
                }

                if (attacker.IsRestrained) difficulty += 2; //Math.max(2, 4 + Math.floor((target.strength() - attacker.strength()) / 2)); //When grappled, up the difficulty based on the relative strength of the combatants. Minimum of +2 difficulty, maximum of +8.
                if (target.IsRestrained) difficulty -= 4; //Lower the difficulty considerably if the target is restrained.
                if (target.IsExposed > 0) difficulty -= 2; // If opponent left themself wide open after a failed strong attack, they'll be easier to hit.
          
                if (target.IsEvading > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += target.IsEvading;
                    damage -= target.IsEvading;
                }
                if (attacker.IsAggressive > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    difficulty -= attacker.IsAggressive;
                    damage += attacker.IsAggressive;
                    attacker.IsAggressive = 0;
                }
                if (target.IsGuarding > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += target.IsGuarding;
                    damage -= target.IsGuarding;
                }

                if (attacker.IsGuarding > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsGuarding = 0;
                }

                if (attacker.IsEvading > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsEvading = 0;
                }

                var critCheck = true;
                if (attacker.Mana < requiredMana)
                {   //Not enough mana-- reduced effect
                    critCheck = false;
                    damage *= attacker.Mana / requiredMana;
                    difficulty += (int)Math.Ceiling((double)((requiredMana - attacker.Mana) / requiredMana) * (20 - difficulty)); // Too tired? You're likely to have your spell fizzle.
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " did not have enough mana, and took penalties to the attack.");
                }

                attacker.HitMana(requiredMana); //Now that required mana has been checked, reduce the attacker's mana by the appopriate amount.

                var attackTable = attacker.BuildActionTable(difficulty, target.Dexterity, attacker.Dexterity, target.Mana, target.ManaCap);
                //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
                TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));

                if (roll <= attackTable.miss)
                {   //Miss-- no effect.
                    TeamBattlefield.WindowController.Hit.Add(" FAILED!");
                    attacker.IsExposed += 4; //If the fighter misses a big attack, it leaves them open and they have to recover balance which gives the opponent a chance to strike.
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " was left wide open by the failed attack and is now Exposed! Any attacks against " + attacker.Name + " have -2 difficulty to hit, and they can be Grabbed even if fighters are not in grappling range!"); return false; //Failed attack, if we ever need to check that.
                }

                if (roll >= attackTable.crit)
                { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                    TeamBattlefield.WindowController.Hit.Add(" CRITICAL HIT! ");
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " landed a particularly vicious blow!");
                    damage += 10;
                    TeamBattlefield.WindowController.Hint.Add("Critical Hit! " + attacker.Name + "'s magic worked abnormally well! " + target.Name + " is dazed and disoriented.");
                }
                else
                { //Normal hit.
                    TeamBattlefield.WindowController.Hit.Add("MAGIC HIT! ");
                }

                //Deal all the actual damage/effects here.

                if (TeamBattlefield.InGrabRange)
                {// Succesful attacks will beat back the grabber before they can grab you, but not if you're already grappling.
                    if (!attacker.IsRestrained && !target.IsRestrained)
                    {
                        TeamBattlefield.InGrabRange = false;
                        TeamBattlefield.WindowController.Hit.Add(attacker.Name + " distracted " + target.Name + " with the attack and was able to move out of grappling range!");
                    }
                }

                //If you're being grappled and you hit the opponent that will make it a little easier to escape later on.
                if (attacker.IsRestrained) attacker.IsEscaping += (int)Math.Floor((double)damage / 5);

                damage = Math.Max(damage, 1);
                target.HitHp(damage);
                attacker.IsGrabbable = 0;
                target.IsGrabbable = 0;
                return true; //Successful attack, if we ever need to check that.
            }

            else
            {
                //If opponent fumbled on their previous action they should become stunned.
                if (othertarget.Fumbled)
                {
                    othertarget.IsDazed = true;
                    othertarget.Fumbled = false;
                }

                if (attacker.IsRestrained) difficulty += 2; //Math.max(2, 4 + Math.floor((othertarget.strength() - attacker.strength()) / 2)); //When grappled, up the difficulty based on the relative strength of the combatants. Minimum of +2 difficulty, maximum of +8.
                if (othertarget.IsRestrained) difficulty -= 4; //Lower the difficulty considerably if the target is restrained.
                if (othertarget.IsExposed > 0) difficulty -= 2; // If opponent left themself wide open after a failed strong attack, they'll be easier to hit.
          
                if (othertarget.IsEvading > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += othertarget.IsEvading;
                    damage -= othertarget.IsEvading;
                    
                }
                if (attacker.IsAggressive > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    difficulty -= attacker.IsAggressive;
                    damage += attacker.IsAggressive;
                    attacker.IsAggressive = 0;
                }
                if (othertarget.IsGuarding > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += othertarget.IsGuarding;
                    damage -= othertarget.IsGuarding;
                }

                if (attacker.IsGuarding > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsGuarding = 0;
                }

                if (attacker.IsEvading > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsEvading = 0;
                }

                var critCheck = true;
                if (attacker.Mana < requiredMana)
                {   //Not enough mana-- reduced effect
                    critCheck = false;
                    damage *= attacker.Mana / requiredMana;
                    difficulty += (int)Math.Ceiling((double)((requiredMana - attacker.Mana) / requiredMana) * (20 - difficulty)); // Too tired? You're likely to have your spell fizzle.
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " did not have enough mana, and took penalties to the attack.");
                }

                attacker.HitMana(requiredMana); //Now that required mana has been checked, reduce the attacker's mana by the appopriate amount.

                var attackTable = attacker.BuildActionTable(difficulty, othertarget.Dexterity, attacker.Dexterity, othertarget.Mana, othertarget.ManaCap);
                //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
                TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));

                if (roll <= attackTable.miss)
                {   //Miss-- no effect.
                    TeamBattlefield.WindowController.Hit.Add(" FAILED!");
                    attacker.IsExposed += 4; //If the fighter misses a big attack, it leaves them open and they have to recover balance which gives the opponent a chance to strike.
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " was left wide open by the failed attack and is now Exposed! Any attacks against " + attacker.Name + " have -2 difficulty to hit, and they can be Grabbed even if fighters are not in grappling range!"); return false; //Failed attack, if we ever need to check that.
                }

                if (roll >= attackTable.crit)
                { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                    TeamBattlefield.WindowController.Hit.Add(" CRITICAL HIT! ");
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " landed a particularly vicious blow!");
                    damage += 10;
                    TeamBattlefield.WindowController.Hint.Add("Critical Hit! " + attacker.Name + "'s magic worked abnormally well! " + othertarget.Name + " is dazed and disoriented.");
                }
                else
                { //Normal hit.
                    TeamBattlefield.WindowController.Hit.Add("MAGIC HIT! ");
                }

                //Deal all the actual damage/effects here.

                if (TeamBattlefield.InGrabRange)
                {// Succesful attacks will beat back the grabber before they can grab you, but not if you're already grappling.
                    if (!attacker.IsRestrained && !othertarget.IsRestrained)
                    {
                        TeamBattlefield.InGrabRange = false;
                        TeamBattlefield.WindowController.Hit.Add(attacker.Name + " distracted " + othertarget.Name + " with the attack and was able to move out of grappling range!");
                    }
                }

                //If you're being grappled and you hit the opponent that will make it a little easier to escape later on.
                if (attacker.IsRestrained) attacker.IsEscaping += (int)Math.Floor((double)damage / 5);

                damage = Math.Max(damage, 1);
                othertarget.HitHp(damage);
                attacker.IsGrabbable = 0;
                othertarget.IsGrabbable = 0;
                return true; //Successful attack, if we ever need to check that.

            }
        }

        
        public bool ActionHex(int roll)
        {
            var attacker = this;
            var target = TeamBattlefield.GetTarget();
            var partner = TeamBattlefield.GetPartner();
            var othertarget = TeamBattlefield.GetOther();
            var damage = Utils.RollDice(new List<int>() { 5, 5 }) - 1 + attacker.Spellpower;
            damage += Math.Min(attacker.Strength, attacker.Spellpower);
            var requiredMana = 5;
            var difficulty = 6; //Base difficulty, rolls greater than this amount will hit.


            if (target.IsDead == false)
            {     //If opponent fumbled on their previous action they should become stunned.
                if (target.Fumbled)
            {
                target.IsDazed = true;
                target.Fumbled = false;
            }

            if (attacker.IsRestrained) difficulty += 2;
            if (target.IsRestrained) difficulty -= 4; //Ranged attacks during grapple are hard, but Hex is now melee.
            if (target.IsExposed > 0) difficulty -= 2; // If opponent left themself wide open after a failed strong attack, they'll be easier to hit.
          

            if (target.IsEvading > 0)
            {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                difficulty += target.IsEvading;//Half effect on ranged attacks.
                damage -= target.IsEvading;

            }
            if (attacker.IsAggressive > 0)
            {//Apply attack bonus from move/teleport then reset it.
                difficulty -= attacker.IsAggressive;
                damage += attacker.IsAggressive;
                attacker.IsAggressive = 0;
            }
                if (target.IsGuarding > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += target.IsGuarding;
                    damage -= target.IsGuarding;
                }

                if (attacker.IsGuarding > 0)
                {//Apply attack bonus from move/teleport then reset it.
                attacker.IsGuarding = 0;
                }
                
                if (attacker.IsEvading > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsEvading = 0;
                }

                var critCheck = true;
            if (attacker.Mana  < requiredMana)
            {   //Not enough mana-- reduced effect
                critCheck = false;
                damage *= attacker.Mana  / requiredMana;
                difficulty += (int)Math.Ceiling((double)((requiredMana - attacker.Mana ) / requiredMana) * (20 - difficulty)); // Too tired? You're likely to have your spell fizzle.
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " did not have enough mana, and took penalties to the attack.");
            }

            attacker.HitMana(requiredMana); //Now that required mana has been checked, reduce the attacker's mana by the appopriate amount.

            var attackTable = attacker.BuildActionTable(difficulty, target.Dexterity, attacker.Dexterity, target.Mana, target.ManaCap);
            //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
            TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));

            if (roll <= attackTable.miss)
            {   //Miss-- no effect.
                TeamBattlefield.WindowController.Hit.Add(" FAILED! ");
                return false; //Failed attack, if we ever need to check that.
            }

            if (roll >= attackTable.crit)
            { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                TeamBattlefield.WindowController.Hit.Add(" CRITICAL HIT! ");
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " landed a particularly vicious blow!");
                damage += 10;
                TeamBattlefield.WindowController.Hint.Add("Critical Hit! " + attacker.Name + "'s magic worked abnormally well! " + target.Name + " is dazed and disoriented.");
            }
            else
            { //Normal hit.
                TeamBattlefield.WindowController.Hit.Add("MAGIC HIT! ");
            }

            //Deal all the actual damage/effects here.

            if (TeamBattlefield.InGrabRange)
            {// Succesful attacks will beat back the grabber before they can grab you, but not if you're already grappling.
                if (!attacker.IsRestrained && !target.IsRestrained)
                {
                    TeamBattlefield.InGrabRange = false;
                    TeamBattlefield.WindowController.Hit.Add(attacker.Name + " distracted " + target.Name + " with the attack and was able to move out of grappling range!");
                }
            }

            //If you're being grappled and you hit the opponent that will make it a little easier to escape later on.
            if (attacker.IsRestrained) attacker.IsEscaping += (int)Math.Floor((double)damage / 5);

            damage = Math.Max(damage, 1);
            target.HitHp(damage);
            target.HitMana(damage);
            attacker.IsGrabbable = 0;
            target.IsGrabbable = 0;
                return true; //Successful attack, if we ever need to check that.
            }

            else
            {
                //If opponent fumbled on their previous action they should become stunned.
                if (othertarget.Fumbled)
                {
                    othertarget.IsDazed = true;
                    othertarget.Fumbled = false;
                }

                if (attacker.IsRestrained) difficulty += 2;
                if (othertarget.IsRestrained) difficulty -= 4; //Ranged attacks during grapple are hard, but Hex is now melee.
                if (othertarget.IsExposed > 0) difficulty -= 2; // If opponent left themself wide open after a failed strong attack, they'll be easier to hit.
          
                if (othertarget.IsEvading > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += othertarget.IsEvading;//Half effect on ranged attacks.
                    damage -= othertarget.IsEvading;

                }
                if (attacker.IsAggressive > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    difficulty -= attacker.IsAggressive;
                    damage += attacker.IsAggressive;
                    attacker.IsAggressive = 0;
                }
                if (othertarget.IsGuarding > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += othertarget.IsGuarding;
                    damage -= othertarget.IsGuarding;
                }

                if (attacker.IsGuarding > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsGuarding = 0;
                }
                if (attacker.IsEvading > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsEvading = 0;
                }
                var critCheck = true;
                if (attacker.Mana < requiredMana)
                {   //Not enough mana-- reduced effect
                    critCheck = false;
                    damage *= attacker.Mana / requiredMana;
                    difficulty += (int)Math.Ceiling((double)((requiredMana - attacker.Mana) / requiredMana) * (20 - difficulty)); // Too tired? You're likely to have your spell fizzle.
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " did not have enough mana, and took penalties to the attack.");
                }

                attacker.HitMana(requiredMana); //Now that required mana has been checked, reduce the attacker's mana by the appopriate amount.

                var attackTable = attacker.BuildActionTable(difficulty, othertarget.Dexterity, attacker.Dexterity, othertarget.Mana, othertarget.ManaCap);
                //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
                TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));

                if (roll <= attackTable.miss)
                {   //Miss-- no effect.
                    TeamBattlefield.WindowController.Hit.Add(" FAILED! ");
                    return false; //Failed attack, if we ever need to check that.
                }

                if (roll >= attackTable.crit)
                { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                    TeamBattlefield.WindowController.Hit.Add(" CRITICAL HIT! ");
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " landed a particularly vicious blow!");
                    damage += 10;
                    TeamBattlefield.WindowController.Hint.Add("Critical Hit! " + attacker.Name + "'s magic worked abnormally well! " + othertarget.Name + " is dazed and disoriented.");
                }
                else
                { //Normal hit.
                    TeamBattlefield.WindowController.Hit.Add("MAGIC HIT! ");
                }

                //Deal all the actual damage/effects here.

                if (TeamBattlefield.InGrabRange)
                {// Succesful attacks will beat back the grabber before they can grab you, but not if you're already grappling.
                    if (!attacker.IsRestrained && !othertarget.IsRestrained)
                    {
                        TeamBattlefield.InGrabRange = false;
                        TeamBattlefield.WindowController.Hit.Add(attacker.Name + " distracted " + othertarget.Name + " with the attack and was able to move out of grappling range!");
                    }
                }

                //If you're being grappled and you hit the opponent that will make it a little easier to escape later on.
                if (attacker.IsRestrained) attacker.IsEscaping += (int)Math.Floor((double)damage / 5);

                damage = Math.Max(damage, 1);
                othertarget.HitHp(damage);
                othertarget.HitMana(damage); 
                attacker.IsGrabbable = 0;
                othertarget.IsGrabbable = 0;
                return true; //Successful attack, if we ever need to check that.

            }
        }

        public bool ActionCurse(int roll)
        {
            var attacker = this;
            var target = TeamBattlefield.GetTarget();
            var partner = TeamBattlefield.GetPartner();
            var othertarget = TeamBattlefield.GetOther();
            var damage = 11 + attacker.Spellpower - attacker.Strength;
            damage *= 2;
            var requiredMana = 20;
            var difficulty = 10; //Base difficulty, rolls greater than this amount will hit.

            if (target.IsDead == false)
            {
                //If opponent fumbled on their previous action they should become stunned.
                if (target.Fumbled)
                {
                    target.IsDazed = true;
                    target.Fumbled = false;
                }

                if (attacker.IsRestrained) difficulty += 2; //Math.Max(2, 4 + (int)Math.Floor((double)(target.Strength - attacker.Strength) / 2)); //When grappled, up the difficulty based on the relative strength of the combatants. Minimum of +2 difficulty, maximum of +8.
                if (target.IsRestrained) difficulty -= 4; //Ranged attacks during grapple are hard.
                if (attacker.IsFocused > 0) difficulty -= (int)Math.Ceiling((double)attacker.IsFocused / 10); //Lower the difficulty if the attacker is focused
          

                if (attacker.IsFocused > 0) damage += (int)Math.Ceiling((double)attacker.IsFocused / 10); //Focus gives bonus damage.

                if (target.IsEvading > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += (int)Math.Ceiling((double)target.IsEvading / 2);//Half effect on ranged attacks.
                    damage -= (int)Math.Ceiling((double)target.IsEvading / 2);
               
                }
                if (attacker.IsAggressive > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    difficulty -= attacker.IsAggressive;
                    damage += attacker.IsAggressive;
                    attacker.IsAggressive = 0;
                }

                if (target.IsGuarding > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += (int)Math.Ceiling((double)target.IsGuarding / 2);
                    damage -= (int)Math.Ceiling((double)target.IsGuarding / 2);
                }

                if (attacker.IsEvading > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsEvading = 0;
                }

                var critCheck = true;
                if (attacker.Mana < requiredMana)
                {   //Not enough mana-- reduced effect
                    critCheck = false;
                    damage *= attacker.Mana / requiredMana;
                    difficulty += (int)Math.Ceiling((double)((requiredMana - attacker.Mana) / requiredMana) * (20 - difficulty)); // Too tired? You're likely to have your spell fizzle.
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " did not have enough mana, and took penalties to the attack.");
                }

                attacker.HitMana(requiredMana); //Now that required mana has been checked, reduce the attacker's mana by the appopriate amount.

                var attackTable = attacker.BuildActionTable(difficulty, target.Dexterity, attacker.Dexterity, target.Mana, target.ManaCap);
                //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
                TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));

                if (roll <= attackTable.miss)
                {   //Miss-- no effect. Happens during grappling.
                    TeamBattlefield.WindowController.Hit.Add(" CURSE FAILED! Curse may not be used again by the attacker!");
                    attacker.CurseUsed += 10;
                    return false; //Failed attack, if we ever need to check that.
                }

                if (roll >= attackTable.crit)
                { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                    TeamBattlefield.WindowController.Hit.Add(" CRITICAL HIT! ");
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " landed a particularly vicious blow!");
                    damage += 10;
                    TeamBattlefield.WindowController.Hint.Add("Critical Hit! " + attacker.Name + "'s magic worked abnormally well! " + target.Name + " is dazed and disoriented.");
                }
                else
                { //Normal hit.
                    TeamBattlefield.WindowController.Hit.Add("CURSE HIT! Curse may not be used again by the attacker!");
                }

                //Deal all the actual damage/effects here.

                if (TeamBattlefield.InGrabRange)
                {// Succesful attacks will beat back the grabber before they can grab you, but not if you're already grappling.
                    if (!attacker.IsRestrained && !target.IsRestrained)
                    {
                        TeamBattlefield.InGrabRange = false;
                        TeamBattlefield.WindowController.Hit.Add(attacker.Name + " distracted " + target.Name + " with the attack and was able to move out of grappling range!");
                    }
                }

                //If you're being grappled and you hit the opponent that will make it a little easier to escape later on.
                if (attacker.IsRestrained) attacker.IsEscaping += (int)Math.Floor((double)damage / 5);

                damage = Math.Max(damage, 1);
                target.HitHp(damage);
                attacker.CurseUsed += 10;
                attacker.IsGrabbable = 0;
                target.IsGrabbable = 0;
                return true; //Successful attack, if we ever need to check that.
            }

            else
            {
                //If opponent fumbled on their previous action they should become stunned.
                if (othertarget.Fumbled)
                {
                    othertarget.IsDazed = true;
                    othertarget.Fumbled = false;
                }

                if (attacker.IsRestrained) difficulty += 2; //Math.Max(2, 4 + (int)Math.Floor((double)(othertarget.Strength - attacker.Strength) / 2)); //When grappled, up the difficulty based on the relative strength of the combatants. Minimum of +2 difficulty, maximum of +8.
                if (othertarget.IsRestrained) difficulty -= 4; //Ranged attacks during grapple are hard.
                if (attacker.IsFocused > 0) difficulty -= (int)Math.Ceiling((double)attacker.IsFocused / 10); //Lower the difficulty if the attacker is focused
          
                if (attacker.IsFocused > 0) damage += (int)Math.Ceiling((double)attacker.IsFocused / 10); //Focus gives bonus damage.

                if (othertarget.IsEvading > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += (int)Math.Ceiling((double)othertarget.IsEvading / 2);//Half effect on ranged attacks.
                    damage -= (int)Math.Ceiling((double)othertarget.IsEvading / 2);
                }
                if (attacker.IsAggressive > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    difficulty -= attacker.IsAggressive;
                    damage += attacker.IsAggressive;
                    attacker.IsAggressive = 0;
                }

                if (othertarget.IsGuarding > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += (int)Math.Ceiling((double)othertarget.IsGuarding / 2);
                    damage -= (int)Math.Ceiling((double)othertarget.IsGuarding / 2);
                }
                if (attacker.IsGuarding > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsGuarding = 0;
                }

                if (attacker.IsEvading > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsEvading = 0;
                }

                var critCheck = true;
                if (attacker.Mana < requiredMana)
                {   //Not enough mana-- reduced effect
                    critCheck = false;
                    damage *= attacker.Mana / requiredMana;
                    difficulty += (int)Math.Ceiling((double)((requiredMana - attacker.Mana) / requiredMana) * (20 - difficulty)); // Too tired? You're likely to have your spell fizzle.
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " did not have enough mana, and took penalties to the attack.");
                }

                attacker.HitMana(requiredMana); //Now that required mana has been checked, reduce the attacker's mana by the appopriate amount.

                var attackTable = attacker.BuildActionTable(difficulty, othertarget.Dexterity, attacker.Dexterity, othertarget.Mana, othertarget.ManaCap);
                //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
                TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));

                if (roll <= attackTable.miss)
                {   //Miss-- no effect. Happens during grappling.
                    TeamBattlefield.WindowController.Hit.Add(" CURSE FAILED! Curse may not be used again by the attacker!");
                    attacker.CurseUsed += 10;
                    return false; //Failed attack, if we ever need to check that.
                }

                if (roll >= attackTable.crit)
                { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                    TeamBattlefield.WindowController.Hit.Add(" CRITICAL HIT! ");
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " landed a particularly vicious blow!");
                    damage += 10;
                    TeamBattlefield.WindowController.Hint.Add("Critical Hit! " + attacker.Name + "'s magic worked abnormally well! " + othertarget.Name + " is dazed and disoriented.");
                }
                else
                { //Normal hit.
                    TeamBattlefield.WindowController.Hit.Add("CURSE HIT! Curse may not be used again by the attacker!");
                }

                //Deal all the actual damage/effects here.

                if (TeamBattlefield.InGrabRange)
                {// Succesful attacks will beat back the grabber before they can grab you, but not if you're already grappling.
                    if (!attacker.IsRestrained && !othertarget.IsRestrained)
                    {
                        TeamBattlefield.InGrabRange = false;
                        TeamBattlefield.WindowController.Hit.Add(attacker.Name + " distracted " + othertarget.Name + " with the attack and was able to move out of grappling range!");
                    }
                }

                //If you're being grappled and you hit the opponent that will make it a little easier to escape later on.
                if (attacker.IsRestrained) attacker.IsEscaping += (int)Math.Floor((double)damage / 5);

                damage = Math.Max(damage, 1);
                othertarget.HitHp(damage);
                attacker.CurseUsed += 10;
                attacker.IsGrabbable = 0;
                othertarget.IsGrabbable = 0;
                return true; //Successful attack, if we ever need to check that.
            }
        }


        public bool ActionSpell(int roll)
        {
            var attacker = this;
            var target = TeamBattlefield.GetTarget();
            var partner = TeamBattlefield.GetPartner();
            var othertarget = TeamBattlefield.GetOther();
            var damage = Utils.RollDice(new List<int>() { 5, 5 }) - 1 + attacker.Spellpower;
            damage *= 2;
            damage += Math.Min(attacker.Strength, attacker.Spellpower);
            var requiredMana = 10;
            var difficulty = 10; //Base difficulty, rolls greater than this amount will hit.


            if (target.IsDead == false)
            {
                //If opponent fumbled on their previous action they should become stunned.
                if (target.Fumbled)
                {
                    target.IsDazed = true;
                    target.Fumbled = false;
                }

                if (attacker.IsRestrained) difficulty += 4; //Math.Max(2, 4 + (int)Math.Floor((double)(target.Strength - attacker.Strength) / 2)); //When grappled, up the difficulty based on the relative strength of the combatants. Minimum of +2 difficulty, maximum of +8.
                if (target.IsRestrained) difficulty += 4; //Ranged attacks during grapple are hard.
                if (attacker.IsFocused > 0) difficulty -= (int)Math.Ceiling((double)attacker.IsFocused / 10); //Lower the difficulty if the attacker is focused
          
                if (attacker.IsFocused > 0) damage += (int)Math.Ceiling((double)attacker.IsFocused / 10); //Focus gives bonus damage.

                if (target.IsEvading > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += (int)Math.Ceiling((double)target.IsEvading / 2);//Half effect on ranged attacks.
                    damage -= (int)Math.Ceiling((double)target.IsEvading / 2);
                }
                if (attacker.IsAggressive > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    difficulty -= attacker.IsAggressive;
                    damage += attacker.IsAggressive;
                    attacker.IsAggressive = 0;
                }

                if (target.IsGuarding > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += (int)Math.Ceiling((double)target.IsGuarding / 2);
                    damage -= (int)Math.Ceiling((double)target.IsGuarding / 2);
                }

                if (attacker.IsEvading > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsEvading = 0;
                }

                if (attacker.IsGuarding > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsGuarding = 0;
                }

                var critCheck = true;
                if (attacker.Mana < requiredMana)
                {   //Not enough mana-- reduced effect
                    critCheck = false;
                    damage *= attacker.Mana / requiredMana;
                    difficulty += (int)Math.Ceiling((double)((requiredMana - attacker.Mana) / requiredMana) * (20 - difficulty)); // Too tired? You're likely to have your spell fizzle.
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " did not have enough mana, and took penalties to the attack.");
                }

                attacker.HitMana(requiredMana); //Now that required mana has been checked, reduce the attacker's mana by the appopriate amount.

                var attackTable = attacker.BuildActionTable(difficulty, target.Dexterity, attacker.Dexterity, target.Mana, target.ManaCap);
                //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
                TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));

                if (roll <= attackTable.miss)
                {   //Miss-- no effect. Happens during grappling.
                    TeamBattlefield.WindowController.Hit.Add(" FAILED! ");
                    return false; //Failed attack, if we ever need to check that.
                }

                if (roll >= attackTable.crit)
                { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                    TeamBattlefield.WindowController.Hit.Add(" CRITICAL HIT! ");
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " landed a particularly vicious blow!");
                    damage += 10;
                    TeamBattlefield.WindowController.Hint.Add("Critical Hit! " + attacker.Name + "'s magic worked abnormally well! " + target.Name + " is dazed and disoriented.");
                }
                else
                { //Normal hit.
                    TeamBattlefield.WindowController.Hit.Add("MAGIC HIT! ");
                }

                //Deal all the actual damage/effects here.

                if (TeamBattlefield.InGrabRange)
                {// Succesful attacks will beat back the grabber before they can grab you, but not if you're already grappling.
                    if (!attacker.IsRestrained && !target.IsRestrained)
                    {
                        TeamBattlefield.InGrabRange = false;
                        TeamBattlefield.WindowController.Hit.Add(attacker.Name + " distracted " + target.Name + " with the attack and was able to move out of grappling range!");
                    }
                }

                //If you're being grappled and you hit the opponent that will make it a little easier to escape later on.
                if (attacker.IsRestrained) attacker.IsEscaping += (int)Math.Floor((double)damage / 5);

                damage = Math.Max(damage, 1);
                target.HitHp(damage);
                attacker.IsGrabbable = 0;
                target.IsGrabbable = 0;
                return true; //Successful attack, if we ever need to check that.
            }

            else
            {
                //If opponent fumbled on their previous action they should become stunned.
                if (othertarget.Fumbled)
                {
                    othertarget.IsDazed = true;
                    othertarget.Fumbled = false;
                }

                if (attacker.IsRestrained) difficulty += 4; //Math.Max(2, 4 + (int)Math.Floor((double)(othertarget.Strength - attacker.Strength) / 2)); //When grappled, up the difficulty based on the relative strength of the combatants. Minimum of +2 difficulty, maximum of +8.
                if (othertarget.IsRestrained) difficulty += 4; //Ranged attacks during grapple are hard.
                if (attacker.IsFocused > 0) difficulty -= (int)Math.Ceiling((double)attacker.IsFocused / 10); //Lower the difficulty if the attacker is focused
          
                if (attacker.IsFocused > 0) damage += (int)Math.Ceiling((double)attacker.IsFocused / 10); //Focus gives bonus damage.

                if (othertarget.IsEvading > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += (int)Math.Ceiling((double)othertarget.IsEvading / 2);//Half effect on ranged attacks.
                    damage -= (int)Math.Ceiling((double)othertarget.IsEvading / 2);
                }
                if (attacker.IsAggressive > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    difficulty -= attacker.IsAggressive;
                    damage += attacker.IsAggressive;
                    attacker.IsAggressive = 0;
                }

                if (othertarget.IsGuarding > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += (int)Math.Ceiling((double)othertarget.IsGuarding / 2);
                    damage -= (int)Math.Ceiling((double)othertarget.IsGuarding / 2);
                }
                if (attacker.IsGuarding > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsGuarding = 0;
                }
                
                if (attacker.IsEvading > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsEvading = 0;
                }

                var critCheck = true;
                if (attacker.Mana < requiredMana)
                {   //Not enough mana-- reduced effect
                    critCheck = false;
                    damage *= attacker.Mana / requiredMana;
                    difficulty += (int)Math.Ceiling((double)((requiredMana - attacker.Mana) / requiredMana) * (20 - difficulty)); // Too tired? You're likely to have your spell fizzle.
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " did not have enough mana, and took penalties to the attack.");
                }

                attacker.HitMana(requiredMana); //Now that required mana has been checked, reduce the attacker's mana by the appopriate amount.

                var attackTable = attacker.BuildActionTable(difficulty, othertarget.Dexterity, attacker.Dexterity, othertarget.Mana, othertarget.ManaCap);
                //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
                TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));

                if (roll <= attackTable.miss)
                {   //Miss-- no effect. Happens during grappling.
                    TeamBattlefield.WindowController.Hit.Add(" FAILED! ");
                    return false; //Failed attack, if we ever need to check that.
                }

                if (roll >= attackTable.crit)
                { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                    TeamBattlefield.WindowController.Hit.Add(" CRITICAL HIT! ");
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " landed a particularly vicious blow!");
                    damage += 10;
                    TeamBattlefield.WindowController.Hint.Add("Critical Hit! " + attacker.Name + "'s magic worked abnormally well! " + othertarget.Name + " is dazed and disoriented.");
                }
                else
                { //Normal hit.
                    TeamBattlefield.WindowController.Hit.Add("MAGIC HIT! ");
                }

                //Deal all the actual damage/effects here.

                if (TeamBattlefield.InGrabRange)
                {// Succesful attacks will beat back the grabber before they can grab you, but not if you're already grappling.
                    if (!attacker.IsRestrained && !othertarget.IsRestrained)
                    {
                        TeamBattlefield.InGrabRange = false;
                        TeamBattlefield.WindowController.Hit.Add(attacker.Name + " distracted " + othertarget.Name + " with the attack and was able to move out of grappling range!");
                    }
                }

                //If you're being grappled and you hit the opponent that will make it a little easier to escape later on.
                if (attacker.IsRestrained) attacker.IsEscaping += (int)Math.Floor((double)damage / 5);

                damage = Math.Max(damage, 1);
                othertarget.HitHp(damage);
                attacker.IsGrabbable = 0;
                othertarget.IsGrabbable = 0;
                return true; //Successful attack, if we ever need to check that.
            }
        }


        public bool ActionManastorm(int roll)
        {
            var attacker = this;
            var target = TeamBattlefield.GetTarget();
            var othertarget = TeamBattlefield.GetOther();
            var partner = TeamBattlefield.GetPartner();
            var damage = Utils.RollDice(new List<int>() { 5, 5 }) - 1 + attacker.Spellpower;
            damage *= 2;
            damage += Math.Min(attacker.Strength, attacker.Spellpower);
            var requiredMana = 25;
            var difficulty = 10; //Base difficulty, rolls greater than this amount will hit.

            if (target.IsDead == false)

            {//If opponent fumbled on their previous action they should become stunned.
                if (target.Fumbled)
                {
                    target.IsDazed = true;
                    target.Fumbled = false;
                }

                if (attacker.IsRestrained) difficulty += 4; //Math.Max(2, 4 + (int)Math.Floor((double)(target.Strength - attacker.Strength) / 2)); //When grappled, up the difficulty based on the relative strength of the combatants. Minimum of +2 difficulty, maximum of +8.
                if (target.IsRestrained) difficulty += 4; //Ranged attacks during grapple are hard.
                if (attacker.IsFocused > 0) difficulty -= (int)Math.Ceiling((double)attacker.IsFocused / 10); //Lower the difficulty if the attacker is focused
          
                if (attacker.IsFocused > 0) damage += (int)Math.Ceiling((double)attacker.IsFocused / 10); //Focus gives bonus damage.

                if (target.IsEvading > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += target.IsEvading;//Half effect on ranged attacks.
                    damage -= target.IsEvading;
                }

                if (attacker.IsAggressive > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    difficulty -= attacker.IsAggressive;
                    damage += attacker.IsAggressive;
                    attacker.IsAggressive = 0;
                }

                if (target.IsGuarding > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    damage -= target.IsGuarding;
                    difficulty += target.IsGuarding;
                }

                if (attacker.IsGuarding > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsGuarding = 0;
                }
                
                if (attacker.IsEvading > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsEvading = 0;
                }

                var critCheck = true;
                if (attacker.Mana < requiredMana)
                {   //Not enough mana-- reduced effect
                    critCheck = false;
                    damage *= attacker.Mana / requiredMana;
                    difficulty += (int)Math.Ceiling((double)((requiredMana - attacker.Mana) / requiredMana) * (20 - difficulty)); // Too tired? You're likely to have your spell fizzle.
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " did not have enough mana, and took penalties to the attack.");
                }

                attacker.HitMana(requiredMana); //Now that required mana has been checked, reduce the attacker's mana by the appopriate amount.

                var attackTable = attacker.BuildActionTable(difficulty, target.Dexterity, attacker.Dexterity, target.Mana, target.ManaCap);
                //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
                TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));

                if (roll <= attackTable.miss)
                {   //Miss-- no effect. Happens during grappling.
                    TeamBattlefield.WindowController.Hit.Add(" FAILED! ");
                    return false; //Failed attack, if we ever need to check that.
                }

                if (roll >= attackTable.crit)
                { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                    TeamBattlefield.WindowController.Hit.Add(" CRITICAL HIT! ");
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " landed a particularly vicious blow!");
                    damage += 10;
                    TeamBattlefield.WindowController.Hint.Add("Critical Hit! " + attacker.Name + "'s magic worked abnormally well! " + target.Name + " is dazed and disoriented.");
                }
                else
                { //Normal hit.
                    TeamBattlefield.WindowController.Hit.Add("MAGIC HIT! ");
                }

                //Deal all the actual damage/effects here.

                if (TeamBattlefield.InGrabRange)
                {// Succesful attacks will beat back the grabber before they can grab you, but not if you're already grappling.
                    if (!attacker.IsRestrained && !target.IsRestrained)
                    {
                        TeamBattlefield.InGrabRange = false;
                        TeamBattlefield.WindowController.Hit.Add(attacker.Name + " distracted " + target.Name + " with the attack and was able to move out of grappling range!");
                    }
                }

                //If you're being grappled and you hit the opponent that will make it a little easier to escape later on.
                if (attacker.IsRestrained) attacker.IsEscaping += (int)Math.Floor((double)damage / 5);

                damage = Math.Max(damage, 1);
                target.HitHp(damage);
                othertarget.HitHp(damage / 2);
                attacker.IsGrabbable = 0;
                target.IsGrabbable = 0;
                othertarget.IsGrabbable = 0;
                return true; //Successful attack, if we ever need to check that.
            }

            else
            { //If opponent fumbled on their previous action they should become stunned.
                if (othertarget.Fumbled)
                {
                    othertarget.IsDazed = true;
                    othertarget.Fumbled = false;
                }

                if (attacker.IsRestrained) difficulty += 4; //Math.Max(2, 4 + (int)Math.Floor((double)(othertarget.Strength - attacker.Strength) / 2)); //When grappled, up the difficulty based on the relative strength of the combatants. Minimum of +2 difficulty, maximum of +8.
                if (othertarget.IsRestrained) difficulty += 4; //Ranged attacks during grapple are hard.
                if (attacker.IsFocused > 0) difficulty -= (int)Math.Ceiling((double)attacker.IsFocused / 10); //Lower the difficulty if the attacker is focused
          
                if (attacker.IsFocused > 0) damage += (int)Math.Ceiling((double)attacker.IsFocused / 10); //Focus gives bonus damage.

                if (othertarget.IsEvading > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += othertarget.IsEvading;//Half effect on ranged attacks.
                    damage -= othertarget.IsEvading;
                }
                if (attacker.IsAggressive > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    difficulty -= attacker.IsAggressive;
                    damage += attacker.IsAggressive;
                    attacker.IsAggressive = 0;
                }

                if (othertarget.IsGuarding > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += othertarget.IsGuarding;
                    damage -= othertarget.IsGuarding;
                }

                if (attacker.IsGuarding > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsGuarding = 0;
                }

                if (attacker.IsEvading > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsEvading = 0;
                }

                var critCheck = true;
                if (attacker.Mana < requiredMana)
                {   //Not enough mana-- reduced effect
                    critCheck = false;
                    damage *= attacker.Mana / requiredMana;
                    difficulty += (int)Math.Ceiling((double)((requiredMana - attacker.Mana) / requiredMana) * (20 - difficulty)); // Too tired? You're likely to have your spell fizzle.
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " did not have enough mana, and took penalties to the attack.");
                }

                attacker.HitMana(requiredMana); //Now that required mana has been checked, reduce the attacker's mana by the appopriate amount.

                var attackTable = attacker.BuildActionTable(difficulty, othertarget.Dexterity, attacker.Dexterity, othertarget.Mana, othertarget.ManaCap);
                //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
                TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));

                if (roll <= attackTable.miss)
                {   //Miss-- no effect. Happens during grappling.
                    TeamBattlefield.WindowController.Hit.Add(" FAILED! ");
                    return false; //Failed attack, if we ever need to check that.
                }

                if (roll >= attackTable.crit)
                { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                    TeamBattlefield.WindowController.Hit.Add(" CRITICAL HIT! ");
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " landed a particularly vicious blow!");
                    damage += 10;
                    TeamBattlefield.WindowController.Hint.Add("Critical Hit! " + attacker.Name + "'s magic worked abnormally well! " + othertarget.Name + " is dazed and disoriented.");
                }
                else
                { //Normal hit.
                    TeamBattlefield.WindowController.Hit.Add("MAGIC HIT! ");
                }

                //Deal all the actual damage/effects here.

                if (TeamBattlefield.InGrabRange)
                {// Succesful attacks will beat back the grabber before they can grab you, but not if you're already grappling.
                    if (!attacker.IsRestrained && !othertarget.IsRestrained)
                    {
                        TeamBattlefield.InGrabRange = false;
                        TeamBattlefield.WindowController.Hit.Add(attacker.Name + " distracted " + othertarget.Name + " with the attack and was able to move out of grappling range!");
                    }
                }

                //If you're being grappled and you hit the opponent that will make it a little easier to escape later on.
                if (attacker.IsRestrained) attacker.IsEscaping += (int)Math.Floor((double)damage / 5);

                damage = Math.Max(damage, 1);
                othertarget.HitHp(damage);
                target.HitHp(damage / 2);
                attacker.IsGrabbable = 0;
                target.IsGrabbable = 0;
                othertarget.IsGrabbable = 0;
                return true; //Successful attack, if we ever need to check that.
            }
        }

        public bool ActionCleave(int roll)
        {
            var attacker = this;
            var target = TeamBattlefield.GetTarget();
            var othertarget = TeamBattlefield.GetOther();
            var partner = TeamBattlefield.GetPartner();
            var damage = Utils.RollDice(new List<int>() { 5, 5 }) - 1 + attacker.Strength;
            damage *= 2;
            damage += Math.Min(attacker.Strength, attacker.Spellpower);
            var requiredStamina = 25;
            var difficulty = 10; //Base difficulty, rolls greater than this amount will hit.

            //If opponent fumbled on their previous action they should become stunned.
            if (target.Fumbled)
            {
                target.IsDazed = true;
                target.Fumbled = false;
            }

            if (attacker.IsRestrained) difficulty += 4; //Math.Max(2, 4 + (int)Math.Floor((double)(target.Strength - attacker.Strength) / 2)); //When grappled, up the difficulty based on the relative strength of the combatants. Minimum of +2 difficulty, maximum of +8.
            if (target.IsRestrained) difficulty += 4; //Ranged attacks during grapple are hard.
            if (attacker.IsFocused > 0) difficulty -= (int)Math.Ceiling((double)attacker.IsFocused / 10); //Lower the difficulty if the attacker is focused
          
            
            if (target.IsEvading > 0)
            {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                difficulty += target.IsEvading / 2;//Half effect on ranged attacks.
                damage -= target.IsEvading;
            }
            if (attacker.IsAggressive > 0)
            {//Apply attack bonus from move/teleport then reset it.
                difficulty -= attacker.IsAggressive;
                damage += attacker.IsAggressive;
                attacker.IsAggressive = 0;
            }

            if (target.IsGuarding > 0)
            {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                damage -= target.IsGuarding;
                difficulty += target.IsGuarding;
            }

                if (attacker.IsGuarding > 0)
                {//Apply attack bonus from move/teleport then reset it.
                attacker.IsGuarding = 0;
                }

            if (attacker.IsEvading > 0)
            {//Apply attack bonus from move/teleport then reset it.
                attacker.IsEvading = 0;
            }

            var critCheck = true;
            if (attacker.Stamina < requiredStamina)
            {   //Not enough Stamina-- reduced effect
                critCheck = false;
                damage *= attacker.Stamina / requiredStamina;
                difficulty += (int)Math.Ceiling((double)((requiredStamina - attacker.Stamina) / requiredStamina) * (20 - difficulty)); // Too tired? You're likely to have your spell fizzle.
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " did not have enough Stamina, and took penalties to the attack.");
            }

            attacker.HitStamina(requiredStamina); //Now that required Stamina has been checked, reduce the attacker's Stamina by the appopriate amount.

            var attackTable = attacker.BuildActionTable(difficulty, target.Dexterity, attacker.Dexterity, target.Stamina, target.StaminaCap);
            //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
            TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));

            if (roll <= attackTable.miss)
            {   //Miss-- no effect. Happens during grappling.
                TeamBattlefield.WindowController.Hit.Add(" FAILED! ");
                return false; //Failed attack, if we ever need to check that.
            }

            if (roll >= attackTable.crit)
            { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                TeamBattlefield.WindowController.Hit.Add(" CRITICAL HIT! ");
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " landed a particularly vicious blow!");
                damage += 10;
                TeamBattlefield.WindowController.Hint.Add("Critical Hit! " + attacker.Name + "'s magic worked abnormally well! " + target.Name + " is dazed and disoriented.");
            }
            else
            { //Normal hit.
                TeamBattlefield.WindowController.Hit.Add("HIT! ");
            }

            //Deal all the actual damage/effects here.

            if (TeamBattlefield.InGrabRange)
            {// Succesful attacks will beat back the grabber before they can grab you, but not if you're already grappling.
                if (!attacker.IsRestrained && !target.IsRestrained)
                {
                    TeamBattlefield.InGrabRange = false;
                    TeamBattlefield.WindowController.Hit.Add(attacker.Name + " distracted " + target.Name + " with the attack and was able to move out of grappling range!");
                }
            }

            //If you're being grappled and you hit the opponent that will make it a little easier to escape later on.
            if (attacker.IsRestrained) attacker.IsEscaping += (int)Math.Floor((double)damage / 5);

            damage = Math.Max(damage, 1);
            target.HitHp(damage * 3 / 4);
            othertarget.HitHp(damage * 3 / 4);
            attacker.IsGrabbable = 0;
            target.IsGrabbable = 0;
            othertarget.IsGrabbable = 0;
            return true; //Successful attack, if we ever need to check that.
        }

        public bool ActionBurn(int roll)
        {
            var attacker = this;
            var target = TeamBattlefield.GetTarget();
            var partner = TeamBattlefield.GetPartner();
            var othertarget = TeamBattlefield.GetOther();
            var requiredMana = 15;
            var difficulty = 8; //Base difficulty, rolls greater than this amount will hit.


            

            if (target.IsDead == false)
            {


                //If opponent fumbled on their previous action they should become stunned.
                if (target.Fumbled)
            {
                target.IsDazed = true;
                target.Fumbled = false;
            }

                if (attacker.IsRestrained) difficulty += 2; //Up the difficulty if the attacker is restrained.
                if (target.IsRestrained) difficulty -= 4; //Lower it if the target is restrained.
                if (target.IsExposed > 0) difficulty -= 2; // If opponent left themself wide open after a failed strong attack, they'll be easier to hit.
          
                if (target.IsEvading > 0)
            {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
             //Not affected by opponent's evasion bonus.
                difficulty += target.IsEvading;
                target.IsEvading = 0;
            }

                if (attacker.IsEvading > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsEvading = 0;
                }

                if (target.IsGuarding > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += target.IsGuarding;
                }

                if (attacker.IsAggressive > 0)
            {//Apply attack bonus from move/teleport then reset it.
                difficulty -= attacker.IsAggressive;
                attacker.IsAggressive = 0;
            }

            if (attacker.Mana < requiredMana)
            {   //Not enough stamina-- reduced effect
                difficulty += (int)Math.Ceiling((double)((requiredMana - attacker.Mana) / requiredMana) * (20 - difficulty)); // Too tired? You're going to fail.
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " didn't have enough Mana and took a penalty to the attempt.");
            }

            attacker.HitMana(requiredMana); //Now that mana has been checked, reduce the attacker's mana by the appopriate amount.

            var attackTable = attacker.BuildActionTable(difficulty, target.Dexterity, attacker.Dexterity, target.Mana, target.ManaCap);
            //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
            TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));


            if (roll <= attackTable.miss)
            {   //Miss-- no effect.
                TeamBattlefield.WindowController.Hit.Add(" FAILED!");
                return false; //Failed attack, if we ever need to check that.
            }

            if (roll >= attackTable.crit)
            { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                TeamBattlefield.WindowController.Hit.Add(" CRITICAL SUCCESS! ");
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " can perform another action!");
                // The only way the target can be stunned is if we set it to stunned with the action we're processing right now.
                // That in turn is only possible if target had fumbled. So we restore the fumbled status, but keep the stun.
                // That way we properly get a third action.
                if (target.IsDazed) target.Fumbled = true;
                target.IsDazed = true;
                if (target.IsDisoriented > 0) target.IsDisoriented += 2;
                if (target.IsExposed > 0) target.IsExposed += 2;
            }

            //The total mobility bonus generated. This will be split bewteen attack and defense.
            var totalBonus = Utils.RollDice(new List<int>() { 5, 5 }) - 1 + attacker.Spellpower;

            {
                target.HPDOT = (int)Math.Ceiling((double)totalBonus / 2);
                target.HPBurn = (4);
                target.ManaDOT = (int)Math.Ceiling((double)totalBonus / 2);
                target.ManaDamage = 4;
                TeamBattlefield.WindowController.Hit.Add(attacker.Name + " landed a strike against " + target.Name + " that will do damage over time for 3 turns!");
            }

            if (TeamBattlefield.InGrabRange)
            {
                TeamBattlefield.WindowController.Hit.Add(attacker.Name + " moved away!");
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " managed to put some distance between them and " + target.Name + " and is now out of grabbing range.");
            }
                attacker.IsGrabbable = 0;
                target.IsGrabbable = 0;
                return true; //Successful attack, if we ever need to check that.
            }

            else
            {
                //If opponent fumbled on their previous action they should become stunned.
                if (othertarget.Fumbled)
                {
                    othertarget.IsDazed = true;
                    othertarget.Fumbled = false;
                }

                if (attacker.IsRestrained) difficulty += 2; //Up the difficulty if the attacker is restrained.
                if (othertarget.IsRestrained) difficulty -= 4; //Lower it if the target is restrained.
                if (othertarget.IsExposed > 0) difficulty -= 2; // If opponent left themself wide open after a failed strong attack, they'll be easier to hit.
          
                if (othertarget.IsEvading > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                 //Not affected by opponent's evasion bonus.
                    difficulty += othertarget.IsEvading;
                }
                if (othertarget.IsGuarding > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += othertarget.IsGuarding;
                }

                if (attacker.IsAggressive > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    difficulty -= attacker.IsAggressive;
                    attacker.IsAggressive = 0;
                }

                if (attacker.IsEvading > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsEvading = 0;
                }

                if (attacker.Mana < requiredMana)
                {   //Not enough stamina-- reduced effect
                    difficulty += (int)Math.Ceiling((double)((requiredMana - attacker.Mana) / requiredMana) * (20 - difficulty)); // Too tired? You're going to fail.
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " didn't have enough Mana and took a penalty to the attempt.");
                }

                attacker.HitMana(requiredMana); //Now that mana has been checked, reduce the attacker's mana by the appopriate amount.

                var attackTable = attacker.BuildActionTable(difficulty, othertarget.Dexterity, attacker.Dexterity, othertarget.Mana, othertarget.ManaCap);
                //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
                TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));


                if (roll <= attackTable.miss)
                {   //Miss-- no effect.
                    TeamBattlefield.WindowController.Hit.Add(" FAILED!");
                    return false; //Failed attack, if we ever need to check that.
                }

                if (roll >= attackTable.crit)
                { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                    TeamBattlefield.WindowController.Hit.Add(" CRITICAL SUCCESS! ");
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " can perform another action!");
                    // The only way the target can be stunned is if we set it to stunned with the action we're processing right now.
                    // That in turn is only possible if target had fumbled. So we restore the fumbled status, but keep the stun.
                    // That way we properly get a third action.
                    if (othertarget.IsDazed) othertarget.Fumbled = true;
                    othertarget.IsDazed = true;
                    if (othertarget.IsDisoriented > 0) othertarget.IsDisoriented += 2;
                    if (othertarget.IsExposed > 0) othertarget.IsExposed += 2;
                }

                //The total mobility bonus generated. This will be split bewteen attack and defense.
                var totalBonus = Utils.RollDice(new List<int>() { 5, 5 }) - 1 + attacker.Spellpower;

                {
                    othertarget.HPDOT = (int)Math.Ceiling((double)totalBonus / 2);
                    othertarget.HPBurn = (4);
                    othertarget.ManaDOT = (int)Math.Ceiling((double)totalBonus / 2);
                    othertarget.ManaDamage = 4;
                    TeamBattlefield.WindowController.Hit.Add(attacker.Name + " landed a critical strike against " + othertarget.Name + " and will do damage over time for 3 turns!");
                }

                if (TeamBattlefield.InGrabRange)
                {
                    TeamBattlefield.WindowController.Hit.Add(attacker.Name + " moved away!");
                    TeamBattlefield.InGrabRange = false;
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " managed to put some distance between them and " + othertarget.Name + " and is now out of grabbing range.");
                }
                attacker.IsGrabbable = 0;
                othertarget.IsGrabbable = 0;

                return true; //Successful attack, if we ever need to check that.
            }
        }
        public bool ActionStab(int roll)
        {
            var attacker = this;
            var target = TeamBattlefield.GetTarget();
            var partner = TeamBattlefield.GetPartner();
            var othertarget = TeamBattlefield.GetOther();
            var requiredStamina = 15;
            var difficulty = 8; //Base difficulty, rolls greater than this amount will hit.


            if (target.IsDead == false)
            { 
                //If opponent fumbled on their previous action they should become stunned.
                if (target.Fumbled)
            {
                target.IsDazed = true;
                target.Fumbled = false;
            }

                if (attacker.IsRestrained) difficulty += 2; //Up the difficulty if the attacker is restrained.
                if (target.IsRestrained) difficulty -= 4; //Lower it if the target is restrained.
                if (target.IsExposed > 0) difficulty -= 2; // If opponent left themself wide open after a failed strong attack, they'll be easier to hit.
          
                if (target.IsEvading > 0)
            {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
             //Not affected by opponent's evasion bonus.
                difficulty += target.IsEvading;
            }

                if (target.IsGuarding > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += target.IsGuarding;
                }

                if (attacker.IsAggressive > 0)
            {//Apply attack bonus from move/teleport then reset it.
                difficulty -= attacker.IsAggressive;
                attacker.IsAggressive = 0;
            }


                if (attacker.IsEvading > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsEvading = 0;
                }

                if (attacker.Stamina < requiredStamina)
            {   //Not enough stamina-- reduced effect
                difficulty += (int)Math.Ceiling((double)((requiredStamina - attacker.Stamina) / requiredStamina) * (20 - difficulty)); // Too tired? You're going to fail.
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " didn't have enough Stamina and took a penalty to the attempt.");
            }

            attacker.HitStamina(requiredStamina); //Now that mana has been checked, reduce the attacker's mana by the appopriate amount.

            var attackTable = attacker.BuildActionTable(difficulty, target.Dexterity, attacker.Dexterity, target.Stamina, target.StaminaCap);
            //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
            TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));


            if (roll <= attackTable.miss)
            {   //Miss-- no effect.
                TeamBattlefield.WindowController.Hit.Add(" FAILED!");
                return false; //Failed attack, if we ever need to check that.
            }

            if (roll >= attackTable.crit)
            { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                TeamBattlefield.WindowController.Hit.Add(" CRITICAL SUCCESS! ");
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " can perform another action!");
                // The only way the target can be stunned is if we set it to stunned with the action we're processing right now.
                // That in turn is only possible if target had fumbled. So we restore the fumbled status, but keep the stun.
                // That way we properly get a third action.
                if (target.IsDazed) target.Fumbled = true;
                target.IsDazed = true;
                if (target.IsDisoriented > 0) target.IsDisoriented += 2;
                if (target.IsExposed > 0) target.IsExposed += 2;
            }

            //The total mobility bonus generated. This will be split bewteen attack and defense.
            var totalBonus = Utils.RollDice(new List<int>() { 5, 5 }) - 1 + attacker.Strength;

            {
                target.HPDOT = (int)Math.Ceiling((double)totalBonus / 2);
                target.HPBurn = (4);
                target.StaminaDOT = (int)Math.Ceiling((double)totalBonus / 2);
                target.StaminaDamage = 4;
                TeamBattlefield.WindowController.Hit.Add(attacker.Name + " landed a strike against " + target.Name + " that will do damage over time for 3 turns!");
                }

                if (TeamBattlefield.InGrabRange)
            {
                TeamBattlefield.WindowController.Hit.Add(attacker.Name + " moved away!");
                TeamBattlefield.InGrabRange = false;
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " managed to put some distance between them and " + target.Name + " and is now out of grabbing range.");
            }
                attacker.IsGrabbable = 0;
                target.IsGrabbable = 0;
                return true; //Successful attack, if we ever need to check that.
            }
            else
            {
                //If opponent fumbled on their previous action they should become stunned.
                if (othertarget.Fumbled)
                {
                    othertarget.IsDazed = true;
                    othertarget.Fumbled = false;
                }

                if (attacker.IsRestrained) difficulty += 2; //Up the difficulty if the attacker is restrained.
                if (othertarget.IsRestrained) difficulty -= 4; //Lower it if the target is restrained.
                if (othertarget.IsExposed > 0) difficulty -= 2; // If opponent left themself wide open after a failed strong attack, they'll be easier to hit.
          

                if (othertarget.IsEvading > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                 //Not affected by opponent's evasion bonus.
                    difficulty += othertarget.IsEvading;
                }

                if (othertarget.IsGuarding > 0)
                {//Evasion bonus from move/teleport. Only applies to one attack, then is reset to 0.
                    difficulty += othertarget.IsGuarding;
                }

                if (attacker.IsAggressive > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    difficulty -= attacker.IsAggressive;
                    attacker.IsAggressive = 0;
                }

                if (attacker.IsEvading > 0)
                {//Apply attack bonus from move/teleport then reset it.
                    attacker.IsEvading = 0;
                }

                if (attacker.Stamina < requiredStamina)
                {   //Not enough stamina-- reduced effect
                    difficulty += (int)Math.Ceiling((double)((requiredStamina - attacker.Stamina) / requiredStamina) * (20 - difficulty)); // Too tired? You're going to fail.
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " didn't have enough Stamina and took a penalty to the attempt.");
                }

                attacker.HitStamina(requiredStamina); //Now that mana has been checked, reduce the attacker's mana by the appopriate amount.

                var attackTable = attacker.BuildActionTable(difficulty, othertarget.Dexterity, attacker.Dexterity, othertarget.Stamina, othertarget.StaminaCap);
                //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
                TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));


                if (roll <= attackTable.miss)
                {   //Miss-- no effect.
                    TeamBattlefield.WindowController.Hit.Add(" FAILED!");
                    return false; //Failed attack, if we ever need to check that.
                }

                if (roll >= attackTable.crit)
                { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                    TeamBattlefield.WindowController.Hit.Add(" CRITICAL SUCCESS! ");
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " can perform another action!");
                    // The only way the target can be stunned is if we set it to stunned with the action we're processing right now.
                    // That in turn is only possible if target had fumbled. So we restore the fumbled status, but keep the stun.
                    // That way we properly get a third action.
                    if (othertarget.IsDazed) othertarget.Fumbled = true;
                    othertarget.IsDazed = true;
                    if (othertarget.IsDisoriented > 0) othertarget.IsDisoriented += 2;
                    if (othertarget.IsExposed > 0) othertarget.IsExposed += 2;
                }

                //The total mobility bonus generated. This will be split bewteen attack and defense.
                var totalBonus = Utils.RollDice(new List<int>() { 5, 5 }) - 1 + attacker.Strength;

                {
                    othertarget.HPDOT = (int)Math.Ceiling((double)totalBonus / 2);
                    othertarget.HPBurn = (4);
                    othertarget.StaminaDOT = (int)Math.Ceiling((double)totalBonus / 2);
                    othertarget.StaminaDamage = 4;
                    TeamBattlefield.WindowController.Hit.Add(attacker.Name + " landed a critical strike against " + othertarget.Name + " and will do damage over time for 3 turns!");
                }

                if (TeamBattlefield.InGrabRange)
                {
                    TeamBattlefield.WindowController.Hit.Add(attacker.Name + " moved away!");
                    TeamBattlefield.InGrabRange = false;
                    TeamBattlefield.WindowController.Hint.Add(attacker.Name + " managed to put some distance between them and " + othertarget.Name + " and is now out of grabbing range.");
                }
                attacker.IsGrabbable = 0;
                othertarget.IsGrabbable = 0;

                return true; //Successful attack, if we ever need to check that.
                }
            }


        public bool ActionRest(int roll)
        {
            var attacker = this;
            var target = TeamBattlefield.GetTarget();
            var partner = TeamBattlefield.GetPartner();
            var othertarget = TeamBattlefield.GetOther();
            var difficulty = 1; //Base difficulty, rolls greater than this amount will succeed.

            //if (attacker.IsDisoriented) difficulty += 2; //Up the difficulty if you are dizzy.
            if (attacker.IsRestrained) difficulty += 9; //Up the difficulty considerably if you are restrained.

           
            if (attacker.IsAggressive > 0)
            {//Apply bonus to our action from move/teleport then reset it.
                difficulty -= attacker.IsAggressive;
                attacker.IsAggressive = 0;
            }
            if (attacker.IsGuarding > 0)
            {//Apply attack bonus from move/teleport then reset it.
                attacker.IsGuarding = 0;
            }

            if (attacker.IsEvading > 0)
            {//Apply attack bonus from move/teleport then reset it.
                attacker.IsEvading = 0;
            }

            if (roll <= difficulty)
            {   //Failed!
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " was too disoriented or distracted to get any benefit from resting.");
                return false; //Failed action, if we ever need to check that.
            }

            if (roll == 20)
            {
                TeamBattlefield.WindowController.Hit.Add("CRITICAL SUCCESS! ");
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " can perform another action!");
                target.IsDazed = true;
                partner.IsDazed = true;
                othertarget.IsDazed= true;
                if (target.IsDisoriented > 0) target.IsDisoriented += 2;
                if (target.IsExposed > 0) target.IsExposed += 2;
            }

            //If opponent fumbled on their previous action they should become stunned, unless they're already stunned by us rolling a 20.
            if (target.Fumbled & !target.IsDazed)
            {
                target.IsDazed = true;
                target.Fumbled = false;
            }

            TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + Math.Max(2, (difficulty + 1)));
            var staminaShift = 12 + (attacker.Willpower * 2);
            //staminaShift = Math.Min(staminaShift, attacker.mana);

            //attacker.StaminaCap = Math.Max(attacker.StaminaCap, attacker.Stamina + staminaShift);
            //attacker.HitMana(staminaShift);
            attacker.AddStamina(staminaShift);
            TeamBattlefield.WindowController.Hit.Add(attacker.Name + " REGENERATES STAMINA!"); //Removed Stamina cost.
            TeamBattlefield.WindowController.Hint.Add(attacker.Name + " recovered " + staminaShift + " stamina!");
            return true;
        }

        public bool ActionFocus(int roll)
        {
            var attacker = this;
            var target = TeamBattlefield.GetTarget();
            var partner = TeamBattlefield.GetPartner();
            var othertarget = TeamBattlefield.GetOther();
            var difficulty = 1; //Base difficulty, rolls greater than this amount will succeed.

            //if (attacker.IsDisoriented) difficulty += 2; //Up the difficulty if you are dizzy.
            if (attacker.IsRestrained) difficulty += 9; //Up the difficulty considerably if you are restrained.


            if (attacker.IsAggressive > 0)
            {//Apply bonus to our action from move/teleport then reset it.
                difficulty -= attacker.IsAggressive;
                attacker.IsAggressive = 0;
            }
            if (attacker.IsGuarding > 0)
            {//Apply attack bonus from move/teleport then reset it.
                attacker.IsGuarding = 0;
            }

            if (attacker.IsEvading > 0)
            {//Apply attack bonus from move/teleport then reset it.
                attacker.IsEvading = 0;
            }

            if (roll <= difficulty)
            {   //Failed!
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " was too disoriented or distracted to focus.");
                return false; //Failed action, if we ever need to check that.
            }

            if (roll == 20)
            {
                TeamBattlefield.WindowController.Hit.Add("CRITICAL SUCCESS! ");
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " can perform another action!");
                target.IsDazed = true;
                partner.IsDazed = true;
                othertarget.IsDazed= true;
                if (target.IsDisoriented > 0) target.IsDisoriented += 2;
                if (target.IsExposed > 0) target.IsExposed += 2;
            }

            //If opponent fumbled on their previous action they should become stunned, unless they're already stunned by us rolling a 20.
            if (target.Fumbled & !target.IsDazed)
            {
                target.IsDazed = true;
                target.Fumbled = false;
            }

            TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + Math.Max(2, (difficulty + 1)));
            TeamBattlefield.WindowController.Hit.Add(attacker.Name + " FOCUSES!");
            attacker.IsFocused += Utils.RollDice(new List<int>() { 6, 6, 6, 6 }) +10 + attacker.Willpower * 4;
            return true;
        }

        public bool ActionChannel(int roll)
        {
            var attacker = this;
            var target = TeamBattlefield.GetTarget();
            var partner = TeamBattlefield.GetPartner();
            var othertarget = TeamBattlefield.GetOther();
            var difficulty = 1; //Base difficulty, rolls greater than this amount will succeed.

            //if (attacker.IsDisoriented) difficulty += 2; //Up the difficulty if you are dizzy.
            if (attacker.IsRestrained) difficulty += 9; //Up the difficulty considerably if you are restrained.

         
            if (attacker.IsAggressive > 0)
            {//Apply bonus to our action from move/teleport then reset it.
                difficulty -= attacker.IsAggressive;
                attacker.IsAggressive = 0;
            }
            if (attacker.IsGuarding > 0)
            {//Apply attack bonus from move/teleport then reset it.
                attacker.IsGuarding = 0;
            }

            if (attacker.IsEvading > 0)
            {//Apply attack bonus from move/teleport then reset it.
                attacker.IsEvading = 0;
            }

            if (roll <= difficulty)
            {   //Failed!
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " was too disoriented or distracted to channel mana.");
                return false; //Failed action, if we ever need to check that.
            }

            if (roll == 20)
            {
                TeamBattlefield.WindowController.Hit.Add("CRITICAL SUCCESS! ");
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " can perform another action!");
                target.IsDazed = true;
                partner.IsDazed = true;
                othertarget.IsDazed = true;
                if (target.IsDisoriented > 0) target.IsDisoriented += 2;
                if (target.IsExposed > 0) target.IsExposed += 2;
            }

            //If opponent fumbled on their previous action they should become stunned, unless they're already stunned by us rolling a 20.
            if (target.Fumbled & !target.IsDazed)
            {
                target.IsDazed = true;
                target.Fumbled = false;
            }

            TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + Math.Max(2, (difficulty + 1)));
            var manaShift = 12 + (attacker.Willpower * 2);
            //manaShift = Math.Min(manaShift, attacker.Stamina); //This also needs to be commented awaay if we want to remove stamina cost.

            //attacker._manaCap = Math.Max(attacker._manaCap, attacker.mana + manaShift);
            //attacker.HitStamina(manaShift);
            attacker.AddMana(manaShift);
            TeamBattlefield.WindowController.Hit.Add(attacker.Name + " GENERATES MANA!"); //Removed Stamina cost.
            TeamBattlefield.WindowController.Hint.Add(attacker.Name + " recovered " + manaShift + " mana!");
            return true;
        }

        public void ActionSkip(int roll)
        {
            TeamBattlefield.WindowController.Hit.Add(Name + " skipped the turn! ");
        }

        public void ActionFumble(int roll)
        {
            var attacker = this;
            var target = TeamBattlefield.GetTarget();

            if (attacker.IsEvading > 0)
            {//Apply attack bonus from move/teleport then reset it.
                attacker.IsEvading = 0;
            }
            if (attacker.IsAggressive > 0)
            {//Only applies to 1 action, so we reset it now.
                attacker.IsAggressive = 0;
            }
            if (attacker.IsGuarding > 0)
            {//Apply attack bonus from move/teleport then reset it.
                attacker.IsGuarding = 0;
            }

            attacker.IsExposed += 2;//Fumbling exposes you.

            TeamBattlefield.WindowController.Hit.Add(" FUMBLE! ");

            // Fumbles make you lose a turn, unless your opponent fumbled on their previous one in which case nobody should lose a turn and we just clear the fumbled status on them.
            // Reminder: if fumbled is true for you, your opponent's next normal action will stun you.
            if (!target.Fumbled)
            {
                attacker.Fumbled = true;
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " loses the next action and is Exposed!");
            }
            else
            {
                target.Fumbled = false;
                TeamBattlefield.WindowController.Hint.Add("Both fighter fumbled and lost an action so it evens out, but you should still emote the fumble.");
            }
        }

        public bool ActionMove(int roll)
        {
            var attacker = this;
            var target = TeamBattlefield.GetTarget();
            var partner = TeamBattlefield.GetPartner();
            var othertarget = TeamBattlefield.GetOther();
            var requiredStam = 5;
            var difficulty = 1; //Base difficulty, rolls greater than this amount will hit.

            //If opponent fumbled on their previous action they should become stunned.
            if (target.Fumbled)
            {
                target.IsDazed = true;
                target.Fumbled = false;
            }


            if (attacker.IsRestrained) difficulty += (11 + (int)Math.Floor((double)(target.Strength - attacker.Strength) / 2)); //When grappled, up the difficulty based on the relative strength of the combatants.
            if (attacker.IsRestrained) difficulty -= attacker.IsEscaping; //Then reduce difficulty based on how much effort we've put into escaping so far.
            if (target.IsRestrained) difficulty -= 4; //Lower the difficulty considerably if the target is restrained.

            
            if (attacker.IsAggressive > 0)
            {//Apply attack bonus from move/teleport then reset it.
                difficulty -= attacker.IsAggressive;
                attacker.IsAggressive = 0;
            }
            if (attacker.IsGuarding > 0)
            {//Apply attack bonus from move/teleport then reset it.
                attacker.IsGuarding = 0;
            }

            if (attacker.IsEvading > 0)
            {//Apply attack bonus from move/teleport then reset it.
                attacker.IsEvading = 0;
            }

            if (attacker.Stamina < requiredStam)
            {   //Not enough stamina-- reduced effect
                difficulty += (int)Math.Ceiling((double)((requiredStam - attacker.Stamina) / requiredStam) * (20 - difficulty)); // Too tired? You're going to fail.
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " didn't have enough Stamina and took a penalty to the escape attempt.");
            }

            attacker.HitStamina(requiredStam); //Now that stamina has been checked, reduce the attacker's stamina by the appopriate amount.

            var attackTable = attacker.BuildActionTable(difficulty, target.Dexterity, attacker.Dexterity, target.Stamina, target.StaminaCap);
            //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
            TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));

            var tempGrappleFlag = true;
            if (attacker.IsGrappling(target))
            { //If you're grappling someone they are freed, regardless of the outcome.
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " used ESCAPE. " + target.Name + " is no longer being grappled. ");
                attacker.IsRestraining = false; 
                target.RemoveGrappler(attacker);
                tempGrappleFlag = false;
            }

            if (roll <= attackTable.miss)
            {   //Miss-- no effect.
                TeamBattlefield.WindowController.Hit.Add(" FAILED!");
                if (attacker.IsRestrained) attacker.IsEscaping += 4;//If we fail to escape, it'll be easier next time.
                return false; //Failed attack, if we ever need to check that.
            }

            if (roll >= attackTable.crit)
            { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                TeamBattlefield.WindowController.Hit.Add(" CRITICAL SUCCESS! ");
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " can perform another action!");
                // The only way the target can be stunned is if we set it to stunned with the action we're processing right now.
                // That in turn is only possible if target had fumbled. So we restore the fumbled status, but keep the stun.
                // That way we properly get a third action.
                if (target.IsDazed) target.Fumbled = true;
                target.IsDazed = true;
                partner.IsDazed = true;
                othertarget.IsDazed = true;
                if (target.IsDisoriented > 0) target.IsDisoriented += 2;
                if (target.IsExposed > 0) target.IsExposed += 2;
            }

            //The total mobility bonus generated. This will be split bewteen attack and defense.
            var totalBonus = Utils.RollDice(new List<int>() { 5, 5 }) - 1 + attacker.Dexterity;

            if (target.IsGrappling(attacker))
            { //If you were being grappled, you get free.
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " escaped " + target.Name + "'s hold! ");
                target.IsRestraining = false; 
                attacker.RemoveGrappler(target);
                tempGrappleFlag = false;
                attacker.IsEvading = (int)Math.Floor((double)totalBonus / 2);
            }
            else
            {
                attacker.IsAggressive = (int)Math.Ceiling((double)totalBonus / 2);
                TeamBattlefield.WindowController.Hit.Add(attacker.Name + " gained mobility bonuses against " + target.Name + " for one turn!");
            }

            if (TeamBattlefield.InGrabRange)
            {
                TeamBattlefield.WindowController.Hit.Add(attacker.Name + " moved away!");
                TeamBattlefield.InGrabRange = false;
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " managed to put some distance between them and " + target.Name + " and is now out of grabbing range.");
            }
            attacker.IsGrabbable = 0;
            
            return true; //Successful attack, if we ever need to check that.
        }

        public bool ActionGuard(int roll)
        {
            var attacker = this;
            var target = TeamBattlefield.GetTarget();
            var partner = TeamBattlefield.GetPartner();
            var othertarget = TeamBattlefield.GetOther();
            var requiredStam = 0;
            var difficulty = 1; //Base difficulty, rolls greater than this amount will hit.

            //If opponent fumbled on their previous action they should become stunned.
            if (target.Fumbled)
            {
                target.IsDazed = true;
                target.Fumbled = false;
            }


            if (attacker.IsRestrained) difficulty += (11 + (int)Math.Floor((double)(target.Strength - attacker.Strength) / 2)); //When grappled, up the difficulty based on the relative strength of the combatants.
            if (attacker.IsRestrained) difficulty -= attacker.IsEscaping; //Then reduce difficulty based on how much effort we've put into escaping so far.
            if (target.IsRestrained) difficulty -= 4; //Lower the difficulty considerably if the target is restrained.

            if (attacker.IsEvading > 0)
            {//Apply attack bonus from move/teleport then reset it.
                attacker.IsEvading = 0;
            }
            if (attacker.IsAggressive > 0)
            {//Apply attack bonus from move/teleport then reset it.
                difficulty -= attacker.IsAggressive;
                attacker.IsAggressive = 0;
            }

            if (attacker.IsGuarding > 0)
            {//Apply attack bonus from move/teleport then reset it.
                attacker.IsGuarding = 0;
            }

            if (attacker.Stamina < requiredStam)
            {   //Not enough stamina-- reduced effect
                difficulty += (int)Math.Ceiling((double)((requiredStam - attacker.Stamina) / requiredStam) * (20 - difficulty)); // Too tired? You're going to fail.
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " didn't have enough Stamina and took a penalty to the escape attempt.");
            }

            attacker.HitStamina(requiredStam); //Now that stamina has been checked, reduce the attacker's stamina by the appopriate amount.

            var attackTable = attacker.BuildActionTable(difficulty, target.Dexterity, attacker.Dexterity, target.Stamina, target.StaminaCap);
            //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
            TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));

            
            if (roll <= attackTable.miss)
            {   //Miss-- no effect.
                TeamBattlefield.WindowController.Hit.Add(" FAILED!");
                return false; //Failed attack, if we ever need to check that.
            }

            if (roll >= attackTable.crit)
            { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                TeamBattlefield.WindowController.Hit.Add(" CRITICAL SUCCESS! ");
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " can perform another action!");
                // The only way the target can be stunned is if we set it to stunned with the action we're processing right now.
                // That in turn is only possible if target had fumbled. So we restore the fumbled status, but keep the stun.
                // That way we properly get a third action.
                if (target.IsDazed) target.Fumbled = true;
                target.IsDazed = true;
                partner.IsDazed = true;
                othertarget.IsDazed = true;
                if (target.IsDisoriented > 0) target.IsDisoriented += 2;
                if (target.IsExposed > 0) target.IsExposed += 2;
            }

            //The total mobility bonus generated. This will be split bewteen attack and defense.
            var totalBonus = Utils.RollDice(new List<int>() { 5, 5 }) - 1 + attacker.Willpower;

            {
                attacker.IsEvading = (int)Math.Ceiling((double)totalBonus / 2);
                TeamBattlefield.WindowController.Hit.Add(attacker.Name + " is guarding against all attacks for one turn!");
            }

            
            return true; //Successful attack, if we ever need to check that.
        }


        public bool ActionTeleport(int roll)
        {
            var attacker = this;
            var target = TeamBattlefield.GetTarget();
            var partner = TeamBattlefield.GetPartner();
            var othertarget = TeamBattlefield.GetOther();
            var requiredMana = 5;
            var difficulty = 1; //Base difficulty, rolls greater than this amount will hit.

            //If opponent fumbled on their previous action they should become stunned.
            if (target.Fumbled)
            {
                target.IsDazed = true;
                target.Fumbled = false;
            }

            if (attacker.IsRestrained) difficulty += (11 + (int)Math.Floor((double)(target.Spellpower + target.Strength - attacker.Spellpower - attacker.Strength) / 2)); //When grappled, up the difficulty based on the relative strength of the combatants.
            if (attacker.IsRestrained) difficulty -= attacker.IsEscaping; //Then reduce difficulty based on how much effort we've put into escaping so far.
            if (target.IsRestrained) difficulty -= 4; //Lower the difficulty considerably if the target is restrained.

            if (attacker.IsEvading > 0)
            {//Apply attack bonus from move/teleport then reset it.
                attacker.IsEvading = 0;
            }
            if (attacker.IsAggressive > 0)
            {//Apply attack bonus from move/teleport then reset it.
                difficulty -= attacker.IsAggressive;
                attacker.IsAggressive = 0;
            }

            if (attacker.IsGuarding > 0)
            {//Apply attack bonus from move/teleport then reset it.
                attacker.IsGuarding = 0;
            }

            if (attacker.Mana < requiredMana)
            {   //Not enough stamina-- reduced effect
                difficulty += (int)Math.Ceiling((double)((requiredMana - attacker.Mana) / requiredMana) * (20 - difficulty)); // Too tired? You're going to fail.
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " didn't have enough Mana and took a penalty to the attempt.");
            }

            attacker.HitMana(requiredMana); //Now that mana has been checked, reduce the attacker's mana by the appopriate amount.

            var attackTable = attacker.BuildActionTable(difficulty, 0, 0, target.Mana, target.ManaCap);
            //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
            TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));

            var tempGrappleFlag = true;
            if (attacker.IsGrappling(target))
            { //If you're grappling someone they are freed, regardless of the outcome.
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " used ESCAPE. " + target.Name + " is no longer being grappled. ");
                attacker.IsRestraining = false; 
                target.RemoveGrappler(attacker);
                tempGrappleFlag = false;
            }

            if (roll <= attackTable.miss)
            {   //Miss-- no effect.
                TeamBattlefield.WindowController.Hit.Add(" FAILED!");
                if (attacker.IsRestrained) attacker.IsEscaping += 4;//If we fail to escape, it'll be easier next time.
                return false; //Failed attack, if we ever need to check that.
            }

            if (roll >= attackTable.crit)
            { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                TeamBattlefield.WindowController.Hit.Add(" CRITICAL SUCCESS! ");
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " can perform another action!");
                // The only way the target can be stunned is if we set it to stunned with the action we're processing right now.
                // That in turn is only possible if target had fumbled. So we restore the fumbled status, but keep the stun.
                // That way we properly get a third action.
                if (target.IsDazed) target.Fumbled = true;
                target.IsDazed = true;
                partner.IsDazed = true;
                othertarget.IsDazed = true;
                if (target.IsDisoriented > 0) target.IsDisoriented += 2;
                if (target.IsExposed > 0) target.IsExposed += 2;
            }

            //The total mobility bonus generated. This will be split bewteen attack and defense.
            var totalBonus = Utils.RollDice(new List<int>() { 5, 5 }) - 1 + attacker.Spellpower;

            if (target.IsGrappling(attacker))
            { //If you were being grappled, you get free.
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " escaped " + target.Name + "'s hold! ");
                target.IsRestraining = false; 
                attacker.RemoveGrappler(target);
                tempGrappleFlag = false;
                attacker.IsEvading = (int)Math.Floor((double)totalBonus / 2);
            }
            else
            {
                attacker.IsAggressive = (int)Math.Ceiling((double)totalBonus / 2);
                TeamBattlefield.WindowController.Hit.Add(attacker.Name + " gained mobility bonuses against " + target.Name + " for one turn!");
            }

            if (TeamBattlefield.InGrabRange)
            {
                TeamBattlefield.WindowController.Hit.Add(attacker.Name + " moved away!");
                TeamBattlefield.InGrabRange = false;
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " managed to put some distance between them and " + target.Name + " and is now out of grabbing range.");
            }
            attacker.IsGrabbable = 0;

            return true; //Successful attack, if we ever need to check that.
        }

        public bool ActionChangeTarget(int roll)
        {
            var attacker = TeamBattlefield.GetActor();
            var target = TeamBattlefield.GetTarget();
            var partner = TeamBattlefield.GetPartner();
            var othertarget = TeamBattlefield.GetOther();
            var difficulty = 6; //Base difficulty, rolls greater than this amount will hit.

            //If opponent fumbled on their previous action they should become stunned.

            if (attacker.IsRestrained) difficulty += (4); //When grappled, up the difficulty based on the relative strength of the combatants.

            if (attacker.IsAggressive > 0)
            {//Apply attack bonus from move/teleport then reset it.
                difficulty -= attacker.IsAggressive;
                attacker.IsAggressive = 0;
            }

            if (attacker.IsGuarding > 0)
            {//Apply attack bonus from move/teleport then reset it.
                attacker.IsGuarding = 0;
            }

            if (attacker.IsEvading > 0)
            {//Apply attack bonus from move/teleport then reset it.
                attacker.IsEvading = 0;
            }

            var attackTable = attacker.BuildActionTable(difficulty, 0, 0, target.Mana, target.ManaCap);
            //If target can dodge the atatcker has to roll higher than the dodge value. Otherwise they need to roll higher than the miss value. We display the relevant value in the output.
            TeamBattlefield.WindowController.Info.Add("Dice Roll Required: " + (attackTable.miss + 1));


            if (roll <= attackTable.miss)
            {   //Miss-- no effect.
                TeamBattlefield.WindowController.Hit.Add(" FAILED!");
                return false; //Failed attack, if we ever need to check that.
            }

            if (roll >= attackTable.crit)
            { //Critical Hit-- increased damage/effect, typically 3x damage if there are no other bonuses.
                TeamBattlefield.WindowController.Hit.Add(" CRITICAL SUCCESS! ");
                TeamBattlefield.WindowController.Hint.Add(attacker.Name + " can perform another action!");
                // The only way the target can be stunned is if we set it to stunned with the action we're processing right now.
                // That in turn is only possible if target had fumbled. So we restore the fumbled status, but keep the stun.
                // That way we properly get a third action.
                if (target.IsDazed) target.Fumbled = true;
                target.IsDazed = true;
                partner.IsDazed = true;
                othertarget.IsDazed = true;
            }

            if (roll >= attackTable.targethit)
            {

                TeamBattlefield.WindowController.Hit.Add("Success! Your target has changed!");
                if (attacker.SetTarget == 1)
                    attacker.SetTarget -=1;
                if (attacker.SetTarget == 0)
                    attacker.SetTarget += 1;
                
            }

            


            return true; //Successful attack, if we ever need to check that.
        }

        public bool CanDodge(TeamFighter attacker)
        {
            return !(IsGrappling(attacker) || IsGrappledBy.Count != 0);
        }

        public bool IsGrappling(TeamFighter target)
        {
            return target.IsGrappledBy.Contains(Name);
        }

        public void RemoveGrappler(TeamFighter target)
        {
            IsGrappledBy.Remove(target.Name);
        }
    }

}
