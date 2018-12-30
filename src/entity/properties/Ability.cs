namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using System.Collections.Generic;
    using System.Linq;

    public class Ability : GameObject {
        /// The title of this ability
        public string Title {get; protected set;}

        /// User of this ability (make sure this is set)
        public Creature User {get; set;}
        
        /// Wether this ability is currently executed
        public bool IsExecuting {get; protected set;}

        /// The projectile this ability may uses (can be null)
        public Projectile Projectile {get; protected set;}

        /// Animation to be shown at user position on execution. The
        /// ability wont trigger until this animation has finished
        public SpriteAnimation OnExecuteAnimation {get; protected set;}

        /// Animation to be shown on target position
        public SpriteAnimation OnHitAnimation {get; protected set;}
        
        private Point target;
        private bool projectileStarted;

        /// Executes the ability on the given target
        /// relative to the user position
        public virtual void execute(Point target) {
            this.target = new Point(
                (int)User.Position.X + target.X,
                (int)User.Position.Y + target.Y);

            IsExecuting = true;
            projectileStarted = false;

            if(OnExecuteAnimation != null) {
                OnExecuteAnimation.Position = User.Position;
                User.ContainingMap.addAnimation(OnExecuteAnimation);
            }
        }

        public override void load(ContentManager content) {
            // base.load(content); // TODO (is Ability really a GameObject?)
            if(Projectile != null) Projectile.load(content);
            if(OnHitAnimation != null) OnHitAnimation.load(content);
            if(OnExecuteAnimation != null) OnExecuteAnimation.load(content);
        }

        public override void update(GameTime gameTime) {
            // this is gonna be...
            if(!IsExecuting) return;
            base.update(gameTime);

            if(OnExecuteAnimation != null
            && OnExecuteAnimation.IsRunning)
                return; // ...wait for it...

            if(Projectile != null && !projectileStarted) {
                User.ContainingMap.addEntity(Projectile);
                Projectile.Position = User.Position;
                Projectile.Facing = User.Facing;
                Projectile.shoot();
                projectileStarted = true;
            }

            if(Projectile != null
            && Projectile.ContainingMap != null)
                return;

            User.ContainingMap
                .getEntities(target.X, target.Y)
                .Where(e => e is Creature).Cast<Creature>()
                .ToList().ForEach(onHit);

            onHit(null);
            IsExecuting = false;
            // ...legendary!
        }

        /// Evaluates the effects when a creature is hit by
        /// this ability and starts the on hit animation
        public virtual void onHit(Creature target) {
            OnHitAnimation.Position = new Vector2(
                this.target.X, this.target.Y);

            if(OnHitAnimation != null && target != null)
                target.ContainingMap.addAnimation(OnHitAnimation);
        }

        /// Gets called on start of execution on each animation
        public virtual void onExecute(SpriteAnimation animation) {}
    }
}