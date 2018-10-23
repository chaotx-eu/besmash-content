namespace BesmashContent {
    using Microsoft.Xna.Framework;

    /// An object with the ability to move
    /// over a TileMap.
    public interface IMovable {
        Point Position {get; set;}
        void move(Point target);
    }
}