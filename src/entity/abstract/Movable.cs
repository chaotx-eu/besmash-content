namespace BesmashContent {
    using Microsoft.Xna.Framework;

    /// An object with the ability to move
    /// over a TileMap. Will always be centered
    /// on the screen.
    public abstract class Movable : Entity {
        public virtual void move(Point target) {
            Position = target;
        }

        public override void update(GameTime time) {
            // Do nothing. Destination rect gets set
            // becomes slave on a map.
        }
    }
}