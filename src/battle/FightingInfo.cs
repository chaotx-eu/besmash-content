namespace BesmashContent
{
    using System.Collections.Generic;
    public struct FightingInfo
    {
        public Creature Creature{get;set;}   //Refferenz auf die jeweilige Creature
        public int priority {get; set;}    //Die Reihenfolge, in der die Creatures an der Reihe sind
        public int times {get; set;}   //Wie oft eine Creature pro Runde angreifen kann
        public int temporalAgility {get; set;} //Wird von der Funktion "calculateFigtingOrder" genutzt
        public Stats stats {get; set;}
        public List<Buff> battleBuffs{get; set;} //Buffs und debuffs 

        public FightingInfo(Creature Creature)
        {
            this.Creature = Creature;
            this.priority = 0;
            this.times = 0;
            this.temporalAgility = 0;
            this.stats = Creature.BaseStats.combineStats(Creature.StatsModifier);
            this.battleBuffs = new List<Buff>();
        }
    }
}