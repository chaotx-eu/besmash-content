namespace BesmashContent {
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Content;

    public class TileMap {
        /// Title of this map.
        [ContentSerializer(Optional = true)]
        public string Title {get; set;} = "";

        /// Describes Viewport width and height
        /// in Tiles. The Viewport position is
        /// centered around the current Slave on
        /// this map. Reinitializes tile size
        /// and slave size/position when set.
        [ContentSerializer(Optional = true)]
        public Point Viewport {
            get {
                return viewport;
            }
            set {
                viewport = value;
                initTileSize();
                initSlave();
            }
        }

        private Point viewport = new Point(4, 4);

        /// List of Tiles this map is made of.
        [ContentSerializer(CollectionItemName = "Tile")]
        public List<Tile> Tiles {get; set;}

        /// List of Entities on this map.
        [ContentSerializerIgnore]
        public List<Entity> Entities {get;} = new List<Entity>();

        /// Width of this map in tiles. Equal
        /// to the x-coord of any Tile furthest
        /// to the right.
        [ContentSerializerIgnore]
        public int Width {get {return width;}}
        private int width = 0;

        /// Height of this map in tiles. Equal
        /// to the y-coord of any Tile furthest
        /// to the bottom.
        [ContentSerializerIgnore]
        public int Height {get {return height;}}
        private int height = 0;

        /// Width in pixels of a single tile
        /// on this map.
        [ContentSerializerIgnore]
        public int TileWidth {get {return tileWidth;}}
        private int tileWidth;

        /// Height in pixels of a single tile
        /// on this map.
        [ContentSerializerIgnore]
        public int TileHeight {get {return tileHeight;}}
        private int tileHeight;

        /// X-coordinate of this map on
        /// the screen.
        [ContentSerializerIgnore]
        public int X {get {return x;}}
        private int x;

        /// Y-coordinate of this map on
        /// the screen.
        [ContentSerializerIgnore]
        public int Y {get {return y;}}
        private int y;

        /// Movable object on this map. Gets affected
        /// by user input (e.g. Controller/Keyboard).
        /// The Viewport is centered around this object.
        [ContentSerializerIgnore]
        public Movable Slave {
            get { return slave;}
            set {
                if(value == null)
                    removeEntity(slave);
                else {
                    if(!Entities.Contains(value))
                        addEntity(value);
                }

                slave = value;
                initSlave();
            }
        }

        private Movable slave;

        /// Reference to running game.
        private Game game;

        /// Initializes this map and all its objects.
        public void init(Game game) {
            this.game = game;
            if(Tiles.Count > 0) {
                width = Tiles.Max(t => t.Position.X) + 1;
                height = Tiles.Max(t => t.Position.Y) + 1;
            }

            initTileSize();
            initSlave();
            Tiles.ForEach(t => t.ContainingMap = this);
        }

        /// Computes and sets the size in pixels of Tiles on
        /// this map based on the map- and screen-viewport.
        public void initTileSize() {
            if(game != null) {
                tileWidth = game.GraphicsDevice.Viewport.Width/(2*Viewport.X + 1);
                tileHeight = game.GraphicsDevice.Viewport.Height/(2*Viewport.Y + 1);
            }
        }

        /// Initializes slave position and size based on
        /// viewport and tile size (centered on screen).
        public void initSlave() {
            if(Slave != null) {
                Slave.DestinationRectangle = new Rectangle(
                    Viewport.X*tileWidth, Viewport.Y*TileHeight,
                    tileWidth, tileHeight
                );
            }
        }

        /// Computes and sets screen X and Y coordinate
        /// dependent on the Viewport and the Slave position.
        public void align() {
            if(Slave != null) {
                x = (Viewport.X - Slave.Position.X)*tileWidth;
                y = (Viewport.Y - Slave.Position.Y)*tileHeight;
            }
        }

        /// Loads all for this map required assets
        /// into memory.
        public void load(ContentManager manager) {
            foreach(Tile tile in Tiles) tile.load(manager);
            foreach(Entity entity in Entities) entity.load(manager);
        }

        /// Aligns the map and updates all game object on it.
        public void update(GameTime time) {
            foreach(Tile tile in Tiles) tile.update(time);
            foreach(Entity entity in Entities) entity.update(time);
            align();
        }

        /// Draws all game objects on this map.
        /// Closes the passed sprite batch when done.
        public void draw(SpriteBatch batch) {
            // https://gamedev.stackexchange.com/questions/6820/how-do-i-disable-texture-filtering-for-sprite-scaling-in-xna-4-0
            batch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);
            foreach(Tile tile in Tiles) tile.draw(batch);
            foreach(Entity entity in Entities) entity.draw(batch);
            batch.End();
        }

        public void addEntity(Entity e) {
            if(!Entities.Contains(e)) {
                e.ContainingMap = this;
                Entities.Add(e);
            }
        }

        public void removeEntity(Entity e) {
            if(Entities.Contains(e)) {
                e.ContainingMap = null;
                Entities.Remove(e);
                if(e == slave) Slave = null;
            }
        }
    }
}
