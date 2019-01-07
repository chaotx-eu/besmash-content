namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;

    public class Forest1Ext : TileMap {
        public override void init(Game game) {
            base.init(game);
            Tile closedDoor = getTile(27, 22, 1);
            Tile openDoor = getTile(27, 22, 2);

            openDoor.Disabled = true;
            closedDoor.TileTriggeredEvent += (sender, args) => {
                if(State == MapState.Roaming && args.Movable == Slave) {
                    closedDoor.Disabled = true;
                    openDoor.Disabled = false;
                }
            };

            openDoor.TileTriggeredEvent += (sender, args) => {
                if(State == MapState.Roaming && args.Movable == Slave) {
                    closedDoor.Disabled = false;
                    openDoor.Disabled = true;
                }
            };

            openDoor.TileSteppedEvent += (sender, args) => {
                if(State == MapState.Roaming && args.Movable == Slave)
                    loadOther("objects/world/maps/forest1_int");
            };
        }

        // public override void onLoad(TileMap fromMap, Team team) {
        //     if(fromMap is Forest1Int) {
        //         team.Player.ForEach(player => {
        //             player.Position = new Vector2(27, 22);
        //             player.Facing = Facing.South;
        //         });
        //     } else if(fromMap == null) { // new game
        //         Player[] members = new Player[3];
        //         Player leader = new Player("images/world/entities/player/grey_sheet");

        //         string[] memberSheets = {
        //             "images/world/entities/player/pink_sheet",
        //             "images/world/entities/player/red_sheet",
        //             "images/world/entities/player/white_sheet"
        //         };

        //         for(int i = -1; i < members.Length; ++i) {
        //             Player player = i < 0 ? leader : new Player(memberSheets[i]);
        //             player.Position = new Vector2(10-i, 38);
        //             player.Facing = Facing.East;
        //             player.StepTime = 250;
        //             if(i >= 0) members[i] = player;
        //         };

        //         team.add(leader, members);
        //         team.Formation[members[0]] = new Point(1, 1);
        //         team.Formation[members[1]] = new Point(-1, 1);
        //         team.Formation[members[2]] = new Point(0, 1);
        //     }

        //     base.onLoad(fromMap, team);
        // }
    }
}