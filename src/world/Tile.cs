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

        public virtual void onTileStepped(TileEventArgs args) {
            TileSteppedHandler handler = TileSteppedEvent;
            if(handler != null) handler(this, args);
        }

        public virtual void onTileTriggered(TileEventArgs args) {
            TileTriggeredHandler handler = TileTriggeredEvent;
            if(handler != null) handler(this, args);
        }
    }
}