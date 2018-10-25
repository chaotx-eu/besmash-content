namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;

    /// Game objects which can be present on a map.
    public class MapObject : GameObject {
        [ContentSerializerIgnore]
        public TileMap ContainingMap {get; set;}

        // Needed to assign values to Position Vector
        // when deserializing a xml file.
        // The XmlSerializer seems to be somewhat
        // inconsistent regarding Vecotr types.
        // http://community.monogame.net/t/xmlserializer-wont-deserialize-vector2/10391
        [ContentSerializer(ElementName = "Position")]
        private Point position {
            get {return new Point((int)Position.X, (int)Position.Y);}
            set {Position = new Vector2(value.X, value.Y);}
        }
        
        /// Position on a TileMap.
        [ContentSerializerIgnore]
        public Vector2 Position {get; set;}

        public override void update(GameTime time) {
            DestinationRectangle = new Rectangle(
                ContainingMap.X + (int)(Position.X*ContainingMap.TileWidth),
                ContainingMap.Y + (int)(Position.Y*ContainingMap.TileHeight),
                ContainingMap.TileWidth,
                ContainingMap.TileHeight);
        }
    }
}