namespace BesmashContent {
    using BesmashContent.Utility;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using System.Collections.Generic;
    using System;

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

        /// Checks wether there is line of sight from the current
        /// position to the desired spot on the containing map.
        public bool canSee(int x, int y) {
            if(ContainingMap == null) return false;
            int tx = (int)Position.X;
            int ty = (int)Position.Y;
            List<Point> ray = MapUtils.getRay(tx, ty, x, y);

            foreach(Point p in ray) {
                Tile tile = ContainingMap.getTile(p.X, p.Y);
                if(tile == null || tile.Solid) return false;
                // if(ContainingMap.getEntities(p.X, p.Y).Count > 0)
                //     return false;
            }

            return true;
        }
    }
}