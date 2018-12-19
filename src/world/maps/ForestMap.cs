namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using System.Linq;

    public class ForestMap : TileMap {
        public ForestMap() : base() {
            // nothing special required here
        }

        public override void init(Game game) {
            base.init(game);

            // define special tiles (doors, triggers, etc.)
            Tile cryptDoor = getTile(45, 16);
            cryptDoor.TileSteppedEvent += (sender, args) => {
                // define here what should happen when stepped on this tile
                if(args.Movable == Slave)
                    loadOther("maps/dungeon1"); // e.g. load another map (map file must exist!)
            };
        }

        public override void onLoad(TileMap fromMap, Team team) {
            if(fromMap is Dungeon1Map) {
                team.Player.ForEach(player
                    => player.Position = new Vector2(45, 16));
            } else if(fromMap == null) { // new game (example)
                Player[] members = new Player[3];
                Player leader = new Player("images/entities/grey_sheet");

                string[] memberSheets = {
                    "images/entities/pink_sheet",
                    "images/entities/red_sheet",
                    "images/entities/white_sheet"
                };

                for(int i = -1; i < members.Length; ++i) {
                    Player player = i < 0 ? leader : new Player(memberSheets[i]);
                    player.Position = new Vector2(16, 86+i);
                    player.StepTime = 250;
                    if(i >= 0) members[i] = player;
                };
                
                leader.aggroRadius = 5;
                team.add(leader, members);
                team.Formation[members[0]] = new Point(1, 1);
                team.Formation[members[1]] = new Point(-1, 1);
                team.Formation[members[2]] = new Point(0, 1);
            }

            // always call this last
            base.onLoad(fromMap, team);
        }

        /// Override this method to spawn and and
        /// entities on load
        protected override void spawnEntities() {
            base.spawnEntities(); // clears entity list by default

            // some example npcs
            Entity donald = new NeutralNPC();
            Entity dagobert = new NeutralNPC();
            donald.SpriteSheet = "images/entities/kevin_sheet";
            dagobert.SpriteSheet = "images/entities/kevin_sheet";
            donald.SpriteRectangle = new Rectangle(0, 0, 16, 16);
            dagobert.SpriteRectangle = new Rectangle(0, 0, 16, 16);
            donald.Facing = Facing.WEST;
            dagobert.Facing = Facing.SOUTH;

            //Some Enemies for me to test the fightingSystem and stuff

            Group Enemies = new Group();
            //The Data of a Rat
            Stats RatStats = new Stats(7, 11, 5, 8, 6, 14, 1.0f, 1.0f, 4);
            //Abilities to choose from:
            OffensiveAbility poisonBite = new OffensiveAbility(null, 25, "Poisonous_Bite", 35, 1, false);
            poisonBite.potentialStatus = new OffensiveAbility.PossibleStatus[1];
            poisonBite.potentialStatus[0] = new OffensiveAbility.PossibleStatus(Status.Type.poison, 30);
            OffensiveAbility bite = new OffensiveAbility(null, 15, "Bite", 25, 1, false);
            MovementAbility run = new MovementAbility(null, 0, "Run", null, null);
            HealAbility endure = new HealAbility(null, 40, "Endure_Pain", 0, 50);
            Ability[] RatAbilities = {poisonBite, bite, run, endure};
            string RatSprite = "images/entities/enemies/Rat_Spritesheet";
            Rectangle RatRectangle = new Rectangle(0, 0, 16, 16);
            //End of the Data of a Rat

            //ShitTonOfRats......YEAY!
            Group EnemyGroup = new Group();
            Enemy Rat1 = new Enemy(RatStats, RatAbilities, NPC.FightingStyle.Meelee, 10, EnemyGroup);
            Rat1.SpriteSheet = RatSprite;
            Rat1.SpriteRectangle = RatRectangle;

            Enemy Rat2 = new Enemy(RatStats, RatAbilities, NPC.FightingStyle.Meelee, 10, EnemyGroup);
            Rat2.SpriteSheet = RatSprite;
            Rat2.SpriteRectangle = RatRectangle;

            Enemy Rat3 = new Enemy(RatStats, RatAbilities, NPC.FightingStyle.Meelee, 10, EnemyGroup);
            Rat3.SpriteSheet = RatSprite;
            Rat3.SpriteRectangle = RatRectangle;

            Enemy Rat4 = new Enemy(RatStats, RatAbilities, NPC.FightingStyle.Meelee, 10, EnemyGroup);
            Rat4.SpriteSheet = RatSprite;
            Rat4.SpriteRectangle = RatRectangle;

            // position auf der map
            Rat1.Position = new Vector2(20, 64);
            Rat2.Position = new Vector2(20, 67);
            Rat3.Position = new Vector2(22, 66);
            Rat4.Position = new Vector2(21, 65);

            addEntity(Rat1);
            addEntity(Rat2);
            addEntity(Rat3);
            addEntity(Rat4);

            donald.Position = new Vector2(68, 60);
            dagobert.Position = new Vector2(43, 17);

            addEntity(donald);
            addEntity(dagobert);
        }
    }
}