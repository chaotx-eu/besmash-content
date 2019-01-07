namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using System.Runtime.Serialization;
    using System.Collections.Generic;
    using System.Linq;
    using System;

    [DataContract]
    public class Creature : Movable {
        /// The max reachable level
        public static int MaxLevel {get;} = 100;

        /// The amount of exp a creature requires
        /// to reach level one from level zero
        public static int BirthExp {get;} = 100;

        /// The name of this creature
        [ContentSerializer(Optional = true)]
        public string Name {
            get {return name == null ? Title : name;}
            protected set {name = value;}
        }

        [DataMember]
        private string name;

        /// The class of this creature
        [DataMember]
        [ContentSerializer(Optional = true)]
        public Class Class {get; set;}

        /// The stats of this creature
        [DataMember]
        [ContentSerializer(Optional = true)]
        public Stats Stats {get; set;}

        /// The element of this creature
        [DataMember]
        [ContentSerializer(Optional = true)]
        public Element Element {get; set;}

        /// Creatures may wear armor
        [DataMember]
        [ContentSerializer(Optional = true)]
        public Helmet Helmet {get; set;}

        [DataMember]
        [ContentSerializer(Optional = true)]
        public Chestplate Chestplate {get; set;}

        [DataMember]
        [ContentSerializer(Optional = true)]
        public Pants Pants {get; set;}

        /// Creatures may hold a weapon
        [DataMember]
        [ContentSerializer(Optional = true)]
        public Weapon Weapon {get; set;}

        /// The current level of this creature
        [DataMember]
        [ContentSerializer(Optional = true)]
        public int Level {get; set;}

        /// The amount of gathered exp in this level
        [DataMember]
        [ContentSerializerIgnore]
        public int Exp {get; set;}

        /// The current hit points of this creature
        [DataMember]
        [ContentSerializerIgnore]
        public int HP {get; set;}

        /// The current action points of this creature
        [DataMember]
        [ContentSerializerIgnore]
        public int AP {get; set;}

        /// Abilities this creature is capable of
        [DataMember]
        [ContentSerializerIgnore]
        public List<Ability> Abilities {get; protected set;}

        /// List of effects that are currently attached
        /// to this creature
        [DataMember]
        [ContentSerializerIgnore]
        public List<AbilityEffect> Effects {get; protected set;}

        /// Random number generator of this creature
        [DataMember]
        [ContentSerializerIgnore]
        public Random RNG {get; protected set;}

        /// The max hit points of this creature
        [ContentSerializerIgnore]
        public int MaxHP {get {return (int)(Stats.VIT*10
            *(1 + (Stats.GrowRate*Stats.VIT/(float)Stats.DeterminedMax)
            *(1 + (Stats.GrowRate*Stats.VIT/(float)Stats.DeterminedMax))));
        }}

        /// The max action points of this creature
        [ContentSerializerIgnore]
        public int MaxAP {get {return 100;}} // TODO formula for max ap and ap per turn

        /// The amount of exp required to level up
        [ContentSerializerIgnore]
        public int MaxExp {
            // TODO some random formula (it is not to bad)
            get {return (int)(BirthExp*Math.Pow(Math.E, Level/6f));}
        }

        public Creature() : this("") {}
        public Creature(string spriteSheet) : base(spriteSheet) {
            Abilities = new List<Ability>();
            Effects = new List<AbilityEffect>();
            Class = new Class();
            Stats = new Stats();
            initRNG();
        }

        /// Loads required resources for this creature
        public override void load(ContentManager content) {
            base.load(content);
            Abilities.ForEach(a => a.load(content));
            if(Class != null) Class.Creature = this;
        }

        /// Adds an preloaded ability to this
        /// creatures ability list
        public void addAbility(Ability ability) {
            ability.User = this;
            Abilities.Add(ability);
        }

        /// Loads a new ability game object and
        /// adds it to this creatures ability list (TODO test)
        public void addAbility(string abilityFile, ContentManager content) {
            Ability ability = content.Load<Ability>(abilityFile);
            ability.User = this;
            ability.load(content);
            Abilities.Add(ability);
        }

        /// Updates this creature and any abilities
        /// that are currently executed
        public override void update(GameTime gameTime) {
            base.update(gameTime);

            Abilities.Where(a => a.IsExecuting)
                .ToList().ForEach(a => a.update(gameTime));
        }

        /// Applys any effects attached to this creature.
        /// They remove themselfs once worn off which is
        /// why this loop runs backwards
        public void applyEffects() {
            for(int i = Effects.Count-1; i >= 0; --i)
                Effects[i].apply();
        }

        /// Increases this creatures level by one
        /// if CurrentExp is greater or equal to
        /// ExpToNextLevel recursivly (TODO max level)
        public void levelUp() {
            levelUp(true);
        }

        public void levelUp(bool heal) {
            int excess = Exp - MaxExp;
            if(excess < 0) return;
            Exp = excess;
            Class.raiseStats();
            ++Level;
            if(heal) HP = MaxHP; // TODO test
            // TODO fire levelUpEvent
            levelUp();
        }

        /// Initializes the random number generator
        /// of this creature
        protected virtual void initRNG() {
            if(RNG == null) RNG = new Random();
        }
    }
}