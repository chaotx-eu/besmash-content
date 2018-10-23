namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Content;

    public class Tile : MapObject {
        /// Solid Tiles cannot be passed by
        /// IMovable objects.
        [ContentSerializer(Optional = true)]
        public bool Solid {get; set;} = false;
        
        public override void update(GameTime time) {
            DestinationRectangle = new Rectangle(
                Position.X*ContainingMap.TileWidth,
                Position.Y*ContainingMap.TileHeight,
                ContainingMap.TileWidth,
                ContainingMap.TileHeight);
        }
    }
}