namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Audio;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Linq;
    using System;
    using Utility;

    /// Ability components are used to build individual
    /// abilities with different behaviours
    [DataContract(IsReference = true)]
    public class AbilityComponent : ICloneable {
        /// Animation asset
        [DataMember]
        [ContentSerializer(Optional = true)]
        public GameAsset<SpriteAnimation> AnimationAsset {get; set;}

        /// Projectile asset
        [DataMember]
        [ContentSerializer(Optional = true)]
        public GameAsset<Projectile> ProjectileAsset {get; set;}

        /// Ability effect asset
        [DataMember]
        [ContentSerializer(Optional = true)]
        public GameAsset<AbilityEffect> EffectAsset {get; set;}

        /// Dictionary of SpriteAnimations with creature names as keys
        /// shown instead of the user sprite at execution of this component
        [DataMember]
        [ContentSerializer(CollectionItemName = "UserAnimation", Optional = true)]
        public Dictionary<string, GameAsset<SpriteAnimation>> UserAnimations {get; set;}

        /// Path to the sound effect file used by this component
        [DataMember]
        [ContentSerializer(ElementName = "ExecutionSound", Optional = true)]
        public string ExecutionSoundFile {get; set;}

        /// The position relative to the parent component
        /// or in case there is none to the ability user
        [DataMember]
        [ContentSerializer(Optional = true)]
        public Point Position {get; set;}

        /// The time in millisecond this component is
        /// executed after its parent has shown exactly or
        /// more than FrameOffset frames. If no parent exists
        /// the timer starts immediately after execution
        [DataMember]
        [ContentSerializer(Optional = true)]
        public int TimeOffset {get; set;}

        /// The min amount of sprites the parent has to
        /// show before this component will be executed.
        /// Values below 0 are interpreted as positive
        /// factor the parents total frames count is
        /// divided with. This value has no effect in
        /// case no parent exists
        [ContentSerializer(Optional = true)]
        public int FrameOffset {
            get {
                return frameOffset < 0 && Parent != null
                    ? -Parent.Animation.SpriteCount/frameOffset
                    : frameOffset;
            }

            set {frameOffset = value;}
        }

        [DataMember]
        private int frameOffset;

        /// A list of child ability components which will
        /// be executed alongside this one
        [ContentSerializer(CollectionItemName = "Component", Optional = true)]
        public List<AbilityComponent> Children {
            get {return children != null ? children
                : (children = new List<AbilityComponent>());}
            set {children = value;}
        }

        [DataMember]
        private List<AbilityComponent> children;

        /// Animation of the ability user shown at execution
        /// of this component
        [DataMember]
        [ContentSerializerIgnore]
        public SpriteAnimation UserAnimation {get; protected set;}

        /// A projectile which will be fired from this component
        /// towards the ability users facing on execution
        [ContentSerializerIgnore]
        public Projectile Projectile {
            get {return ProjectileAsset != null
                ? ProjectileAsset.Object : null;}
        }

        /// An animtaion which is shown at the position
        /// of this component on execution
        [ContentSerializerIgnore]
        public SpriteAnimation Animation {
            get {return AnimationAsset != null
                ? AnimationAsset.Object : null;}
        }

        /// An effect which is attached to creatures on
        /// the same tile as this component on execution
        [ContentSerializerIgnore]
        public AbilityEffect Effect {
            get {return EffectAsset != null
                ? EffectAsset.Object : null;}
        }

        /// The sound effect which is played on
        /// execution of this component
        [ContentSerializerIgnore]
        public SoundEffect ExecutionSound {get; set;}

        /// Reference to the ability this component belongs to
        [ContentSerializerIgnore]
        public Ability Ability {
            get {
                return ability != null
                    ? ability : Parent != null
                    ? Parent.Ability : null;
            }
            set {ability = value;}
        }

        [DataMember]
        private Ability ability;

        /// Reference to the parent component
        [DataMember]
        [ContentSerializerIgnore]
        public AbilityComponent Parent {get; protected set;}

        /// Wether this component is currently beeing executed
        [DataMember]
        [ContentSerializerIgnore]
        public bool IsExecuting {get; protected set;}

        [DataMember] private SpriteAnimation animation;
        [DataMember] private Projectile projectile;
        [DataMember] private List<AbilityComponent> activeList;
        [DataMember] private List<AbilityComponent> waitList;
        [DataMember] private bool projectileReady;
        [DataMember] private bool animationReady;
        [DataMember] private int timer;
        [DataMember] private int framer;

        /// Creates a new ability component object
        /// values with default 
        public AbilityComponent() {
            UserAnimations = new Dictionary<string, GameAsset<SpriteAnimation>>();
            Children = new List<AbilityComponent>();
        }

        /// Loads any required ressources for this components
        /// animation and projectile as well as for its children
        public void load(ContentManager content) {
            if(AnimationAsset != null)
                AnimationAsset.load(content);

            if(ProjectileAsset != null)
                ProjectileAsset.load(content);

            if(EffectAsset != null)
                EffectAsset.load(content);

            if(ExecutionSoundFile != null)
                ExecutionSound = content.Load<SoundEffect>(ExecutionSoundFile);

            if(Animation != null) Animation.load(content);
            if(UserAnimation != null) UserAnimation.load(content); // TODO check why this load is necessary when loading a save game
            if(Projectile != null) Projectile.load(content);
            if(Effect != null) Effect.load(content);

            Creature user;
            if((user = Ability.User as Creature) != null) {
                if(UserAnimations.ContainsKey(user.Name)) {
                    UserAnimations[user.Name].load(content);
                    UserAnimation = UserAnimations[user.Name].Object as SpriteAnimation;
                }
            }

            Children.ForEach(child => {
                child.Parent = this;
                child.load(content);
            });
        }

        /// Updates this components animation and projectile
        /// as well as its children
        public void update(GameTime gameTime) {
            if(!IsExecuting) return;
            timer += gameTime.ElapsedGameTime.Milliseconds;
            if(timer < TimeOffset) return;

            if(Animation != null && animationReady) {
                animationReady = false;
                Ability.User.ContainingMap.addAnimation(animation, true);
            }

            if(Projectile != null && projectileReady) {
                Ability.User.ContainingMap.addEntity(projectile);
                projectileReady = false;
                projectile.shoot();
            }

            int i;
            for(i = waitList.Count-1; i >= 0; --i) {
                AbilityComponent child = waitList[i];

                if(framer >= child.FrameOffset) {
                    child.execute();
                    waitList.RemoveAt(i);
                    activeList.Add(child);
                }
            }

            for(i = activeList.Count-1; i >= 0; --i) {
                AbilityComponent child = activeList[i];
                if(child.IsExecuting) child.update(gameTime);
                else activeList.RemoveAt(i);
            }

            if(Animation != null)
                framer = animation.CurrentFrame;

            if(waitList.Count == 0 && activeList.Count == 0
            && (Animation == null || !animation.IsRunning)
            && (Projectile == null || projectile.ContainingMap == null)) {
                IsExecuting = false;
                // TODO stop user animation ?
            }
        }

        /// Executes this ability component
        /// and its chidlren
        public void execute() {
            activeList = new List<AbilityComponent>();
            waitList = new List<AbilityComponent>(Children);
            timer = framer = 0;
            IsExecuting = true;

            if(Animation != null) {
                animation = Animation.clone() as SpriteAnimation;
                animation.ContainingMap = Ability.User.ContainingMap;
                animation.Origin = Ability.User;
                animation.Offset = Position + (Parent != null ? Parent.Position : Point.Zero);
                animationReady = true;
            }

            // TODO test attached effect
            if(Effect != null) {
                Ability.User.ContainingMap
                    .getEntities(getPosition())
                    .Where(e => e is Creature).Cast<Creature>()
                    // .ToList().ForEach(c => Effect.attach(
                    //     Ability.User as Creature, c));
                    // TODO crashes for user type == Projectile (hotfix below)
                    .ToList().ForEach(c => {
                        MapObject usr = Ability.User is Projectile
                            ? ((Projectile)Ability.User).User : Ability.User;

                        Effect.attach(usr as Creature, c);
                    });
            }

            if(Projectile != null) {
                Projectile.User = Ability.User as Creature; // TODO causes trouble for non creature types
                Projectile.Position = Ability.User.Position + (
                    Parent != null ? MapUtils.rotatePoint(
                        Parent.Position + Position,
                        Ability.User.Facing)
                    : MapUtils.rotatePoint(Position, Ability.User.Facing)
                ).ToVector2();

                Projectile.Facing = Ability.User.Facing;
                Projectile.ContainingMap = Ability.User.ContainingMap;
                projectile = Projectile.clone() as Projectile;
                projectileReady = true;
            }

            Creature user = Ability.User as Creature;
            if(UserAnimation != null && user != null)
                user.ActiveAnimation = UserAnimation;

            if(ExecutionSound != null)
                ExecutionSound.Play();
        }

        /// Creates and returns a deep clone of this component
        public object clone() {
            AbilityComponent copy = MemberwiseClone() as AbilityComponent;
            List<AbilityComponent> children = new List<AbilityComponent>();

            if(AnimationAsset != null) copy.AnimationAsset =
                AnimationAsset.clone() as GameAsset<SpriteAnimation>;

            if(ProjectileAsset != null) copy.ProjectileAsset =
                ProjectileAsset.clone() as GameAsset<Projectile>;

            if(EffectAsset != null) copy.EffectAsset =
                EffectAsset.clone() as GameAsset<AbilityEffect>;

            AbilityComponent child;
            Children.ForEach(orig => {
                child = orig.clone() as AbilityComponent;
                child.Parent = copy;
                children.Add(child);
            });

            copy.Children = children;
            return copy;
        }

        /// Gets true position of this component
        private Point getPosition() {
            if(Parent == null) return
                Ability.User.Position.ToPoint() + 
                MapUtils.rotatePoint(Position, Ability.User.Facing);

            return Parent.getPosition() +
                MapUtils.rotatePoint(Position, Ability.User.Facing);
        }
    }
}