namespace BesmashContent {
    public class Stats {
        public int VIT {get; protected set;}    // vitality
        public int ATK {get; protected set;}    // attack
        public int DEF {get; protected set;}    // defense
        public int AGI {get; protected set;}    // agility

        public Stats() : this(0, 0, 0, 0) {}
        public Stats(int vit, int atk, int def, int agi) {
            VIT = vit;
            ATK = atk;
            DEF = def;
            AGI = agi;
        }
    }
}