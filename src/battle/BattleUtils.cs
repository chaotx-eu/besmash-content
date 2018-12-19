namespace BesmashContent
{
    using System;
    using System.Collections.Generic;
    public class BattleUtils
    {
        private static BattleUtils instance = null;
        public List<FightingInfo> FightingEntities;
        public Random random;
        public TileMap map;
        private bool battleIsHappening;
        public struct groupToAdd
        {
            public List<Creature> group {get;set;}
            public FightingInfo.Faction Alignment {get;set;}
            public groupToAdd(List<Creature> creatures, FightingInfo.Faction side)
            {
                group = creatures; Alignment = side;
            }
        }
        public List<groupToAdd> AddTheeseGroups{get;set;}

        public static BattleUtils newInstance()
        {
            if(instance == null)
                instance = new BattleUtils();
            return instance;
        }

        private BattleUtils()
        {
            random = new Random();
            FightingEntities = new List<FightingInfo>();
            battleIsHappening = false;
            AddTheeseGroups = new List<groupToAdd>();
        }

        public void add(List<Creature> creatures, FightingInfo.Faction side)
        {
            AddTheeseGroups.Add(new groupToAdd(creatures, side));
        }

        public bool attack(FightingInfo attacker, FightingInfo defender, OffensiveAbility attack)
        {
            bool success = false;   //Rückgabe, ob der Angriff erfolgreich war oder nicht (eventuell nötig für irgendwas)
            float damageMultiplier; //Multiplikator errechnet sich aus den Werten von Angreifer und Verteidiger
            float dodgeRate = (attacker.stats.ACC) / (defender.stats.DDG);
            if(attack.IsMagical)
                damageMultiplier = ((float)attacker.stats.MGA) / ((float)defender.stats.MGD);
            else
                damageMultiplier = ((float)attacker.stats.ATK) / ((float)defender.stats.DEF);

            if(random.Next(100) > dodgeRate * attack.BaseAcc)
                return false;
            success = defender.Creature.isDealtDamage((int) (damageMultiplier * attack.BaseDamage));
            
            if(defender.Creature.status.dead)
                attacker.Creature.onKill();

            return success;
        }


        public void heal(FightingInfo source, FightingInfo reciever, int amount)
        {
            reciever.Creature.CurrentHP += amount;
        }
    }
}