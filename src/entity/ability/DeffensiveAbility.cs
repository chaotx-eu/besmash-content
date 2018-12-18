namespace BesmashContent
{
    public abstract class DeffensiveAbility : Ability
    {
        public int maxRange{get; set;} //0 for self target abilitys, -1 for Abilitys that do not require a target
        
        private Creature priv_target;
        public Creature target{get{return priv_target;} set{if(isInRange(value)) priv_target = value;}} //Wer soll das Ziel der Attacke sein? (Nur möglich, wennd as Ziel in Reichweite ist)

        public DeffensiveAbility() {} // TODO quick fix for content serializer
        public DeffensiveAbility(Creature user, int cost, string name, int range) : base(user, cost, name)
        {
            maxRange = range;
        }
        public void determineTarget()
        {
            //todo
        }
        public bool isInRange(Creature target)    //Überprüft ob das Ziel in Reichweite des Nutzers ist
        {
            //todo
            return true;        //Bis ich es implementiert hab, ist erstmal alles in Reichweite
        }
    }
}