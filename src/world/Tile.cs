namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Content;

    public class Tile : MapObject {
        /// Solid Tiles cannot be passed by
        /// Movable objects.
        [ContentSerializer(Optional = true)]
        public bool Solid {get; set;} = false;
    }
}