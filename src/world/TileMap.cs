namespace BesmashContent {
    using System.Runtime.Serialization;
    using System.Collections.Generic;
    using System.Linq;
    using System;

    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Media;
    using Microsoft.Xna.Framework.Content;

    [KnownType(typeof(Player))]
    [KnownType(typeof(Enemy))]
    [KnownType(typeof(Cursor))]
    [KnownType(typeof(Projectile))]
    [KnownType(typeof(ForestMap))]
    [KnownType(typeof(Dungeon1Map))]
    [KnownType(typeof(Dungeon2Map))]
    [KnownType(typeof(Dungeon3Map))]
    [KnownType(typeof(Forest1Ext))]
    [KnownType(typeof(Forest1Int))]
    [KnownType(typeof(RainForest))]
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

        /// The amount of npcs that are allowd to spawn
        /// on this map where the value will be randomly
        /// chosen between X and Y (both inclusive)
        [DataMember]
        [ContentSerializer(Optional = true)]
        public Point SpawnCount {get; set;}

        /// List of spawn points which determine what
        /// npcs where spawn when this map is loaded
        [DataMember]
        [ContentSerializer(CollectionItemName = "SpawnPoint", Optional = true)]
        public List<SpawnPoint> SpawnPoints {get; set;}

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

        /// TODO test serialization!
        /// List of sprite animations on this map
        [DataMember]
        [ContentSerializerIgnore]
        public List<SpriteAnimation> Animations {get {
            return animations == null
                ? (animations = new List<SpriteAnimation>())
                : animations;
        }}
        private List<SpriteAnimation> animations;

        /// Movable object on this map. Gets affected
        /// by user input (e.g. Controller/Keyboard).
        /// Will always be centered on the screen.
        [DataMember]
        [ContentSerializerIgnore]
        public Movable Slave {
            get { return slave;}
            set {
                if(value != null && !Entities.Contains(value))
                    addEntity(value);

                slave = value;
                initSlave();
            }
        }

        /// The state of this map
        [DataMember]
        [ContentSerializerIgnore]
        public MapState State {get; private set;}

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
        // deprecated
        // [ContentSerializerIgnore]
        // public Point BattleMapCenter {get; private set;}

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

        /// Reference to the Cursor of this Map
        [ContentSerializerIgnore]
        public Cursor Cursor {get; private set;} = new Cursor();

        /// Content manager of this map
        [ContentSerializerIgnore]
        public ContentManager Content {get; protected set;}

        /// Random number generator of this map
        [ContentSerializerIgnore]
        public Random RNG {get; protected set;}

        /// The battle map of this tile map
        [ContentSerializerIgnore]
        public BattleMap BattleMap {get;}

        private string otherMap = null;
        private Point viewport = new Point(4, 4);
        private List<Entity> entities;
        private Dictionary<string, Tile> tileDict;
        private Movable slave;
        private Game game;

        private int minLayer, maxLayer;
        private int tileWidth, tileHeight;
        private int width, height;
        private int x, y;

        public TileMap() {
            RNG = new Random();
            Cursor.ContainingMap = this;
            BattleMap = new BattleMap(this);
        }

        /// This method is called after the map is loaded
        /// with the previous map and active team as parameter
        public virtual void onLoad(TileMap fromMap, Team team) {
            // spawnEntities(); // TODO onLoad is deprecated  
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
        /// entities on load (TODO does not need be virtual)
        public virtual void spawnEntities() {
            // Entities.Clear(); // hm (TODO does not really belong here)
            if(SpawnPoints == null) return;

            // TODO (new spawning system)
            List<SpawnPoint> spawnPoints;
            int count = RNG.Next(SpawnCount.X, SpawnCount.Y+1);
            int s = 0, i;

            // spawns with negative weight in prioritized order
            spawnPoints = SpawnPoints
                .Where(sp => sp.Weight < 0).ToList();

            spawnPoints.OrderByDescending(sp => sp.Weight).ToList().ForEach(p => {
                if(s >= count) return;
                if(spawnAt(p)) ++s;
            });

            // spawns with positive weight in no particular order (but weighted)
            spawnPoints.Clear();
            SpawnPoints.Where(sp => sp.Weight > 0).ToList().ForEach(sp => {
                for(i = 0; i < sp.Weight; ++i)
                    spawnPoints.Add(sp);
            });

            SpawnPoint point;
            for(; s < count && spawnPoints.Count > 0; ++s) {
                point = spawnPoints[RNG.Next(spawnPoints.Count)];                
                spawnPoints.RemoveAll(p => p.Equals(point));
                if(spawnAt(point)) ++s;
            }
        }

        /// Helper for choose, load and place a spawning entity
        /// on the map. Returns true on success false otherwise
        protected bool spawnAt(SpawnPoint point) {
            List<Spawn> spawns = new List<Spawn>();

            int i;
            point.Spawns.ForEach(spawn => {
                for(i = 0; i < spawn.Weight; ++i)
                    spawns.Add(spawn);
            });

            if(spawns.Count > 0) {
                Spawn spawn = spawns[RNG.Next(spawns.Count)];
                Creature e = Content.Load<Npc>(spawn.NPC).clone() as Creature;
                e.load(Content);

                int lvl = RNG.Next(spawn.Level.X, spawn.Level.Y+1);
                for(i = 0; i < lvl; ++i) {
                    e.Exp = e.MaxExp;
                    e.levelUp(1);
                }

                if(e is Npc) (e as Npc).SpawnPosition = point.Position;
                e.Position = point.Position.ToVector2();
                e.AP = e.MaxAP;
                addEntity(e);
                return true;
            }

            return false;
        }

        /// Initializes this map and all its objects.
        public virtual void init(Game game) {
            this.game = game;
            if(Tiles.Count > 0) {
                width = Tiles.Max(t => (int)t.Position.X) + 1;
                height = Tiles.Max(t => (int)t.Position.Y) + 1;
            }

            // Tiles are saved in a dictionary for faster access
            tileDict = new Dictionary<string, Tile>();
            minLayer = maxLayer = -1;

            Tiles.ForEach(t => {
                string index = t.Position.X + "|" + t.Position.Y + "|" + t.MapLayer;
                if(minLayer < 0 || t.MapLayer < minLayer) minLayer = (int)t.MapLayer;
                if(maxLayer < 0 || t.MapLayer > maxLayer) maxLayer = (int)t.MapLayer;
                tileDict.Add(index, t);
            });

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
            // if(Slave != null) {
                dx = Slave.Position.X;
                dy = Slave.Position.Y;
            } else {// if(State == MapState.Fighting) {
                dx = BattleMap.Position.X;
                dy = BattleMap.Position.Y;
            }

            x = Viewport.X*tileWidth - (int)(dx*tileWidth);
            y = Viewport.Y*tileHeight - (int)(dy*tileHeight);
        }

        /// Loads all required resources for this
        /// map and any entities on it
        public void load() {
            if(Content == null && game != null)
                Content = new ContentManager(game.Services, "Content");

            foreach(Tile tile in Tiles) tile.load(Content);
            foreach(Entity entity in Entities) entity.load(Content);

            // testing: background music (TODO)
            // if(BackgroundMusicFile != null) try {
            //     BackgroundMusic = Content.Load<Song>(BackgroundMusicFile);
            //     // MediaPlayer.Play(BackgroundMusic);
            // }

            Cursor.load(Content);
        }

        /// Unloads and disposes all resources this
        /// map and any entities on it require
        public void unload() {
            if(Content != null) {
                Content.Dispose(); // also calls Unload
                Content = null; // let garbage collection do the rest
            }
        }

        /// Aligns the map and updates all game object on it.
        public void update(GameTime gameTime) {
            align();
            int max = Math.Max(Math.Max(Tiles.Count, Entities.Count), Animations.Count);

            for(int i = 0; i < max; ++i) {
                if(i < Tiles.Count) {
                    Tile tile = Tiles[i];
                    if(!tile.Disabled) tile.update(gameTime);
                }

                if(i < Entities.Count) Entities[i].update(gameTime);
                if(i < Animations.Count) Animations[i].update(gameTime);
            }

            // check if battle is still ongoing
            if(State == MapState.Fighting
            && BattleMap.Participants.Where(c => c is Enemy).Count() == 0)
                setRoamingState();

            // Once a battle has finished the battle map moves
            // towards the slave Player and once reached removes
            // itself from the map and resets the slave
            if(State == MapState.Roaming
            && Slave == null && slavePlayer != null
            && BattleMap.Position == slavePlayer.Position) {
                Slave = slavePlayer;
                removeEntity(BattleMap);
            }
        }

        /// Draws all game objects on this map.
        /// Closes the passed sprite batch when done.
        public void draw(SpriteBatch batch) {
            // https://gamedev.stackexchange.com/questions/6820/how-do-i-disable-texture-filtering-for-sprite-scaling-in-xna-4-0
            batch.Begin(SpriteSortMode.FrontToBack, null, SamplerState.PointClamp, null, null, null, null);
            int max = Math.Max(Math.Max(Tiles.Count, Entities.Count), Animations.Count);

            for(int i = 0; i < max; ++i) {
                if(i < Tiles.Count) {
                    Tile tile = Tiles[i];

                    if(!tile.Disabled && !tile.Hidden)
                        tile.draw(batch);
                }

                if(i < Entities.Count) Entities[i].draw(batch);
                if(i < Animations.Count) Animations[i].draw(batch);
            }
        
            batch.End();
        }

        /// Adds a clone of the passed animation to
        /// the map and starts it
        public void addAnimation(SpriteAnimation a) {
            addAnimation(a, false);
        }

        /// Adds an animition to this map and starts it.
        /// If reference is set to true the passed reference
        /// will be added otherwise a clone of it
        public void addAnimation(SpriteAnimation a, bool reference) {
            SpriteAnimation animation = reference ? a
                : a.clone() as SpriteAnimation;

            if(!Animations.Contains(animation))
                Animations.Add(animation);

            animation.ContainingMap = this;
            animation.start();
        }

        public void addEntity(Entity e) {
            if(!Entities.Contains(e)) {
                e.ContainingMap = this;
                e.Layer = 4/8f; // TODO EntityLayer/MAX_LAYER
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

        public Tile getTile(Point pos) {
            return getTile(pos.X, pos.Y);
        }

        public Tile getTile(int x, int y) {
            return getTile(x, y, false);
        }

        public Tile getTile(Point pos, bool top) {
            return getTile(pos.X, pos.Y, top);
        }

        public Tile getTile(int x, int y, bool top) {
            return getTile(x, y, top ? maxLayer : minLayer);
        }

        public Tile getTile(Point pos, int z) {
            return getTile(pos.X, pos.Y, z);
        }

        public Tile getTile(int x, int y, int z) {
            return tileDict[x+"|"+y+"|"+z];
        }

        public List<Tile> getTiles(Point pos) {
            return getTiles(pos.X, pos.Y);
        }

        public List<Tile> getTiles(int x, int y) {
            return tileDict.Values.Where(t =>
                t.Position.X == x && !t.Disabled &&
                t.Position.Y == y).ToList();
        }

        public List<Entity> getEntities(Point pos) {
            return getEntities(pos.X, pos.Y);
        }

        public List<Entity> getEntities(int x, int y) {
            return Entities.Where(e =>
                e.Position.X == x &&
                e.Position.Y == y).ToList();
        }

        /// Shows and enables the cursor
        private Movable slaveBuffer;
        public void showCursor() {
            Cursor.Position = BattleMap.Position; // TODO (show at TeamLeader/last position)
            Cursor.stop();
            slaveBuffer = Slave;
            Slave = Cursor;
        }

        /// Hides and disables the cursor
        public void hideCursor() {
            if(Slave is Cursor) {
                removeEntity(Cursor);
                Slave = slaveBuffer;
            }
        }

        public void setFightingState() {
            Point center = Point.Zero;
            BattleMap.Participants.Clear();
            BattleMap.Participants.AddRange(Entities.Where(e => (e is Creature)
                && e.Position.X >= Slave.Position.X - Viewport.X
                && e.Position.X <= Slave.Position.X + Viewport.X
                && e.Position.Y >= Slave.Position.Y - Viewport.Y
                && e.Position.Y <= Slave.Position.Y + Viewport.Y).Cast<Creature>());
                
            BattleMap.Participants.ForEach(e => center += e.Position.ToPoint());
            center /= new Point(BattleMap.Participants.Count, BattleMap.Participants.Count);
            setFightingState(center);
        }

        private Movable slavePlayer; // backup slave player
        public void setFightingState(Point center) {
            if(State != MapState.Fighting) {
                BattleMap.stop();
                BattleMap.Position = Slave.Position;
                State = MapState.Fighting;
                slavePlayer = Slave;
                Slave = null;
                addEntity(BattleMap); // TODO sloppy solution (?)
                BattleMap.moveTo(center);
            }
        }

        public void setRoamingState() {
            if(State != MapState.Roaming) {
                hideCursor();
                State = MapState.Roaming;
                BattleMap.moveTo(slavePlayer.Position.ToPoint());
            }
        }
    }
}
