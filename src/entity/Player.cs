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

            // TODO test
            BasicAttack = new BasicAttack();
            BasicAttack.User = this;

            // no collision with other players
            CollisionResolver = ((x, y, mos) => {
                foreach(MapObject mo in mos) {
                    if(mo is Tile && ((Tile)mo).Solid
                    || mo is Entity && !(mo is Player))
                        return Point.Zero;
                }

                return null;
            });
        }

        // TODO test projectile
        public override void load(Microsoft.Xna.Framework.Content.ContentManager content) {
            if(Abilities.Where(a => a.Title == "Fireball").Count() == 0)
                addAbility(content.Load<Ability>("objects/battle/abilities/fireball_ability"));
                
            base.load(content);
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