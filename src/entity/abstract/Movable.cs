namespace BesmashContent {
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
        public long StepTime {get; set;} = 0;

        /// Holds wether this Movable is currently moving.
        public bool Moving {get { return moving;}}

        /// Event handler for handling move events i.e.
        /// will be triggered after this Movable started moving.
        public event MoveStartedHandler MoveStartedEvent;

        /// Event handler for handling move events i.e.
        /// will be triggered after this Movable finished moving.
        public event MoveFinishedHandler MoveFinishedEvent;

        private bool moving;
        private int tgtX, tgtY;
        private float posX, posY;

        /// Stops any movement immediately.
        public virtual void stop() {
            moving = false;
            onMoveFinished(new MoveEventArgs(
                new Point((int)posX, (int)posY),
                new Point(tgtX, tgtY)));
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
            if(!moving && (distanceX != 0 || distanceY != 0)) {
                posX = Position.X;
                posY = Position.Y;
                tgtX = (int)posX + distanceX;
                tgtY = (int)posY + distanceY;

                Tile tgtTile = ContainingMap.getTile(tgtX, tgtY);
                Point? newDistance = resolve(distanceX, distanceY, tgtTile);
                if(newDistance == null) {
                    foreach(Entity e in ContainingMap.getEntities(tgtX, tgtY))
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
                    
                moving = true;
                onMoveStarted(new MoveEventArgs(
                    new Point((int)posX, (int)posY),
                    new Point(tgtX, tgtY)));
            }
        }
        
        public override void update(GameTime time) {
            if(ContainingMap.Slave != this)
                base.update(time);

            if(moving) {
                int et = time.ElapsedGameTime.Milliseconds;

                if(StepTime > 0) {
                    float fragment = et/(float)StepTime;

                    if(posX != tgtX) {
                        if(tgtX < posX)
                            posX = Math.Max(posX-fragment, tgtX);

                        if(tgtX > posX)
                            posX = Math.Min(posX+fragment, tgtX);
                    }

                    if(posY != tgtY) {
                        if(tgtY < posY)
                            posY = Math.Max(posY-fragment, tgtY);

                        if(tgtY > posY)
                            posY = Math.Min(posY+fragment, tgtY);
                    }

                    moving = posX != tgtX || posY != tgtY;
                } else {
                    posX = tgtX;
                    posY = tgtY;
                    moving = false;
                }

                Position = new Vector2(posX, posY);
                if(!moving) onMoveFinished(new MoveEventArgs(
                    new Point(tgtX, tgtY),
                    new Point(tgtX, tgtY)));
            }
        }

        protected virtual void onMoveStarted(MoveEventArgs args) {
            MoveStartedHandler handler = MoveStartedEvent;
            if(handler != null) handler(this, args);
        }

        protected virtual void onMoveFinished(MoveEventArgs args) {
            MoveFinishedHandler handler = MoveFinishedEvent;
            if(handler != null) handler(this, args);
        }
    }
}