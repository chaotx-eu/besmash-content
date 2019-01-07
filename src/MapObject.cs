namespace BesmashContent {
    using BesmashContent.Utility;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using System.Collections.Generic;
    using System;
    using System.Runtime.Serialization;

    /// Possible facings on a tile map
    [DataContract]
    public enum Facing {
        [EnumMember] North,
        [EnumMember] East,
        [EnumMember] South,
        [EnumMember] West
    }

    /// Game objects which can be present on a map.
    [DataContract(IsReference = true)]
    public class MapObject : GameObject {
        // Needed to assign values to Position Vector
        // when deserializing a xml file.
        // The XmlSerializer seems to be somewhat
        // inconsistent regarding Vector types.
        // http://community.monogame.net/t/xmlserializer-wont-deserialize-vector2/10391
        [ContentSerializer(ElementName = "Position", Optional = true)]
        private Point position {
            get {return new Point((int)Position.X, (int)Position.Y);}
            set {Position = new Vector2(value.X, value.Y);}
        }

        /// The direction this map object is facing
        [DataMember]
        [ContentSerializer(Optional = true)]
        public Facing Facing {get; set;}

        /// The map this object is contained by
        [DataMember]
        [ContentSerializerIgnore]
        public TileMap ContainingMap {get; set;}

        /// Position on a TileMap.
        [DataMember]
        [ContentSerializerIgnore]
        public Vector2 Position {get; set;}

        public override void update(GameTime gameTime) {
            base.update(gameTime);
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
                // if(ContainingMap.getEntities(p.X, p.Y).Count > 0)
                //     return false;
                
                foreach(Tile tile in ContainingMap.getTiles(p.X, p.Y))
                    if(tile == null || tile.Solid) return false;
            }

            return true;
        }
    }
}