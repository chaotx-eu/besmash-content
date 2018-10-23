namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;

    /// Game objects which can be present on a map.
    public class MapObject : GameObject {
        [ContentSerializerIgnore]
        public TileMap ContainingMap {get; set;}
        
        /// Position on a TileMap.
        public Point Position {get; set;}

        public override void update(GameTime time) {
            DestinationRectangle = new Rectangle(
                ContainingMap.X + Position.X*ContainingMap.TileWidth,
                ContainingMap.Y + Position.Y*ContainingMap.TileHeight,
                ContainingMap.TileWidth,
                ContainingMap.TileHeight);
        }
    }
}