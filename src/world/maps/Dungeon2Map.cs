namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using System.Linq;

    public class Dungeon2Map : TileMap {
        public Dungeon2Map() : base() {
            // nothing special required here
        }

        public override void init(Game game) {
            base.init(game);

            // define special tiles (doors, triggers, etc.)
            Tile stairsUp = getTile(75, 8);
            Tile stairsDown = getTile(61, 74);

            stairsUp.TileSteppedEvent += (sender, args) => {
                // define here what should happen when stepped on this tile
                if(args.Movable == Slave)
                    loadOther("objects/world/mapsdungeon1"); // e.g. load another map (map file must exist!)
            };
            stairsDown.TileSteppedEvent += (sender, args) => {
                if(args.Movable == Slave)
                    loadOther("objects/world/mapsdungeon3");
            };
        }

        public override void onLoad(TileMap fromMap, Team team) {
            if(fromMap is Dungeon1Map) {
                team.Player.ForEach(player
                    => player.Position = new Vector2(75, 8));
            }
            else if(fromMap is Dungeon3Map) {
                team.Player.ForEach(player
                    => player.Position = new Vector2(61, 74));
            }
            // always call this last
            base.onLoad(fromMap, team);
        }

        /// Override this method to spawn and add
        /// entities on load
        protected override void spawnEntities() {
            base.spawnEntities(); // clears entity list by default

            // some example npcs
            
        }
    }
}