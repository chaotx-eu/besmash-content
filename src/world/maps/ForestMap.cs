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
                    loadOther("objects/world/mapsdungeon1"); // e.g. load another map (map file must exist!)
            };
        }

        public override void onLoad(TileMap fromMap, Team team) {
            if(fromMap is Dungeon1Map) {
                team.Player.ForEach(player
                    => player.Position = new Vector2(45, 16));
            } else if(fromMap == null) { // new game (example)
                Player[] members = new Player[3];
                Player leader = new Player("images/wolrd/entities/player/grey_sheet");

                string[] memberSheets = {
                    "images/world/entities/player/pink_sheet",
                    "images/world/entities/player/red_sheet",
                    "images/world/entities/player/white_sheet"
                };

                for(int i = -1; i < members.Length; ++i) {
                    Player player = i < 0 ? leader : new Player(memberSheets[i]);
                    player.Position = new Vector2(16, 86+i);
                    player.StepTime = 250;
                    if(i >= 0) members[i] = player;
                };

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
            Entity donald = new Npc();
            Entity dagobert = new Npc();
            donald.SpriteSheet = "images/world/entities/player/kevin_sheet";
            dagobert.SpriteSheet = "images/world/entities/player/kevin_sheet";
            donald.SpriteRectangle = new Rectangle(0, 0, 16, 16);
            dagobert.SpriteRectangle = new Rectangle(0, 0, 16, 16);
            donald.Facing = Facing.West;
            dagobert.Facing = Facing.South;

            // position auf der map
            donald.Position = new Vector2(68, 60);
            dagobert.Position = new Vector2(43, 17);

            addEntity(donald);
            addEntity(dagobert);
        }
    }
}