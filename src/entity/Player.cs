namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using System;

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
            SpritesPerStep = DEFAULT_SPS;
            SpritesPerSecond = DEFAULT_SPS;
            Facing = Facing.SOUTH;
            Name = randomName();
        }

        public static Random PlayerRNG = new Random();
        public static string[] SourceNames {get;} = {
            "Cecilia Gould",
            "Gladys Garcia",
            "Iwan Lawson",
            "Alayah Schroeder",
            "Connar Mccray",
            "Jun Conroy",
            "Shah Hardy",
            "Anaiya Fletcher",
            "Fariha Draper",
            "Kallum Carney",
            "Eleasha Rivera",
            "Ronald Akhtar",
            "Kush Cline",
            "Gloria Byrne",
            "Dominykas Moyer",
            "Richard Montgomery",
            "Arnas Carrillo",
            "Tiarna Braun",
            "Jonny Rivers",
            "Elizabeth Santiago",
            "Shauna Wagstaff",
            "Anas Mathis",
            "Belinda Bate",
            "Matylda Emerson",
            "Keeley Steadman",
            "Matteo Hester",
            "Tristan Blaese",
            "Lillie Nicholls",
            "Elowen Mcmahon",
        };

        public string randomName() {
            return SourceNames[PlayerRNG.Next(SourceNames.Length)];
        }
    }
}