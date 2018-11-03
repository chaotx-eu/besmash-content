namespace BesmashContent
{
    public struct BattleEntity
    {
        public Entity entity;   //Refferenz auf die jeweilige Entity
        public int priority {get; set;}    //Die Reihenfolge, in der die Entitys an der Reihe sind
        public int times {get; set;}   //Wie oft eine Entity pro Runde angreifen kann
        public int temporalAgility {get; set;} //Wird von der Funktion "calculateFigtingOrder" genutzt
    }
}