namespace BesmashContent {
    using System;
    using System.Runtime.Serialization;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;

    [DataContract(IsReference = true)]
    public class Projectile : Movable {
        /// Max tiles this projectile may travel
        /// before it will be removed from the map
        [DataMember]
        [ContentSerializer(Optional = true)]
        public int MaxDistance {get; set;} = 32;

        /// Ability asset
        [DataMember]
        [ContentSerializer(Optional = true)]
        public GameAsset<Ability> AbilityAsset {get; set;}

        /// Reference to the source of this projectile
        [DataMember]
        [ContentSerializerIgnore]
        public Creature User {get; set;}

        /// Ability which is executed on collision
        /// relative to the position of the projectile
        [ContentSerializerIgnore]
        public Ability Ability {
            get {return AbilityAsset != null
                ? AbilityAsset.Object : null;}
        }

        /// Handler for when a projectile starts moving on a map
        public event EventHandler ProjectileStartedEvent;

        /// Handler for when this projectile is removed from the map
        public event EventHandler ProjectileRemovedEvent;

        private int distanceTraveled;
        private bool collided;

        public Projectile() : this(null) {}
        public Projectile(string spriteSheet) : base(spriteSheet) {
            initHandler();
        }

        /// Loads required resources for this
        /// projectile and its hit ability
        public override void load(ContentManager content) {
            base.load(content);
            if(AbilityAsset != null) AbilityAsset.load(content);
            if(Ability != null) Ability.load(content);
        }

        /// Starts moving this projectile towards its
        /// facing if it is contained by a map
        public void shoot() {
            if(ContainingMap == null) return;
            Rotation = (((int)Facing + 3) % 4)*90;
            StepTimeMultiplier = 1;
            distanceTraveled = 0;
            move();
            onProjectileStarted();
        }

        /// Initializes evend handler and similiar
        protected override void initHandler() {
            base.initHandler();
            MoveFinishedEvent += (sender, args) => {
                if(ContainingMap != null) {
                    if(++distanceTraveled > MaxDistance)
                        onCollision(null);
                    else move();
                }
            };

            CollisionResolver = (x, y, mos) => {
                foreach(MapObject mo in mos) if(mo is Creature
                || (mo is Tile) && ((Tile)mo).Solid) {
                    onCollision(mo);
                    return Point.Zero;
                }

                // TODO test acceleration
                StepTimeMultiplier -= StepTimeMultiplier/8f;
                return null;
            };
        }

        /// Updates this projectile and the hit ability
        /// after a collision
        public override void update(GameTime gameTime) {
            base.update(gameTime);

            if(collided) {
                if(Ability == null || !Ability.IsExecuting) {
                    ContainingMap.removeEntity(this);
                    onProjectileRemoved();
                } else if(Ability != null)
                    Ability.update(gameTime);
            }
        }

        /// Overrides clone to reinitialize event handler
        public new object clone() {
            Projectile projectile = base.clone() as Projectile;

            if(Ability != null) {
                projectile.AbilityAsset = AbilityAsset.clone() as GameAsset<Ability>;
                projectile.Ability.User = projectile;
            }

            projectile.initHandler();
            return projectile;
        }

        /// Disable dependency of sprite rectangle y-coord
        /// from facing of this projectile
        protected override void updateSprite(bool reset) {
            base.updateSprite(reset);
            SpriteRectangle = new Rectangle(
                SpriteRectangle.X, 0, // TODO (add property SpriteRow?)
                SpriteRectangle.Width,
                SpriteRectangle.Height);
        }
        
        /// Gets called when this projectile collides with
        /// another map object considered solid
        protected virtual void onCollision(MapObject target) {
            if(ContainingMap != null) {
                if(Ability != null)
                    Ability.execute();

                Color = Color.Transparent; // hide projectile
                collided = true;
            }
        }

        protected virtual void onProjectileStarted() {
            EventHandler handler = ProjectileStartedEvent;
            if(handler != null) handler(this, null);
        }

        protected virtual void onProjectileRemoved() {
            EventHandler handler = ProjectileRemovedEvent;
            if(handler != null) handler(this, null);;
        }
    }
}