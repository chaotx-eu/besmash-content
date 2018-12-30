namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using System.Runtime.Serialization;
    using System.Collections.Generic;
    using System.Linq;
    using System;

    /// Custom EventArgs for MoveEvents.
    public class MoveEventArgs : EventArgs {
        public Point Position {get;}
        public Point Target {get;}

        public MoveEventArgs(Point position, Point target) {
            Position = position;
            Target = target;
        }
    }

    /// Defines the signature of a MoveStartedEvent handler.
    public delegate void MoveStartedHandler(Movable sender, MoveEventArgs args);

    /// Defines the signature of a MoveFinishedEvent handler.
    public delegate void MoveFinishedHandler(Movable sender, MoveEventArgs args);

    /// Defines how to react on collision with another MapObject.
    /// Where distanceX and distanceY are the originally planned
    /// movement values and others is a list of MapObjects this
    /// object would collide with. As return value a Point is
    /// expected representing the new movement values to be taken
    /// instead or null if nothing should happen (i.e. no collision)
    public delegate Point? CollisionResolver(int distanceX, int distanceY, List<MapObject> others);

    /// An object with the ability to move
    /// over a TileMap.
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

        /// Time in millis this object needs to
        /// move the distance of one Tile. Will be
        /// multiplied with step time multiplier on
        /// retreival
        [DataMember]
        public int StepTime {
            get {return (int)(stepTime*StepTimeMultiplier);}
            set {stepTime = value;}
        }

        private int stepTime;

        /// StepTime is multiplied with this
        /// value on retreival
        [DataMember]
        public float StepTimeMultiplier {get; set;} = 1;

        /// Holds wether this Movable is currently moving.
        [DataMember]
        public bool Moving {get; set;}

        /// The amount of different sprites on the horizontal
        /// pane of the spritesheet. The vertical ammount will
        /// always be four (0:TOP, 1:EAST, 2:SOUTH, 3:WEST)
        [DataMember]
        public int SpriteCount {get; set;} = 1;

        /// How many sprites are shown when moving the distance
        /// of one tile
        [DataMember]
        public int SpritesPerStep {get; set;} = 1;

        /// Sprite update rate of this cursor
        [DataMember]
        public int SpritesPerSecond {get; set;} = 1;

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

        /// Target coordinate of the tile this objects
        /// moves to. Is automatically set on move.
        [DataMember]
        public Point Target {get; set;}

        /// Initializes a default collision resolver where solid
        /// tiles and entities are unpassable. Behaviour has to be
        /// reimplemented if a different CollisionResolver is used.
        public Movable() : this("") {}
        public Movable(string spriteSheet) {
            SpriteSheet = spriteSheet;
            SpriteRectangle = new Rectangle(0, 0, DEFAULT_SPRITE_W, DEFAULT_SPRITE_H);
            SpriteCount = DEFAULT_SPRITE_C;
            SpritesPerStep = DEFAULT_SPS;
            SpritesPerSecond = DEFAULT_SPS;
            CollisionResolver = (x, y, mos) => {
                foreach(MapObject mo in mos)
                    if(mo is Entity || mo is Tile && ((Tile)mo).Solid)
                        return Point.Zero;

                return null;
            };
        }

        /// Stops any movement immediately.
        public virtual void stop() {
            Moving = false;
            onMoveFinished(new MoveEventArgs(
                Position.ToPoint(), Target));
        }

        /// Moves this object distanceX tiles on the x-axis and
        /// distanceY tiles on the y-axis relative to its own
        /// position. The default collision resolver is used
        public virtual void move(int distanceX, int distanceY) {
            move(distanceX, distanceY, CollisionResolver);
        }

        /// Moves this object distanceX tiles on the x-axis and
        /// distanceY tiles on the y-axis relative to its own
        /// position. Collisions are resolved by the passed
        /// collision resolver.
        public virtual void move(int distanceX, int distanceY, CollisionResolver resolve) {
            if(!Moving && (distanceX != 0 || distanceY != 0)) {
                // TODO sprite is updated with delay
                //      when turning, not sure why
                Facing = distanceY > 0
                    ? Facing.SOUTH : distanceY < 0
                    ? Facing.NORTH : distanceX > 0
                    ? Facing.EAST : distanceX < 0
                    ? Facing.WEST : Facing;

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
                    return;
                }
                    
                Moving = true;
                onMoveStarted(new MoveEventArgs(
                    Position.ToPoint(), Target));
            }
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