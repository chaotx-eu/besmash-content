namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using System.Collections.Generic;
    using System.Linq;

    public class Creature : Movable {
        /// Creatures may wear armor
        public Helmet Helmet {get; set;}
        public Chestplate Chestplate {get; set;}
        public Pants Pants {get; set;}

        /// Creatures may hold a weapon
        public Weapon Weapon {get; set;}

        /// The name of this creature
        public string Name {
            get {return name == null ? Title : name;}
            protected set {name = value;}
        }

        private string name;

        /// The stats of this creature
        public Stats Stats {get; protected set;}

        /// Basic attack of this creature
        public BasicAttack BasicAttack {get; protected set;}

        /// Abilities this creature is capable of
        public List<Ability> Abilities {get; protected set;} = new List<Ability>();

        /// Loads required resources for this creature
        public override void load(ContentManager content) {
            base.load(content);
            Abilities.ForEach(a => a.load(content));
            if(BasicAttack != null) BasicAttack.load(content);
        }

        public Creature() : this("") {}
        public Creature(string spriteSheet) : base(spriteSheet) {}

        /// Adds an ability to this creatures ability list
        /// and loads its required resources (TODO)
        public void addAbility(Ability ability) {
            // if(IsLoaded) ability.load(ContainingMap.Content);
            ability.User = this;
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