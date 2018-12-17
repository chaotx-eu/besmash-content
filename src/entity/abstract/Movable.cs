namespace BesmashContent {
    using System.Runtime.Serialization;
    using Microsoft.Xna.Framework;
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
    /// movement values and other is the MapObject this object
    /// would collide with. As return value a Point is expected
    /// representing the new movement values to be taken instead
    /// or null if nothing should happen.
    /// Note: Unfinished, may change in the future!
    public delegate Point? CollisionResolver(int distanceX, int distanceY, MapObject other);

    /// An object with the ability to move
    /// over a TileMap.
    public abstract class Movable : Entity {
        /// Time in millis this object needs to
        /// move the distance of one Tile.
        [DataMember]
        public long StepTime {get; set;} = 0;

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

        /// Stops any movement immediately.
        public virtual void stop() {
            Moving = false;
            onMoveFinished(new MoveEventArgs(
                Position.ToPoint(), Target));
        }

        public virtual void move(int distanceX, int distanceY) {
            // Default CollisionResolver => Solid Tiles and Entities are unpassable.
            // Has to be reimplemented if a different CollisionResolver is used.
            move(distanceX, distanceY, ((x, y, mo) => {
                if(mo is Entity || mo is Tile && ((Tile)mo).Solid)
                    return Point.Zero;
                    
                return null;
            }));
        }

        public virtual void move(int distanceX, int distanceY, CollisionResolver resolve) {
            if(!Moving && (distanceX != 0 || distanceY != 0)) {
                int positionX = (int)Position.X;
                int positionY = (int)Position.Y;
                Target = new Point(
                    positionX + distanceX,
                    positionY + distanceY);

                Tile tgtTile = ContainingMap.getTile(Target.X, Target.Y);
                Point? newDistance = resolve(distanceX, distanceY, tgtTile);
                if(newDistance == null) {
                    foreach(Entity e in ContainingMap.getEntities(Target.X, Target.Y))
                        if((newDistance = resolve(distanceX, distanceY, e)) != null) break;
                }

                if(newDistance != null) {
                    move(newDistance.Value.X, newDistance.Value.Y, resolve);
                    return;
                }

                Facing = distanceY > 0 ? Facing.SOUTH
                    : distanceY < 0 ? Facing.NORTH
                    : distanceX > 0 ? Facing.EAST
                    : distanceX < 0 ? Facing.WEST
                    : Facing;
                    
                Moving = true;
                onMoveStarted(new MoveEventArgs(
                    Position.ToPoint(), Target));
            }
        }
        
        float stepTimer, idleTimer;
        public override void update(GameTime time) {
            if(ContainingMap.Slave != this)
                base.update(time);

            if(Moving) {
                int et = time.ElapsedGameTime.Milliseconds;
                float positionX = Position.X;
                float positionY = Position.Y;
                idleTimer = 0;
                stepTimer += et;

                if(stepTimer*SpritesPerStep >= StepTime) {
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

            } else if(idleTimer < 300)
                idleTimer += time.ElapsedGameTime.Milliseconds;

            if(idleTimer >= 300)
                updateSprite(true);
        }

        private void updateSprite() {
            updateSprite(false);
        }

        private void updateSprite(bool reset) {
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

            // trigger tile stepped event
            Tile tile = ContainingMap.getTile(args.Target.X, args.Target.Y);
            tile.onTileStepped(new TileEventArgs(this, ContainingMap, args.Target));
        }
    }
}