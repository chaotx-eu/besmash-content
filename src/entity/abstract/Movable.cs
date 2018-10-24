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
            if(ContainingMap.Slave != this)
                base.update(time);
        }
    }
}