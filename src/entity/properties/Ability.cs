namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using Utility;
    
    public class Ability : GameObject {
        /// The title of this ability
        public string Title {get; set;}

        /// Path to the file of the OnHitAnimation
        [ContentSerializer(ElementName = "HitAnimation", Optional = true)]
        public string HitAnimationFile {get; set;}

        /// Path to the file of the OnChargeAnimation
        [ContentSerializer(ElementName = "ChargeAnimation", Optional = true)]
        public string ChargeAnimationFile {get; set;}

        /// Path to the file of the Projectile
        [ContentSerializer(ElementName = "Projectile", Optional = true)]
        public string ProjectileFile {get; set;}
        
        /// List of coordinates hit animations should
        /// target, relative to the user facing north
        /// (default = {(0, -1)})
        [ContentSerializerIgnore] // TODO investigate why Point is not suppurted by Content Reader when a member of this class?!
        public List<Point> Targets {get; set;}

        /// Path to the file of the User creature
        [ContentSerializer(ElementName = "User", Optional = true)]
        public string UserFile {get; set;}

        /// Animation to be shown on target position
        [ContentSerializerIgnore]
        public SpriteAnimation OnHitAnimation {get; protected set;}

        /// Animation to be shown at user position before execution.
        /// The ability wont trigger until this animation has finished
        [ContentSerializerIgnore]
        public SpriteAnimation OnChargeAnimation {get; protected set;}

        /// The projectile this ability may uses (can be null)
        [ContentSerializerIgnore]
        public Projectile Projectile {get; protected set;}

        /// User of this ability (make sure this is set)
        [ContentSerializerIgnore]
        public Creature User {get; set;}

        /// Wether this ability is currently executed
        [ContentSerializerIgnore]
        public bool IsExecuting {get; protected set;}

        /// Initializes a new ability with the default target coords
        public Ability() {
            Targets = new List<Point>();
            Targets.Add(new Point(0, -1));
        }

        /// Executes the ability on the given target
        /// relative to the user position
        public virtual void execute(Point target) {
            IsExecuting = true;
            if(OnChargeAnimation != null) {
                OnChargeAnimation.Position = User.Target.ToVector2();
                OnChargeAnimation.Facing = User.Facing;
                User.ContainingMap.addAnimation(OnChargeAnimation);
            } else if(Projectile != null)
                fireProjectile(User.Target.ToVector2(), User.Facing);
            else if(OnHitAnimation != null)
                hitTargets(User.Target.ToVector2(), User.Facing);
            else IsExecuting = false;
        }

        public override void load(ContentManager content) {
            // base.load(content); // TODO (is Ability really a GameObject?)

            if(Projectile == null && ProjectileFile != null) {
                Projectile = content.Load<Projectile>(ProjectileFile);
                Projectile.ProjectileRemovedEvent -= onProjectileRemoved;
                Projectile.ProjectileRemovedEvent += onProjectileRemoved;
            }

            if(OnHitAnimation == null && HitAnimationFile != null) {
                OnHitAnimation = content.Load<SpriteAnimation>(HitAnimationFile);
                OnHitAnimation.AnimationFinishedEvent -= onHitFinished;
                OnHitAnimation.AnimationFinishedEvent += onHitFinished;
            }

            if(OnChargeAnimation == null && ChargeAnimationFile != null) {
                OnChargeAnimation = content.Load<SpriteAnimation>(ChargeAnimationFile);
                OnChargeAnimation.AnimationFinishedEvent -= onChargedFinished;
                OnChargeAnimation.AnimationFinishedEvent += onChargedFinished;
            }

            if(Projectile != null) Projectile.load(content);
            if(OnHitAnimation != null) OnHitAnimation.load(content);
            if(OnChargeAnimation != null) OnChargeAnimation.load(content);
        }

        /// Evaluates the effects when a creature is hit by
        /// this ability and starts the on hit animation
        public virtual void onHit(Creature creature) {
            if(OnHitAnimation != null && creature != null)
                creature.ContainingMap.addAnimation(OnHitAnimation);
        }

        /// Helper to fire a copy of the projectile from the
        /// given position towards the passed facing
        protected virtual void fireProjectile(Vector2 position, Facing facing) {
            if(Projectile == null) return;
            Projectile projectile = Projectile.clone() as Projectile;
            projectile.Position = position;
            projectile.Facing = facing;
            User.ContainingMap.addEntity(projectile);
            projectile.shoot();
        }

        /// Helper to start copies of the hit animation on
        /// entities at the target positions relative to
        /// the passed position and facing
        protected virtual void hitTargets(Vector2 position, Facing facing) {
            if(OnHitAnimation == null) return;
            Targets.ForEach(p => {
                Point target = MapUtils.rotatePoint(p, facing)
                    + position.ToPoint();

                OnHitAnimation.Facing = facing;
                OnHitAnimation.Position = target.ToVector2();
                User.ContainingMap.addAnimation(OnHitAnimation);

                User.ContainingMap
                    .getEntities(target.X, target.Y)
                    .Where(e => e is Creature).Cast<Creature>()
                    .ToList().ForEach(onHit);
            });
        }

        private void onProjectileRemoved(
        object sender, EventArgs args) {
            IsExecuting = false;
        }

        private void onHitFinished(
        SpriteAnimation sender, AnimationEventArgs args) {
            IsExecuting = false;
        }

        private void onChargedFinished(
        SpriteAnimation sender, AnimationEventArgs args) {
            if(Projectile != null)
                fireProjectile(sender.Position, sender.Facing);
            else {
                hitTargets(sender.Position, sender.Facing);
            }
        }
    }
}