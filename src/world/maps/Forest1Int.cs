namespace BesmashContent {
    using Microsoft.Xna.Framework;

    public class Forest1Int : TileMap {
        public override void init(Game game) {
            base.init(game);
            getTile(11, 15).TileSteppedEvent += (sender, args) => {
                if(State == MapState.Roaming && args.Movable == Slave)
                    loadOther("objects/world/maps/forest1_ext");
            };
        }

        public override void onLoad(TileMap fromMap, Team team) {
            if(fromMap is Forest1Ext) {
                team.Player.ForEach(player => {
                    player.Position = new Vector2(11, 15);
                    player.Facing = Facing.NORTH;
                });
            }

            base.onLoad(fromMap, team);
        }
    }
}