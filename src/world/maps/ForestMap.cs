namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using System.Linq;

    public class ForestMap : TileMap {
        public ForestMap() : base() {
            // nothing special required here
        }

        public override void onLoad(TileMap fromMap, Team team) {
            if(team.Player.Count == 0) { // fromMap == null // new game (example)
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

                team.add(leader, members);
                team.Formation[members[0]] = new Point(1, 1);
                team.Formation[members[1]] = new Point(-1, 1);
                team.Formation[members[2]] = new Point(0, 1);
            } else if(!(fromMap is ForestMap)) { // i.e. if coming from another map
                // if(fromMap is Dungeon1Map) {
                //     team.Leader.Position = new Vector2(25, 35); // example
                //     ...
                // }
            }

            // always call this last
            base.onLoad(fromMap, team);
        }

        public override void init(Game game) {
            base.init(game);

            // some example npcs
            Entity donald = new NeutralNPC();
            Entity dagobert = new NeutralNPC();
            donald.SpriteSheet = "images/entities/kevin_sheet";
            dagobert.SpriteSheet = "images/entities/kevin_sheet";
            donald.SpriteRectangle = new Rectangle(0, 16, 16, 16);
            dagobert.SpriteRectangle = new Rectangle(0, 48, 16, 16);

            // position auf der map
            donald.Position = new Vector2(8, 8);
            dagobert.Position = new Vector2(32, 7);

            addEntity(donald);
            addEntity(dagobert);
        }
    }
}