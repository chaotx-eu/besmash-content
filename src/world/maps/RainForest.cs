namespace BesmashContent {
    using Microsoft.Xna.Framework;

    public class RainForest : TileMap {
        public override void onLoad(TileMap fromMap, Team team) {
            if(fromMap is Forest1Int) {
                team.Player.ForEach(player => {
                    player.Position = new Vector2(15, 16);
                    player.Facing = Facing.NORTH;
                });
            } else if(fromMap == null) { // new game
                Player[] members = new Player[3];
                Player leader = new Player("images/world/entities/player/grey_sheet");

                string[] memberSheets = {
                    "images/world/entities/player/pink_sheet",
                    "images/world/entities/player/red_sheet",
                    "images/world/entities/player/white_sheet"
                };

                for(int i = -1; i < members.Length; ++i) {
                    Player player = i < 0 ? leader : new Player(memberSheets[i]);
                    player.Position = new Vector2(15, 16+i);
                    player.Facing = Facing.NORTH;
                    player.StepTime = 250;
                    if(i >= 0) members[i] = player;
                };

                team.add(leader, members);
                team.Formation[members[0]] = new Point(1, 1);
                team.Formation[members[1]] = new Point(-1, 1);
                team.Formation[members[2]] = new Point(0, 1);
            }

            base.onLoad(fromMap, team);
        }
    }
}