namespace BesmashContent
{
    public struct BattleEntity
    {
        public Entity entity;   //Refferenz auf die jeweilige Entity
        public int priority {get; set;}    //Die Reihenfolge, in der die Entitys an der Reihe sind
        public int times {get; set;}   //Wie oft eine Entity pro Runde angreifen kann
        public int temporalAgility {get; set;} //Wird von der Funktion "calculateFigtingOrder" genutzt
        public Stats stats {get; set;}
        public Stats battleBuffs{get; set;} //Buffs und debuffs (Änderungen von Werten, die nur für den Kampf gelten) 

        public BattleEntity(Entity entity)
        {
            this.entity = entity;
            this.priority = 0;
            this.times = 0;
            this.temporalAgility = 0;
            this.stats = entity.BaseStats.combineStats(entity.StatsModifier);
            this.battleBuffs = new Stats();
        }
    }
}