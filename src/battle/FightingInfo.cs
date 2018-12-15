namespace BesmashContent
{
    using System.Collections.Generic;
    public struct FightingInfo
    {
        public enum Faction {Allied, Hostile, Neutral, Solo, ThirdParty}
        public Faction faction{get;set;}
        /*  Bestimmt für welche Seite die Kreatur kämpft.
            Allied: Auf seiten der Spieler.
            Hostile: Auf Seiten der Gegner
            Neutral: Greift nicht an
            Solo: Greift alles an
            ThirdParty: Kämpfen sowohl gegen Allied als auch Hostile, aber nicht untereinander
         */
        public Creature Creature{get;set;}   //Refferenz auf die jeweilige Creature
        public int priority {get; set;}    //Die Reihenfolge, in der die Creatures an der Reihe sind
        public int times {get; set;}   //Wie oft eine Creature pro Runde angreifen kann
        public int temporalAgility {get; set;} //Wird von der Funktion "calculateFigtingOrder" genutzt
        public Stats stats {get; set;}
        public List<Buff> battleBuffs{get; set;} //Buffs und debuffs 

        public FightingInfo(Creature Creature, Faction alignment)
        {
            this.Creature = Creature;
            this.priority = 0;
            this.times = 0;
            this.temporalAgility = 0;
            this.stats = Creature.BaseStats.combineStats(Creature.StatsModifier);
            this.battleBuffs = new List<Buff>();
            this.faction = alignment;
        }

        public static bool IsFriendlyTo(FightingInfo a, FightingInfo b)
        {
            switch (a.faction)
            {
                case Faction.Allied :
                    switch (b.faction)
                    {
                        case Faction.Allied : return true;
                        case Faction.Hostile : return false;
                        case Faction.Neutral : return true;
                        case Faction.Solo : return false;
                        case Faction.ThirdParty : return false;
                        default : return true;
                    }

                case Faction.Hostile :
                    switch (b.faction)
                    {
                        case Faction.Allied : return false;
                        case Faction.Hostile : return true;
                        case Faction.Neutral : return true;
                        case Faction.Solo : return false;
                        case Faction.ThirdParty : return false;
                        default : return true;
                    }

                case Faction.Neutral :
                    switch (b.faction)
                    {
                        case Faction.Allied : return true;
                        case Faction.Hostile : return true;
                        case Faction.Neutral : return true;
                        case Faction.Solo : return false;
                        case Faction.ThirdParty : return true;
                        default : return true;
                    }

                case Faction.Solo :
                    switch (b.faction)
                    {
                        case Faction.Allied : return false;
                        case Faction.Hostile : return false;
                        case Faction.Neutral : return false;
                        case Faction.Solo : return false;
                        case Faction.ThirdParty : return false;
                        default : return false;
                    }

                case Faction.ThirdParty:
                    switch (b.faction)
                    {
                        case Faction.Allied : return false;
                        case Faction.Hostile : return false;
                        case Faction.Neutral : return true;
                        case Faction.Solo : return false;
                        case Faction.ThirdParty : return true;
                        default : return true;
                    }
                default : return true;
            }
        }
    }
}