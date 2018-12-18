namespace BesmashContent {
    using System.Runtime.Serialization;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Media;
    using Microsoft.Xna.Framework.Content;

    [KnownType(typeof(Player))]
    [KnownType(typeof(Enemy))]
    [KnownType(typeof(NeutralNPC))]
    [KnownType(typeof(EffectAnimation))]
    [KnownType(typeof(OffensiveAbility))]
    [KnownType(typeof(DeffensiveAbility))]
    [KnownType(typeof(MovementAbility))]
    [KnownType(typeof(HealAbility))]
    [KnownType(typeof(CureAbility))]
    [KnownType(typeof(BuffAbility))]
    [KnownType(typeof(Cursor))]
    [KnownType(typeof(NeutralNPC))]
    [KnownType(typeof(ForestMap))]
    [KnownType(typeof(Dungeon1Map))]
    [KnownType(typeof(Dungeon2Map))]
    [KnownType(typeof(Dungeon3Map))]
    [DataContract(IsReference = true)]
    public class TileMap {
        /// Possible states a map can be in
        public enum MapState {Roaming, Fighting}

        /// Alpha value for all maps and its content
        public static float MapAlpha {get; set;} = 1f;

        /// Title of this map.
        [DataMember]
        [ContentSerializer(Optional = true)]
        public string Title {get; set;} = "";

        /// Path to the file used for the played
        /// BackgroundMusic while the map is loaded
        [DataMember]
        [ContentSerializer(Optional = true)]
        public string BackgroundMusicFile {get; set;} // TODO

        /// Describes Viewports width and height
        /// in Tiles. The Viewport position is
        /// centered around the current Slave on
        /// this map. Reinitializes tile size
        /// and slave size/position when set.
        [DataMember]
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

        /// List of Tiles this map is made of.
        [DataMember]
        [ContentSerializer(CollectionItemName = "Tile")]
        public List<Tile> Tiles {get; set;}

        /// List of Entities on this map.
        [DataMember]
        [ContentSerializerIgnore]
        public List<Entity> Entities {get {
            return entities == null 
                ? (entities = new List<Entity>())
                : entities;
        }}

        /// Movable object on this map. Gets affected
        /// by user input (e.g. Controller/Keyboard).
        /// Will always be centered on the screen.
        [DataMember]
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

        /// The state of this map
        [DataMember]
        [ContentSerializerIgnore]
        public MapState State {get; private set;} = MapState.Roaming;

        /// Width of this map in tiles. Equal
        /// to the x-coord of any Tile furthest
        /// to the right plus 1.
        [ContentSerializerIgnore]
        public int Width {get {return width;}}

        /// Height of this map in tiles. Equal
        /// to the y-coord of any Tile furthest
        /// to the bottom plus 1.
        [ContentSerializerIgnore]
        public int Height {get {return height;}}

        /// Width in pixels of a single tile
        /// on this map.
        [ContentSerializerIgnore]
        public int TileWidth {get {return tileWidth;}}

        /// Height in pixels of a single tile
        /// on this map.
        [ContentSerializerIgnore]
        public int TileHeight {get {return tileHeight;}}

        /// X-coordinate of this map on
        /// the screen.
        [ContentSerializerIgnore]
        public int X {get {return x;}}

        /// Y-coordinate of this map on
        /// the screen.
        [ContentSerializerIgnore]
        public int Y {get {return y;}}
        
        /// Reference to the background song object
        [ContentSerializerIgnore]
        public Song BackgroundMusic {get; set;} // TODO

        /// Wether this map has already been initialized
        [ContentSerializerIgnore]
        public bool Initialized {get; private set;}

        /// The center of the viewport in fighting state
        [ContentSerializerIgnore]
        public Point BattleMapCenter {get; private set;}

        /// Map to be loaded on next update. Resets to
        /// null on retreival
        [ContentSerializerIgnore]
        public string OtherMap {
            get {
                string value = otherMap;
                otherMap = null;
                return value;
            }
            private set {otherMap = value;}
        }

        // Reference to the Cursor of this Map
        [ContentSerializerIgnore]
        public Cursor Cursor {get; private set;} = new Cursor();

        private string otherMap = null;
        private Point viewport = new Point(4, 4);
        private List<Entity> entities;
        private Tile[][] tileArray;
        private Movable slave;
        private Game game;

        private int tileWidth, tileHeight;
        private int width, height;
        private int x, y;

        public TileMap() {
            Cursor.ContainingMap = this;
        }

        /// This method is called after the map is loaded
        /// with the previous map and active team as parameter
        public virtual void onLoad(TileMap fromMap, Team team) {
            spawnEntities();
            team.Player.ForEach(player => {
                if(fromMap != null)
                    fromMap.removeEntity(player);
                    
                player.stop(); // while ContainingMap is null
                addEntity(player);
            });

            Slave = team.Leader;
            team.resetFormation();
        }

        /// Override this method to spawn and add
        /// entities on load
        protected virtual void spawnEntities() {
            Entities.Clear();
        }

        /// Initializes this map and all its objects.
        public virtual void init(Game game) {
            this.game = game;
            if(Tiles.Count > 0) {
                width = Tiles.Max(t => (int)t.Position.X) + 1;
                height = Tiles.Max(t => (int)t.Position.Y) + 1;
            }

            // Tiles are saved in an array for faster access
            tileArray = new Tile[width][];

            for(int x = 0; x < width; ++x)
                tileArray[x] = new Tile[height];

            Tiles.ForEach(t => tileArray
                [(int)t.Position.X][(int)t.Position.Y] = t);

            initTileSize();
            initSlave();
            Tiles.ForEach(t => t.ContainingMap = this);
            Initialized = true;
        }

        /// Defines another map to be loaded on next update
        public void loadOther(string mapFile) {
            OtherMap = mapFile;
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

        /// Computes and sets X and Y onscreen-coordinate
        /// dependent on the Viewport and the Slave position
        /// in roaming state or the the BattleMapCenter in
        /// Figthing state
        public void align() {
            float dx = 0, dy = 0;
            if(State == MapState.Roaming && Slave != null) {
                dx = Slave.Position.X;
                dy = Slave.Position.Y;
            } else if(State == MapState.Fighting) {
                dx = BattleMapCenter.X;
                dy = BattleMapCenter.Y;
            }

            x = Viewport.X*tileWidth - (int)(dx*tileWidth);
            y = Viewport.Y*tileHeight - (int)(dy*tileHeight);
        }

        /// Loads all for this map required assets
        /// into memory.
        public void load(ContentManager manager) {
            foreach(Tile tile in Tiles) tile.load(manager);
            foreach(Entity entity in Entities) entity.load(manager);

            // testing: background music (TODO)
            if(BackgroundMusicFile != null) try {
                BackgroundMusic = manager.Load<Song>(BackgroundMusicFile);
                // MediaPlayer.Play(BackgroundMusic);
            } catch(ContentLoadException) {
                // ignore
            }

            Cursor.load(manager);
        }

        /// Aligns the map and updates all game object on it.
        public void update(GameTime time) {
            align();
            foreach(Tile tile in Tiles) tile.update(time);
            for(int i = Entities.Count-1; i >= 0; --i) Entities[i].update(time);
            // foreach(Entity entity in Entities) entity.update(time);
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

        public Tile getTile(int x, int y) {
            if(x >= 0 && x < Width && y >= 0 && y < Height)
                return tileArray[x][y];

            return null;
        }

        public List<Entity> getEntities(int x, int y) {
            return Entities.Where(e =>
                e.Position.X == x &&
                e.Position.Y == y).ToList();
        }

        public void setFightingState(Team team) {
            setFightingState(team, team.Leader.Target);
        }

        public void setFightingState(Team team, Point center) {
            Cursor.Position = new Vector2(
                team.Leader.Target.X,
                team.Leader.Target.Y);

            BattleMapCenter = center;
            State = MapState.Fighting;
            Slave = Cursor;
            Cursor.stop();
        }

        public void setRoamingState(Team team) {
            if(Slave is Cursor)
                removeEntity(Slave);

            Slave = team.Leader;
            State = MapState.Roaming;
        }
    }
}
