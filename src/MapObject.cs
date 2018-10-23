namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;

    /// Game objects which can be present on a map.
    public class MapObject : GameObject {
        [ContentSerializerIgnore]
        public TileMap ContainingMap {get; set;}
        
        /// Position on a TileMap.
        public Point Position {get; set;}
    }
}