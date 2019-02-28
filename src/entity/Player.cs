namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using System.Linq;
    using System;

    public class Player : Creature {
        public Player() : this("") {}
        public Player(string spriteSheet) : this(spriteSheet, randomName()) {}
        public Player(string spriteSheet, string name) : base(spriteSheet) {
            Name = name;
            SpritesPerSecond = 4;
        }

        // TODO (this is just for fun)
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

        public static string randomName() {
            return SourceNames[PlayerRNG.Next(SourceNames.Length)];
        }
    }
}