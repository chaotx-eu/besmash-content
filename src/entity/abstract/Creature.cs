namespace BesmashContent {
    using Microsoft.Xna.Framework;

    public class Creature : Movable {
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

        /// Creatures may wear armor
        public Helmet Helmet {get; set;}
        public Chestplate Chestplate {get; set;}
        public Pants Pants {get; set;}

        /// Creatures may hold a weapon
        public Weapon Weapon {get; set;}

        /// The name of this creature
        public string Name {get; protected set;}

        /// The stats of this creature
        public Stats Stats {get; protected set;}

        public Creature() : this("") {}
        public Creature(string spriteSheet) {
            SpriteSheet = spriteSheet;
            SpriteRectangle = new Rectangle(0, 0, DEFAULT_SPRITE_W, DEFAULT_SPRITE_H);
            SpriteCount = DEFAULT_SPRITE_C;
            SpritesPerStep = DEFAULT_SPS;
            SpritesPerSecond = DEFAULT_SPS;
        }
    }
}