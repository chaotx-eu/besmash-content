namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;

    public class SpriteAnimation : MapObject {
        /// Total amount of sprites per row in this animation
        public int SpriteCount {get; set;}

        /// Size of a single sprite
        public Point SpriteSize {get; set;}

        /// How many sprites are shown per second
        public int SPS {get; set;}

        /// Row index in the sprite sheet
        [ContentSerializer(Optional = true)]
        public int SpriteRow {get; set;}

        /// Wether this animation is currently running
        [ContentSerializerIgnore]
        public bool IsRunning {get; protected set;}

        private int column, timer;

        /// Starts the animation on the given map
        public void start(TileMap map) {
            if(ContainingMap != map)
                map.addAnimation(this);
            else {
                SpriteRectangle = new Rectangle(0, SpriteRow*SpriteSize.Y, SpriteSize.X, SpriteSize.Y);
                column = timer = 0;
                IsRunning = true;
            }
        }

        public override void update(GameTime gameTime) {
            if(!IsRunning) return;
            base.update(gameTime);

            timer += gameTime.ElapsedGameTime.Milliseconds;
            if(timer > 1000f/SPS) {
                nextFrame();
                timer = 0;
            }
        }

        /// Sets the sprite rectangle to the next
        /// frame in the sprite sheet or disables
        /// this animation in case there is none
        protected virtual void nextFrame() {
            if(++column >= SpriteCount) {
                ContainingMap.Animations.Remove(this);
                ContainingMap = null;
                IsRunning = false;
                return;
            }

            SpriteRectangle = new Rectangle(
                SpriteSize.X*column, SpriteRow,
                SpriteSize.X, SpriteSize.Y
            );
        }
    }
}