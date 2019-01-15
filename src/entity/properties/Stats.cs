namespace BesmashContent {
    using System.Runtime.Serialization;

    public enum StatType {Vit, Atk, Int, Def, Wis, Agi}

    /// Helper class which comes with a set
    /// of modifiers that the true stats of
    /// the targetet creature may be
    /// multiplied with
    public class StatsMod : ICloneable {
        public float VITMod {get; set;} = 1f;
        public float ATKMod {get; set;} = 1f;
        public float INTMod {get; set;} = 1f;
        public float DEFMod {get; set;} = 1f;
        public float WISMod {get; set;} = 1f;
        public float AGIMod {get; set;} = 1f;

        public object clone() {
            return MemberwiseClone();
        }
    }

    [DataContract]
    public class Stats : ICloneable {
        public static Stats operator*(Stats stats, StatsMod statsMod) {
            return new Stats(
                (int)(stats.VIT*statsMod.VITMod),
                (int)(stats.ATK*statsMod.ATKMod),
                (int)(stats.INT*statsMod.INTMod),
                (int)(stats.DEF*statsMod.DEFMod),
                (int)(stats.WIS*statsMod.WISMod),
                (int)(stats.AGI*statsMod.AGIMod)
            );
        }

        /// The determined max value that can be
        /// possibly reached on one single stat
        public static int DeterminedMax {get;} = 1000;

        /// Amount of stats added to a creature on level up
        public static int LevelUpPoints {get;} = 8;

        /// Grow rate of stats used for several calculations
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

        public object clone() {
            return MemberwiseClone();
        }
    }
}