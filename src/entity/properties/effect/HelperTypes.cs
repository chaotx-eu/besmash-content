namespace BesmashContent {
    using System.Runtime.Serialization;
    
    [DataContract]
    public enum PropertyTarget {
        [EnumMember] HP,
        [EnumMember] AP
    }

    [DataContract]
    public enum PercentTarget {
        [EnumMember] MaxHP,
        [EnumMember] CurrentHP,
        [EnumMember] MaxAP,
        [EnumMember] CurrentAP
    }

    /// Element Theory:
    /// There are five Elements in total. Two are
    /// offensive two defensive and one is neutral.
    /// Those are: None, Fire, Earth, Thunder, Water
    ///
    ///         | None  | Fire  | Earth |Thunder| Water |
    /// | None  |  1.0  |  1.0  |  1.0  |  1.0  |  1.0  |
    /// | Fire  |  1.5  |  1.0  |  2.0  |  1.0  |  0.5  |
    /// | Earth |  1.0  |  1.0  |  1.0  |  2.0  |  1.0  |
    /// |Thunder|  1.5  |  1.0  |  0.5  |  1.0  |  2.0  |
    /// | Water |  1.0  |  2.0  |  1.0  |  1.0  |  1.0  |
    [DataContract]
    public enum Element {
        [EnumMember] None,
        [EnumMember] Fire,
        [EnumMember] Earth,
        [EnumMember] Thunder,
        [EnumMember] Water
    }

    /// The type of damage determines which stats
    /// will be relevant during damage calculation
    [DataContract]
    public enum DamageType {
        [EnumMember] Physical,
        [EnumMember] Magical
    }
}