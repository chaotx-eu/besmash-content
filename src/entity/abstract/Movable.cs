namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using System;

    /// An object with the ability to move
    /// over a TileMap.
    public abstract class Movable : Entity {
        /// Time in millis this object needs to
        /// move the distance of one Tile.
        public long StepTime {get; set;} = 0;

        private bool moving;
        private int tgtX, tgtY;
        private float posX, posY;

        /// Defines how to react on collision with another MapObject.
        /// Where distanceX and distanceY are the originally planned
        /// movement values and other is the MapObject this object
        /// would collide with. As return value a Point is expected
        /// representing the new movement values to be taken instead
        /// or null if nothing should happen.
        /// Note: Unfinished, may change in the future!
        public delegate Point? CollisionResolver(int distanceX, int distanceY, MapObject other);

        public virtual void move(int distanceX, int distanceY) {
            // Default CollisionResolver => Solid Tiles are unpassable.
            // Has to be reimplemented if a different CollisionResolver is used.
            move(distanceX, distanceY, ((x, y, mo) => {
                if(mo != null && mo.GetType() == typeof(Tile))
                    if(((Tile)mo).Solid) return new Point(0, 0);
                    
                return null;
            }));
        }

        public virtual void move(int distanceX, int distanceY, CollisionResolver resolve) {
            if(!moving) {
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
                // if(newDistance != null) {
                //     tgtX = (int)posX + newDistance.Value.X;
                //     tgtY = (int)posY + newDistance.Value.Y;
                // }

                moving = true;
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
            }
        }
    }
}