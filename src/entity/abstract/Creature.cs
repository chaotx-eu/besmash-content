namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using System.Runtime.Serialization;
    using System.Collections.Generic;
    using System.Linq;

    [DataContract]
    public class Creature : Movable {
        /// Creatures may wear armor
        [DataMember]
        public Helmet Helmet {get; set;}

        [DataMember]
        public Chestplate Chestplate {get; set;}

        [DataMember]
        public Pants Pants {get; set;}

        /// Creatures may hold a weapon
        [DataMember]
        public Weapon Weapon {get; set;}

        /// The name of this creature
        public string Name {
            get {return name == null ? Title : name;}
            protected set {name = value;}
        }

        [DataMember]
        private string name;

        /// The stats of this creature
        [DataMember]
        public Stats Stats {get; protected set;}

        /// Basic attack of this creature
        [DataMember]
        public BasicAttack BasicAttack {get; protected set;}

        /// Abilities this creature is capable of
        [DataMember]
        public List<Ability> Abilities {get; protected set;}

        public Creature() : this("") {}
        public Creature(string spriteSheet) : base(spriteSheet) {
            Abilities = new List<Ability>();
        }

        /// Loads required resources for this creature
        public override void load(ContentManager content) {
            base.load(content);
            Abilities.ForEach(a => a.load(content));
            if(BasicAttack != null) BasicAttack.load(content);
        }

        /// Adds an preloaded ability to this
        /// creatures ability list
        public void addAbility(Ability ability) {
            // if(IsLoaded) ability.load(ContainingMap.Content);
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

        /// Updates this creature and any abilities that
        /// are currently executed
        public override void update(GameTime gameTime) {
            base.update(gameTime);

            Abilities.Where(a => a.IsExecuting)
                .ToList().ForEach(a => a.update(gameTime));

            if(BasicAttack != null && BasicAttack.IsExecuting)
                BasicAttack.update(gameTime);
        }
    }
}