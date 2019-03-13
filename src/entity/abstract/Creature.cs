namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Audio;
    using System.Runtime.Serialization;
    using System.Collections.Generic;
    using System.Linq;
    using System;

    [DataContract]
    public class Creature : Movable {
        /// The max reachable level
        public static int MaxLevel {get;} = 100;

        /// The name of this creature
        [ContentSerializer(Optional = true)]
        public string Name {
            get {return name == null ? Title : name;}
            protected set {name = value;}
        }

        [DataMember]
        private string name;

        /// Class asset
        [DataMember]
        [ContentSerializer(ElementName = "Class", Optional = true)]
        public GameAsset<Class> ClassAsset {get; set;}

        /// The stats of this creature
        [ContentSerializer(Optional = true)]
        public Stats Stats {
            get {
                Stats s = stats;
                Effects.ForEach(e => s *= e.StatsMod);
                return s;
            }
            set {stats = value;}
        }

        [DataMember]
        private Stats stats;

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

        /// The amount of exp this creature requires
        /// to reach level one from level zero. Also
        /// affects the amount of exp this creature
        /// grants when killed (default 100)
        [DataMember]
        [ContentSerializer(Optional = true)]
        public int BirthExp {get; set;} = 100;

        /// The base ap cost this creature requires
        /// to move the distance of one tile in battle
        /// default: 10
        [DataMember]
        [ContentSerializer(Optional = true)]
        public int MoveAP {get; set;} = 10;

        /// The amount of ap this creature gets at the
        /// begining of its turn in a battle
        // default: 10
        [DataMember]
        [ContentSerializer(Optional = true)]
        public int APGain {get; set;} = 10;

        /// Path to file used for death sound effect
        [DataMember]
        [ContentSerializer(ElementName = "DeathSound", Optional = true)]
        public string DeathSoundFile {get; set;}

        /// Path to file used for death attack effect
        [DataMember]
        [ContentSerializer(ElementName = "AttackSound", Optional = true)]
        public string AttackSoundFile {get; set;}

        /// Path to file used for damage sound effect
        [DataMember]
        [ContentSerializer(ElementName = "DamageSound", Optional = true)]
        public string DamageSoundFile {get; set;}
        
        /// Path to file used for level up sound effect
        [DataMember]
        [ContentSerializer(ElementName = "LevelUpSound", Optional = true)]
        public string LevelUpSoundFile {get; set;}

        /// Asset used for death animation
        [DataMember]
        [ContentSerializer(Optional = true)]
        public GameAsset<SpriteAnimation> DeathAnimationAsset {get; set;}

        /// Asset used for attack animation
        [DataMember]
        [ContentSerializer(Optional = true)]
        public GameAsset<SpriteAnimation> AttackAnimationAsset {get; set;}

        /// Asset used for damage animation
        [DataMember]
        [ContentSerializer(Optional = true)]
        public GameAsset<SpriteAnimation> DamageAnimationAsset {get; set;}

        /// Asset used for level up animation
        [DataMember]
        [ContentSerializer(Optional = true)]
        public GameAsset<SpriteAnimation> LevelUpAnimationAsset {get; set;}

        /// List of ability files this creature
        /// receives when loaded
        [ContentSerializer(ElementName = "DefaultAbilities", CollectionItemName ="Path", Optional = true)]
        public List<string> AbilityFiles {
            get {return abilityFiles == null
                ? (abilityFiles = new List<string>())
                : abilityFiles;}
            set {abilityFiles = value;}
        }

        [DataMember]
        private List<string> abilityFiles;

        /// The class of this creature
        [DataMember]
        [ContentSerializerIgnore]
        public Class Class {
            get {return ClassAsset != null
                ? ClassAsset.Object : clazz;}
            protected set {clazz = value;}
        }

        private Class clazz;

        /// The amount of gathered exp in this level
        [DataMember]
        [ContentSerializerIgnore]
        public int Exp {get; set;}

        /// The current hit points of this creature
        [DataMember]
        [ContentSerializerIgnore]
        public int HP {get; set;}

        /// The current action points of this creature
        [ContentSerializerIgnore]
        public int AP {
            get {return ap;}
            set {ap = Math.Min(MaxAP, value);}
        }

        [DataMember]
        private int ap;

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

        /// How much exp this creature grants
        /// when killed at its current level
        [ContentSerializerIgnore]
        public int ExpGrant {get { // TODO this is some random formular
            return (int)(MaxExp/(5 + 95*(Level/(float)MaxLevel)));
        }}

        /// Wether this creature is currently participating a battle
        [ContentSerializerIgnore]
        public bool IsFighting {get; set;}

        /// Sound effect that is played when this creature dies
        [ContentSerializerIgnore]
        public SoundEffect DeathSound {get; protected set;}

        /// Sound effect that is played when this creature attacks
        [ContentSerializerIgnore]
        public SoundEffect AttackSound {get; protected set;}

        /// Sound effect that is played when this creature takes damage
        [ContentSerializerIgnore]
        public SoundEffect DamageSound {get; protected set;}

        /// Sound effect that is played when this creature levels up
        [ContentSerializerIgnore]
        public SoundEffect LevelUpSound {get; protected set;}

        /// Animation that is shown instead of the current
        /// sprite sheet when this creature dies
        [ContentSerializerIgnore]
        public SpriteAnimation DeathAnimation {
            get {return DeathAnimationAsset != null
                ? DeathAnimationAsset.Object : null;}
        }

        /// Animation that is shown instead of the current
        /// sprite sheet when this creature attacks
        [ContentSerializerIgnore]
        public SpriteAnimation AttackAnimation {
            get {
                return AttackAnimationAsset != null
                ? AttackAnimationAsset.Object : null;}
        }

        /// Animation that is shown instead of the current
        /// sprite sheet when this creature takes damage
        [ContentSerializerIgnore]
        public SpriteAnimation DamageAnimation {
            get {
                return DamageAnimationAsset != null
                ? DamageAnimationAsset.Object : null;}
        }

        /// Animation that is shown instead of the current
        /// sprite sheet when this creature levels up
        [ContentSerializerIgnore]
        public SpriteAnimation LevelUpAnimation {
            get {
                return LevelUpAnimationAsset != null
                ? LevelUpAnimationAsset.Object : null;}
        }

        /// Active animation which starts running immediately
        /// when set. Resets back to null once finished
        [ContentSerializerIgnore]
        public SpriteAnimation ActiveAnimation {
            get {return activeAnimation;}
            set {
                if(value == null) {
                    if(activeAnimation != null
                    && activeAnimation.IsRunning)
                        activeAnimation.stop();

                    activeAnimation = null;
                } else {
                    activeAnimation = value.clone() as SpriteAnimation;
                    activeAnimation.Origin = this;
                    ContainingMap.addAnimation(activeAnimation, true);
                }
            }
        }

        private SpriteAnimation activeAnimation;

        /// Wether this creature is considered born i.e. it has
        /// been spawned once and the base stat points of the
        /// creatures class has been distributed
        [ContentSerializerIgnore]
        public bool IsBorn {get; protected set;}

        /// Event handler which is triggered when a creature dies
        public event EventHandler DeathEvent;

        /// Is triggered whenever this creature receives damage
        public event DamageEventHandler DamageEvent;

        public Creature() : this(null) {}
        public Creature(string spriteSheet) : base(spriteSheet) {
            Abilities = new List<Ability>();
            Effects = new List<AbilityEffect>();
            clazz = new Class();
            Stats = new Stats();
            initRNG();

            // TODO reinit handler on deserialization
            MoveStartedEvent += (sender, args) => {
                if(ContainingMap == null) return;
                if(ContainingMap.getEntities(args.Position)
                .Where(e => e is Creature).Count() < 2)
                    ContainingMap.getTile(args.Position).Occupied = false;

                ContainingMap.getTile(args.Target).Occupied = true;
            };
        }

        /// Loads required resources for this creature
        public override void load(ContentManager content) {
            base.load(content);
            AbilityFiles.ForEach(af => {
                Ability ability = content.Load<Ability>(af);
                if(Abilities.Where(a => a.Title == ability.Title).Count() == 0)
                    addAbility(ability.clone() as Ability);
            });

            Abilities.ForEach(a => a.load(content));

            if(DeathSoundFile != null)
                DeathSound = content.Load<SoundEffect>(DeathSoundFile);

            if(AttackSoundFile != null)
                AttackSound = content.Load<SoundEffect>(AttackSoundFile);

            if(DamageSoundFile != null)
                DamageSound = content.Load<SoundEffect>(DamageSoundFile);

            if(LevelUpSoundFile != null)
                LevelUpSound = content.Load<SoundEffect>(LevelUpSoundFile);

            if(DeathAnimationAsset != null) DeathAnimationAsset.load(content);
            if(AttackAnimationAsset != null) AttackAnimationAsset.load(content);
            if(DamageAnimationAsset != null) DamageAnimationAsset.load(content);
            if(LevelUpAnimationAsset != null) LevelUpAnimationAsset.load(content);

            if(DeathAnimation != null) {
                DeathAnimation.load(content);
                DeathAnimation.Origin = this;
            }

            if(AttackAnimation != null) {
                AttackAnimation.load(content);
                AttackAnimation.Origin = this;
            }

            if(DamageAnimation != null) {
                DamageAnimation.load(content);
                DamageAnimation.Origin = this;
            }

            if(LevelUpAnimation != null) {
                LevelUpAnimation.load(content);
                LevelUpAnimation.Origin = this;
            }

            if(ClassAsset != null) ClassAsset.load(content);
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
            if(HP <= 0) die();
            bool abilityRunning = false;

            Abilities.Where(a => a.IsExecuting).ToList().ForEach(a => {
                abilityRunning = true;
                a.update(gameTime);
            });

            if(!dieing) base.update(gameTime);
            else if(!abilityRunning && (ActiveAnimation == null
            || !ActiveAnimation.IsRunning)) {
                ContainingMap.BattleMap.Participants.Remove(this);
                ContainingMap.removeEntity(this);
            }
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
            levelUp(0);
        }

        public void levelUp(float healPercent) {
            int excess = Exp - MaxExp;
            if(excess < 0) return;
            Exp = excess;
            Class.raiseStats();
            ++Level;
            HP = Math.Min(MaxHP, HP + (int)(MaxHP*healPercent));

            if(LevelUpSound != null) LevelUpSound.Play();
            ActiveAnimation = LevelUpAnimation;
            // TODO fire levelUpEvent => onLevelUpd
            levelUp();
        }

        /// Marks this creature as born so the base
        /// stat points of this creatures class wont
        /// be distributed when this creature levels up
        public void setBorn() {
            IsBorn = true;
        }

        private bool dieing;
        /// Kills this creature in case its life is
        /// lower or equal to zero by running its
        /// death animation and then removing it
        /// from the map and any battle maps.
        public void die() {
            if(HP > 0 || dieing) return;
            onDeath(null);
            dieing = true;
        }

        /// Initializes the random number generator
        /// of this creature
        protected virtual void initRNG() {
            if(RNG == null) RNG = new Random();
        }

        protected void onDeath(EventArgs args) {
            EventHandler handler = DeathEvent;
            if(handler != null) handler(this, args);

            if(DeathSound != null) DeathSound.Play();
            ActiveAnimation = DeathAnimation;
        }

        public void onDamaged(DamageEventArgs args) {
            DamageEventHandler handler = DamageEvent;
            if(handler != null) handler(this, args);

            if(DamageSound != null) DamageSound.Play();
            ActiveAnimation = DamageAnimation;
        }

        public override void draw(SpriteBatch batch) {
            if(ActiveAnimation == null || !ActiveAnimation.IsRunning)
                base.draw(batch);
        }

        public new object clone() {
            Creature copy = base.clone() as Creature;
            if(ClassAsset != null)
                copy.ClassAsset = ClassAsset.clone() as GameAsset<Class>;

            copy.Stats = Stats.clone() as Stats;
            copy.RNG = new Random();
            copy.Effects = new List<AbilityEffect>();
            copy.Abilities = new List<Ability>();
            Abilities.ForEach(a => copy.addAbility(a.clone() as Ability));

            if(DeathAnimation != null) copy.DeathAnimationAsset =
                DeathAnimationAsset.clone() as GameAsset<SpriteAnimation>;
            
            if(AttackAnimation != null) copy.AttackAnimationAsset =
                AttackAnimationAsset.clone() as GameAsset<SpriteAnimation>;

            if(DamageAnimation != null) copy.DamageAnimationAsset =
                DamageAnimationAsset.clone() as GameAsset<SpriteAnimation>;

            if(LevelUpAnimation != null) copy.LevelUpAnimationAsset =
                LevelUpAnimationAsset.clone() as GameAsset<SpriteAnimation>;

            // TODO
            // copy.Weapon = Weapone.clone() as Weapon;
            // copy.Chestplate = Chestplate.clone() as Chestplate;
            // copy.Helmet = Helmet.clone() as Helmet;
            // copy.Pants = Pants.clone() as Pants;
            return copy;
        }
    }
}