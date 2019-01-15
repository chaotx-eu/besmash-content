namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Linq;
    using System;
    using Utility;

    /// Ability components are used to build individual
    /// abilities with different behaviours
    [DataContract(IsReference = true)]
    public class AbilityComponent : ICloneable {
        /// Path to the animation object file
        /// this component shows on execution
        [DataMember]
        [ContentSerializer(ElementName = "Animation", Optional = true)]
        public string AnimationFile {get; set;}

        /// Path to the projectile object file
        /// this component shoots on execution
        [DataMember]
        [ContentSerializer(ElementName = "Projectile", Optional = true)]
        public string ProjectileFile {get; set;}

        /// The position relative to the parent component
        /// or in case there is none to the ability user
        [DataMember]
        [ContentSerializer(Optional = true)]
        public Point Position {get; set;}

        // TODO change Position => selected position for execution
        /// List of positions relative to the parent component
        /// or in case there is none to the ability user this
        /// component may be executed on
        /// not sure if i like it :?
        // [DataMember]
        // [ContentSerializer(Optional = true)]
        // public List<Point> Positions {get; set;}

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

        /// Wether this components animation should stick
        /// to its ability users position (default true)
        [DataMember]
        [ContentSerializer(Optional = true)]
        public bool StickyPosition {get; set;}

        /// Wether this components animation should stick
        /// to its ability users rotation, has no effect
        /// if RotateRelative is set to false (default true)
        [DataMember]
        [ContentSerializer(Optional = true)]
        public bool StickyRotation {get; set;}

        /// An effect which is attached to creatures on
        /// the same tile as this component on execution
        [DataMember]
        [ContentSerializer(Optional = true)]
        public AbilityEffect Effect {get; set;}

        /// A list of child ability components which will
        /// be executed alongside this one
        [DataMember]
        [ContentSerializer(CollectionItemName = "Component", Optional = true)]
        public List<AbilityComponent> Children {get; set;}

        /// A projectile which will be fired from this components
        /// towards the ability users facing (TODO) on execution
        [DataMember]
        [ContentSerializerIgnore]
        public Projectile Projectile {get; protected set;}

        /// An animtaion which is shown at the position
        /// of this component on execution
        [DataMember]
        [ContentSerializerIgnore]
        public SpriteAnimation Animation {get; protected set;}

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
            Children = new List<AbilityComponent>();
            StickyPosition = true;
            StickyRotation = true;
        }

        /// Loads any required ressources for this components
        /// animation and projectile as well as for its children
        public void load(ContentManager content) {
            if(AnimationFile != null && Animation == null)
                Animation = content.Load<SpriteAnimation>(AnimationFile);

            if(ProjectileFile != null && Projectile == null)
                Projectile = content.Load<Projectile>(ProjectileFile);

            if(Animation != null) Animation.load(content);
            if(Projectile != null) Projectile.load(content);
            if(Effect != null) Effect.load(content);

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

            if(Animation != null) {
                framer = animation.CurrentFrame;

                if(animation.IsRunning) {
                    if(StickyPosition)
                        updateAnimationPosition();

                    if(StickyRotation)
                        updateAnimationRotation();
                }
            }

            if(waitList.Count == 0 && activeList.Count == 0
            && (Animation == null || !animation.IsRunning)
            && (Projectile == null || projectile.ContainingMap == null))
                IsExecuting = false;
        }

        /// Executes this ability and adds all
        /// children to the wait list
        public void execute() {
            activeList = new List<AbilityComponent>();
            waitList = new List<AbilityComponent>(Children);
            timer = framer = 0;
            IsExecuting = true;

            if(Animation != null) {
                animation = Animation.clone() as SpriteAnimation;
                updateAnimationPosition();
                updateAnimationRotation();
                animation.ContainingMap = Ability.User.ContainingMap;
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
                        MapObject user = Ability.User is Projectile
                            ? ((Projectile)Ability.User).User : Ability.User;

                        Effect.attach(user as Creature, c);
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
        }

        /// Creates and returns a deep clone of this component
        public object clone() {
            AbilityComponent copy = MemberwiseClone() as AbilityComponent;
            List<AbilityComponent> children = new List<AbilityComponent>();

            if(Animation != null) copy.Animation = Animation.clone() as SpriteAnimation;
            if(Projectile != null) copy.Projectile = Projectile.clone() as Projectile;

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

        /// Updates the position of the animation
        private void updateAnimationPosition() {
            animation.Position = Ability.User.Position + (
                Parent != null ? MapUtils.rotatePoint(
                    Parent.Position + Position,
                    Ability.User.Facing)
                : MapUtils.rotatePoint(Position, Ability.User.Facing)
            ).ToVector2();
        }

        /// Updates the rotation of the animation
        private void updateAnimationRotation() {
            if(Animation.RotateRelative) {
                int faceDiff = (int)animation.Facing - (int)Ability.User.Facing;
                animation.Rotation = ((4 - faceDiff)%4)*90;
            }
        }
    }
}