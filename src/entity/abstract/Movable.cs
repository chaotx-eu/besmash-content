namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using System.Runtime.Serialization;
    using System.Collections.Generic;
    using System.Linq;
    using System;

    /// An object with the ability to move
    /// over a TileMap.
    [DataContract]
    public class Movable : Entity {
        /// Default width in pixels of a single sprite
        /// in a creature spritesheet
        public static int DEFAULT_SPRITE_W {get;} = 16;

        /// Default height in pixels of a single sprite
        /// in a creature spritesheet
        public static int DEFAULT_SPRITE_H {get;} = 16;

        /// Default sprite count on horizontal pane
        public static int DEFAULT_SPRITE_C {get;} = 4;

        /// Default amount of sprites shown per step
        public static int DEFAULT_SPS {get;} = 2;

        /// The amount of different sprites on the horizontal
        /// pane of the spritesheet. The vertical ammount will
        /// always be four (0:North, 1:East, 2:South, 3:West)
        [DataMember]
        [ContentSerializer(Optional = true)]
        public int SpriteCount {get; set;} = 1;

        /// How many sprites are shown when moving the distance
        /// of one tile
        [DataMember]
        [ContentSerializer(Optional = true)]
        public int SpritesPerStep {get; set;} = 1;

        /// Sprite update rate of this movable while
        /// not moving
        [DataMember]
        [ContentSerializer(Optional = true)]
        public int SpritesPerSecond {get; set;} = 1;

        /// Time in millis this object needs to
        /// move the distance of one Tile. Will be
        /// multiplied with step time multiplier on
        /// retreival
        [DataMember]
        [ContentSerializer(Optional = true)]
        public int StepTime {
            get {return (int)(stepTime*StepTimeMultiplier);}
            set {stepTime = value;}
        }

        private int stepTime;

        /// StepTime is multiplied with this
        /// value on retreival
        [DataMember]
        [ContentSerializerIgnore]
        public float StepTimeMultiplier {get; set;} = 1;

        /// Holds wether this Movable is currently moving.
        [DataMember]
        [ContentSerializerIgnore]
        public bool Moving {get; set;}

        /// Target coordinate of the tile this objects
        /// moves to. Is automatically set on move.
        [DataMember]
        [ContentSerializerIgnore]
        public Point Target {get; set;}

        /// The default collision resolver which is used
        /// in case none is passed to move
        [ContentSerializerIgnore]
        public CollisionResolver CollisionResolver {get; protected set;}

        /// Event handler for handling move events i.e.
        /// will be triggered after this Movable started moving.
        public event MoveStartedHandler MoveStartedEvent;

        /// Event handler for handling move events i.e.
        /// will be triggered after this Movable finished moving.
        public event MoveFinishedHandler MoveFinishedEvent;

        /// Initializes a default collision resolver where solid
        /// tiles and entities are unpassable. Behaviour has to be
        /// reimplemented if a different CollisionResolver is used.
        public Movable() : this("") {}
        public Movable(string spriteSheet) {
            SpriteSheet = spriteSheet;
            SpriteRectangle = new Rectangle(0, 0, DEFAULT_SPRITE_W, DEFAULT_SPRITE_H);
            SpriteCount = DEFAULT_SPRITE_C;
            SpritesPerStep = DEFAULT_SPS;
            initHandler();
        }

        /// Stops any movement immediately.
        public virtual void stop() {
            Moving = false;
            onMoveFinished(new MoveEventArgs(
                Position.ToPoint(), Target));
        }

        /// Moves one tile towards the facing of this movable
        public bool move() {
            int x = Facing == Facing.East ? 1
                : Facing == Facing.West ? -1 : 0;

            int y = Facing == Facing.South ? 1
                : Facing == Facing.North ? -1 : 0;

            return move(x, y);
        }

        /// Overload for convenience, moves X tiles on the
        /// x-axis and Y tiles on the y-axis
        public bool move(Point vector) {
            return move(vector.X, vector.Y);
        }

        /// Moves this object distanceX tiles on the x-axis and
        /// distanceY tiles on the y-axis relative to its own
        /// position. The default collision resolver is used
        public virtual bool move(int distanceX, int distanceY) {
            return move(distanceX, distanceY, CollisionResolver);
        }

        /// Moves this object distanceX tiles on the x-axis and
        /// distanceY tiles on the y-axis relative to its own
        /// position. Collisions are resolved by the passed
        /// collision resolver.
        public virtual bool move(int distanceX, int distanceY, CollisionResolver resolve) {
            if(!Moving && (distanceX != 0 || distanceY != 0)) {
                // TODO sprite is updated with delay
                //      when turning, not sure why
                Facing = distanceY > 0
                    ? Facing.South : distanceY < 0
                    ? Facing.North : distanceX > 0
                    ? Facing.East : distanceX < 0
                    ? Facing.West : Facing;

                int positionX = (int)Position.X;
                int positionY = (int)Position.Y;
                Target = new Point(
                    positionX + distanceX,
                    positionY + distanceY);

                List<MapObject> targets = ContainingMap
                    .getTiles(Target.X, Target.Y).Cast<MapObject>()
                    .Concat(ContainingMap.getEntities(Target.X, Target.Y)).ToList();

                Point? newDistance = resolve(distanceX, distanceY, targets);
                if(newDistance != null) {
                    move(newDistance.Value.X, newDistance.Value.Y, resolve);
                    return false;
                }
                    
                Moving = true;
                onMoveStarted(new MoveEventArgs(
                    Position.ToPoint(), Target));

                return true;
            }

            return false;
        }

        /// Moves this object to the absolute target coordinate
        public bool moveTo(Point pos) {
            return moveTo(pos.X, pos.Y);
        }

        /// Moves this object to the absolute target coordinate
        public bool moveTo(int x, int y) {
            return moveTo(x, y, CollisionResolver);
        }

        /// Moves this object to the absolute target coordinate
        public bool moveTo(int x, int y, CollisionResolver resolve) {
            return move(x - (int)Position.X, y - (int)Position.Y, resolve);
        }
        
        float stepTimer, idleTimer;
        public override void update(GameTime gameTime) {
            if(ContainingMap.Slave != this || (ContainingMap.Slave is Cursor))
                base.update(gameTime);

            if(Moving) {
                int et = gameTime.ElapsedGameTime.Milliseconds;
                float positionX = Position.X;
                float positionY = Position.Y;
                idleTimer = 0;
                stepTimer += et;

                if(stepTimer*SpritesPerStep > StepTime) {
                    updateSprite();
                    stepTimer = 0;
                }

                if(StepTime > 0) {
                    float fragment = et/(float)StepTime;

                    if(positionX != Target.X) {
                        if(Target.X < positionX)
                            positionX = Math.Max(positionX-fragment, Target.X);

                        if(Target.X > positionX)
                            positionX = Math.Min(positionX+fragment, Target.X);
                    }

                    if(positionY != Target.Y) {
                        if(Target.Y < positionY)
                            positionY = Math.Max(positionY-fragment, Target.Y);

                        if(Target.Y > positionY)
                            positionY = Math.Min(positionY+fragment, Target.Y);
                    }

                    Moving = positionX != Target.X || positionY != Target.Y;
                } else {
                    positionX = Target.X;
                    positionY = Target.Y;
                    Moving = false;
                }

                Position = new Vector2(positionX, positionY);
                if(!Moving) onMoveFinished(new MoveEventArgs(
                    Position.ToPoint(), Target));

            } else {
                idleTimer += gameTime.ElapsedGameTime.Milliseconds;

                if(idleTimer > 1000f/SpritesPerSecond) {
                    updateSprite(true);
                    idleTimer = 0;
                }
            }
        }

        /// Initializes event handler and similiar
        protected virtual void initHandler() {
            MoveStartedEvent = null;
            MoveFinishedEvent = null;
            CollisionResolver = (x, y, mos) => {
                foreach(MapObject mo in mos)
                    if(mo is Entity || mo is Tile && ((Tile)mo).Solid)
                        return Point.Zero;

                return null;
            };
        }

        private void updateSprite() {
            updateSprite(false);
        }

        protected virtual void updateSprite(bool reset) {
            int w = SpriteRectangle.Width;
            int h = SpriteRectangle.Height;
            int y = ((int)Facing)*SpriteRectangle.Height;
            int x = reset ? 0 : ((SpriteRectangle.X + w) % (SpriteCount*w));
            SpriteRectangle = new Rectangle(x, y, w, h);
        }

        protected virtual void onMoveStarted(MoveEventArgs args) {
            MoveStartedHandler handler = MoveStartedEvent;
            if(handler != null) handler(this, args);
        }

        protected virtual void onMoveFinished(MoveEventArgs args) {
            MoveFinishedHandler handler = MoveFinishedEvent;
            if(handler != null) handler(this, args);

            // trigger tile stepped event(s)
            if(ContainingMap != null) {
                ContainingMap.getTiles(args.Target.X, args.Target.Y).ForEach(tile => {
                    tile.onTileStepped(new TileEventArgs(
                        this, ContainingMap, args.Target));
                });
            }
        }
    }
}