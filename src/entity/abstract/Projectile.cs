namespace BesmashContent {
    using Microsoft.Xna.Framework;

    public class Projectile : Movable {
        /// Animation to be shown on collision
        public SpriteAnimation CollisionAnimation {get; protected set;}

        public Projectile(string spriteSheet) : base(spriteSheet) {
            MoveFinishedEvent += (sender, args) => {
                if(ContainingMap != null) {
                    int x = Facing == Facing.EAST ? 1
                        : Facing == Facing.WEST ? -1 : 0;

                    int y = Facing == Facing.SOUTH ? 1
                        : Facing == Facing.NORTH ? -1 : 0;

                    move(x, y);
                }
            };

            CollisionResolver = (x, y, mos) => {
                foreach(MapObject mo in mos) if(!(mo is Tile)
                || (mo is Tile) && ((Tile)mo).Solid) {
                    onCollision(mo);
                    return Point.Zero;
                }

                // TODO test acceleration
                this.StepTime -= 10;
                return null;
            };
        }

        /// Starts moving this projectile towards its
        /// facing if it is contained by a map
        public void shoot() {
            if(ContainingMap == null) return;

            int x = Facing == Facing.EAST ? 1
                : Facing == Facing.WEST ? -1 : 0;

            int y = Facing == Facing.SOUTH ? 1
                : Facing == Facing.NORTH ? -1 : 0;

            move(x, y);
        }

        protected virtual void onCollision(MapObject target) {
            ContainingMap.removeEntity(this);
        }
    }
}