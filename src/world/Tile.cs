namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Content;

    public class Tile : MapObject {
        public event TileSteppedHandler TileSteppedEvent;
        public event TileTriggeredHandler TileTriggeredEvent;

        /// Solid Tiles cannot be passed by
        /// Movable objects.
        [ContentSerializer(Optional = true)]
        public bool Solid {get; set;} = false;

        /// Tiles can be on different layers on the map.
        /// A fraction of this value is added to the absolute
        /// layer of this tile
        [ContentSerializer(Optional = true, ElementName = "Layer")]
        public float MapLayer {get; set;}

        /// The absolute layer of this tile including
        /// its map layer
        [ContentSerializerIgnore]
        public override float Layer {
            get {return MapLayer/8f;} // TODO
            set {base.Layer = value;}
        }

        /// A hidden tile will not be drawn but still
        /// updated by its containing map
        [ContentSerializerIgnore]
        public bool Hidden {get; set;}

        /// A disabled tile will neither be drawn or
        /// updated by its containinig map
        [ContentSerializerIgnore]
        public bool Disabled {get; set;}

        /// Triggers the tile triggered event with the
        /// passed movable entity as activator
        public void trigger(Movable activator) {
            onTileTriggered(new TileEventArgs(activator,
                ContainingMap, Position.ToPoint()));
        }

        public virtual void onTileStepped(TileEventArgs args) {
            TileSteppedHandler handler = TileSteppedEvent;
            if(handler != null) handler(this, args);
        }

        protected virtual void onTileTriggered(TileEventArgs args) {
            TileTriggeredHandler handler = TileTriggeredEvent;
            if(handler != null) handler(this, args);
        }
    }
}