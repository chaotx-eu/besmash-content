namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using System.Runtime.Serialization;
    using System;

    /// An effect is something that can be attached to
    /// a creature and manipulate its properties over
    /// time and/or immediately
    [DataContract]
    public class AbilityEffect : ICloneable {
        /// Multiplier for critical hits
        public static float CritMultiplier {get;} = 1.5f;

        /// Animation which is shown on victim position
        /// when this effect is applied
        [DataMember]
        [ContentSerializer(ElementName = "Animation", Optional = true)]
        public string AnimationFile {get; set;}

        [DataMember]
        private SpriteAnimation animation;

        /// Base damage that is applied to the creature
        /// this effect is attached to
        [DataMember]
        [ContentSerializer(Optional = true)]
        public int BaseDamage {get; set;}

        /// Grow rate base damage will be multiplied
        /// with after any time this effect is applied
        [DataMember]
        [ContentSerializer(Optional = true)]
        public float BaseDamageGrow {get; set;}

        /// The property base damage will target
        [DataMember]
        [ContentSerializer(Optional = true)]
        public PropertyTarget BaseDamageTarget {get; set;}

        /// The type of base damgage dealt
        [DataMember]
        [ContentSerializer(Optional = true)]
        public DamageType BaseDamageType {get; set;}

        /// The element of base damage dealt
        [DataMember]
        [ContentSerializer(Optional = true)]
        public Element BaseDamageElement {get; set;}

        /// Wether the base damage will be recalculated
        /// taking the stats of the effect source (e.g.
        /// another player), if one exists, and the stats
        /// of the creature this effect is attached to
        /// into account. True by default
        [DataMember]
        [ContentSerializer(Optional = true)]
        public bool RecalculateBaseDamage {get; set;}

        /// Base heal that is applied to the creature
        /// this effect is attached to
        [DataMember]
        [ContentSerializer(Optional = true)]
        public int BaseHeal {get; set;}

        /// Grow rate base heal will be multiplied
        /// with after any time this effect is applied
        [DataMember]
        [ContentSerializer(Optional = true)]
        public float BaseHealGrow {get; set;}

        /// The property base heal will target
        [DataMember]
        [ContentSerializer(Optional = true)]
        public PropertyTarget BaseHealTarget {get; set;}

        /// Wether the base heal will be recalculated
        /// taking the stats of the effect source (e.g.
        /// another player), if one exists, and the stats
        /// of the creature this effect is attached to
        /// into account. True by default
        [DataMember]
        [ContentSerializer(Optional = true)]
        public bool RecalculateBaseHeal {get; set;}

        /// Percent damage that is applied to the
        /// creature this effect is attached to
        [DataMember]
        [ContentSerializer(Optional = true)]
        public int PercentDamage {get; set;}

        /// The property of the creature percent
        /// damage will be relative to
        [DataMember]
        [ContentSerializer(Optional = true)]
        public PercentTarget PercentDamageTarget {get; set;}

        /// Wether the percent damage will be recalculated
        /// taking the stats of the effect source (e.g.
        /// another player), if one exists, and the stats
        /// of the creature this effect is attached to
        /// into account
        [DataMember]
        [ContentSerializer(Optional = true)]
        public bool RecalculatePercentDamage {get; set;}

        /// Percent heal that is applied to the
        /// creature this effect is attached to
        [DataMember]
        [ContentSerializer(Optional = true)]
        public int PercentHeal {get; set;}

        /// The property of the creature percent
        /// heal will be relative to
        [ContentSerializer(Optional = true)]
        public PercentTarget PercentHealTarget {get; set;}

        /// Wether the percent heal will be recalculated
        /// taking the stats of the effect source (e.g.
        /// another player), if one exists, and the stats
        /// of the creature this effect is attached to
        /// into account
        [DataMember]
        [ContentSerializer(Optional = true)]
        public bool RecalculatePercentHeal {get; set;}

        /// Modifiers that are appplied to
        /// the stats of the targeted creature
        [DataMember]
        [ContentSerializer(Optional = true)]
        public StatsMod StatsMod {get; set;}

        /// How many turns this effect will
        /// stick to the creature where X is
        /// the min and Y is the max amount
        [DataMember]
        [ContentSerializer(Optional = true)]
        public Point TurnsToLast {get; set;}

        /// The current turn this effect is in. The
        /// counter gets incremented any time after
        /// this effect has been applied
        [DataMember]
        [ContentSerializerIgnore]
        public int CurrentTurn {get; protected set;}

        /// Reference to the creature
        /// that has applied the effect
        [DataMember]
        [ContentSerializerIgnore]
        public Creature User {get; protected set;}

        /// Reference to the creature
        /// this effect is attached to
        [DataMember]
        [ContentSerializerIgnore]
        public Creature Victim {get; protected set;}

        /// Indicates wether the effect is still active
        /// or if it can be removed from the creature
        [DataMember]
        [ContentSerializerIgnore]
        public bool IsActive {get; protected set;}

        /// The total amount of turns this effect will
        /// stick to a creature. Is set during attachement
        /// and will always be between TurnsToLast.X and
        /// TurnsToLast.Y (inclusive)
        [DataMember]
        [ContentSerializerIgnore]
        private int MaxTurns {get; set;}

        public void load(ContentManager content) {
            if(AnimationFile != null)
                animation = content.Load<SpriteAnimation>(AnimationFile);

            if(animation != null)
                animation.load(content);
        }

        /// Creates a new Effect with default properties
        public AbilityEffect() {
            TurnsToLast = new Point(1, 1);
            RecalculateBaseDamage = true;
            RecalculatePercentDamage = false;
            RecalculateBaseHeal = true;
            RecalculatePercentHeal = false;
            StatsMod = new StatsMod();
        }

        /// Attaches a new instance of this effect
        /// to the passed creature
        public void attach(Creature user, Creature victim) {
            AbilityEffect copy = clone() as AbilityEffect;
            copy.User = user;
            copy.Victim = victim;
            copy.CurrentTurn = 1;
            copy.IsActive = true;
            copy.MaxTurns = user.RNG.Next(TurnsToLast.X, TurnsToLast.Y+1);
            victim.Effects.Add(copy);
            copy.apply(); // affects are applied once when attached (TODO test)
        }

        /// Applies this effect to the creature
        /// according to the set properties
        public void apply() {
            if(Victim == null) return;

            if(++CurrentTurn > MaxTurns)
                Victim.Effects.Remove(this);

            if(BaseDamageTarget == PropertyTarget.HP)
                Victim.HP -= calculateBaseDamage();
            else if(BaseDamageTarget == PropertyTarget.AP)
                Victim.AP -= calculateBaseDamage();

            if(BaseHealTarget == PropertyTarget.HP)
                Victim.HP += calculateBaseHeal();
            else if(BaseHealTarget == PropertyTarget.AP)
                Victim.AP += calculateBaseHeal();

            if(animation != null && Victim.ContainingMap != null) {
                animation.Position = Victim.Position;
                Victim.ContainingMap.addAnimation(animation);
            }

            BaseDamage = (int)(BaseDamage*BaseDamageGrow);
            BaseHeal = (int)(BaseHeal*BaseHealGrow);

            // TODO PercentDamage and PercentHeal
        }

        /// Creates a clone of this effect
        public object clone() {
            AbilityEffect copy = MemberwiseClone() as AbilityEffect;
            copy.animation = animation == null ? null : animation.clone() as SpriteAnimation;
            return copy;
        }

        /// Calculates the base damage (TODO what if user == null)
        protected virtual int calculateBaseDamage() {
            // base damage of ability and weapon
            int baseDamage = BaseDamage + (User.Weapon == null ? 0
                : User.RNG.Next(User.Weapon.MinDamage, User.Weapon.MaxDamage+1));

            if(!RecalculateBaseDamage)
                return baseDamage;

            // relevant stat (physival -> ATK, magical -> INT)
            StatType relevantStat = BaseDamageType == DamageType.Magical
                ? StatType.Int : StatType.Atk;

            // chance for critical hit
            int critChance = 10 + (int)(100*((User.Stats.AGI
                + Math.Max(0, (User.Stats.AGI - User.Stats.get(relevantStat))))
                / (2f*Stats.DeterminedMax)));

            // the total amount of damage taking user stats
            // and elements into account (TODO better function)
            int stat = User.Stats.get(relevantStat);
            float mod = Stats.GrowRate*stat/(float)Stats.DeterminedMax;
            int totalDamage = (int)(baseDamage*(1 + mod*(1 + mod))
                *(User.RNG.Next(100) < critChance ? 1.5f : 1)
                *elementMult(BaseDamageElement, Victim.Element) + 0.5f);

            relevantStat = relevantStat == StatType.Int
                ? StatType.Wis : StatType.Def;

            // final recalculation of damage taking
            // victims defensive stats into account
            int finalDamage = (int)(totalDamage*(1 - (3*Victim.Stats.get(relevantStat))/(4f*Stats.DeterminedMax)));
            return finalDamage;
        }

        /// Calculates the base heal
        protected virtual int calculateBaseHeal() {
            // TODO
            return 0;
        }

        /// Calculates the percantage damage
        protected virtual int calculatePercentDamage() {
            // TODO
            return 0;
        }

        /// Calculates the percentage heal
        protected virtual int calculatePercentHeal() {
            // TODO
            return 0;
        }

        /// Helper to get the damage multiplier of the
        /// first passed element against the second one
        private float elementMult(Element from, Element against) {
            return from == Element.Fire
                ? against == Element.Water ? 0.5f
                : against == Element.None ? 1.5f
                : against == Element.Earth ? 2.0f : 1.0f
            : from == Element.Thunder
                ? against == Element.Earth ? 0.5f
                : against == Element.None ? 1.5f
                : against == Element.Water ? 2.0f : 1.0f
            : from == Element.Earth
                ? against == Element.Thunder ? 2.0f : 1.0f
            : from == Element.Water
                ? against == Element.Fire ? 2.0f : 1.0f
            : 1.0f; // None
        }
    }
}