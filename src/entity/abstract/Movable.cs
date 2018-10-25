namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using System;

    /// An object with the ability to move
    /// over a TileMap. Will always be centered
    /// on the screen.
    public abstract class Movable : Entity {
        /// Time in millis this object needs to
        /// move the distance of one Tile.
        public long StepTime {get; set;} = 0;

        private bool moving;
        private int tgtX, tgtY;
        private float posX, posY;

        public virtual void move(int distanceX, int distanceY) {
            if(!moving) {
                posX = Position.X;
                posY = Position.Y;
                tgtX = (int)posX + distanceX;
                tgtY = (int)posY + distanceY;
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