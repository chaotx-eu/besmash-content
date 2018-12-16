namespace BesmashContent {
    using Microsoft.Xna.Framework;

    public class Player : Creature {
        /// default width in pixels of a single sprite
        /// in a creature spritesheet
        public static int DEFAULT_SPRITE_W {get;} = 16;

        /// default height in pixels of a single sprite
        /// in a creature spritesheet
        public static int DEFAULT_SPRITE_H {get;} = 16;

        /// default sprite count on horizontal pane
        public static int DEFAULT_SPRITE_C {get;} = 4;

        /// default amount of sprites shown per step
        public static int DEFAULT_SPS {get;} = 2;

        public Player() : this("") {}
        public Player(string spriteSheet) {
            SpriteSheet = spriteSheet;
            SpriteRectangle = new Rectangle(0, 0, DEFAULT_SPRITE_W, DEFAULT_SPRITE_H);
            SpriteCount = DEFAULT_SPRITE_C;
            SpritesPerStep = 2;
            Facing = Facing.SOUTH;
        }
    }
}