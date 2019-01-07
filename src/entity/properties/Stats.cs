namespace BesmashContent {
    using System.Runtime.Serialization;

    public enum StatType {Vit, Atk, Int, Def, Wis, Agi}

    [DataContract]
    public class Stats {
        /// The determined max value that can be
        /// possibly reached on one single stat
        public static int DeterminedMax {get;} = 1000;

        /// Amount of stats added to a creature on level up
        public static int LevelUpPoints {get;} = 8;

        /// Grow rate for of stats used for several calculations
        public static int GrowRate {get;} = 14;

        [DataMember] public int VIT {get; protected set;}    // vitality (hp modifier)
        [DataMember] public int ATK {get; protected set;}    // attack (physical attack)
        [DataMember] public int INT {get; protected set;}    // intelligence (magic attack)
        [DataMember] public int DEF {get; protected set;}    // defense (physical defence)
        [DataMember] public int WIS {get; protected set;}    // wisdom (magic defence)
        [DataMember] public int AGI {get; protected set;}    // agility (turn speed modifier)

        public Stats() : this(1, 1, 1, 1, 1, 1) {}
        public Stats(int vit, int atk, int inT, int def, int wis, int agi) {
            add(vit, atk, inT, def, wis, agi);
        }

        /// Returns stat value of the corresponding stat type
        public int get(StatType stat) {
            return stat == StatType.Vit ? VIT
                : stat == StatType.Atk ? ATK
                : stat == StatType.Int ? INT
                : stat == StatType.Def ? DEF
                : stat == StatType.Wis ? WIS
                : stat == StatType.Agi ? AGI : -1;
        }

        /// Sets passed value for the corresponding stat
        public void set(int value, StatType stat) {
            if(stat == StatType.Vit) VIT = value;
            else if(stat == StatType.Atk) ATK = value;
            else if(stat == StatType.Int) INT = value;
            else if(stat == StatType.Def) DEF = value;
            else if(stat == StatType.Wis) WIS = value;
            else if(stat == StatType.Agi) AGI = value;
        }

        /// Adds passed value to the corresponding stat
        public void add(int value, StatType stat) {
            if(stat == StatType.Vit) VIT += value;
            else if(stat == StatType.Atk) ATK += value;
            else if(stat == StatType.Int) INT += value;
            else if(stat == StatType.Def) DEF += value;
            else if(stat == StatType.Wis) WIS += value;
            else if(stat == StatType.Agi) AGI += value;
        }

        /// Sets passed values to the stats
        public void set(int vit, int atk, int inT, int def, int wis, int agi) {
            VIT = vit;
            ATK = atk;
            INT = inT;
            DEF = def;
            WIS = wis;
            AGI = agi;
        }

        /// Adds passed values to the stats
        public void add(int vit, int atk, int inT, int def, int wis, int agi) {
            VIT += vit;
            ATK += atk;
            INT += inT;
            DEF += def;
            WIS += wis;
            AGI += agi;
        }
    }
}