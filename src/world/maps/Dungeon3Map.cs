namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using System.Linq;

    public class Dungeon3Map : TileMap {
        public Dungeon3Map() : base() {
            // nothing special required here
        }

        public override void init(Game game) {
            base.init(game);

            // define special tiles (doors, triggers, etc.)
            Tile stairsUp = getTile(49, 88);

            stairsUp.TileSteppedEvent += (sender, args) => {
                // define here what should happen when stepped on this tile
                if(args.Movable == Slave)
                    loadOther("maps/dungeon2"); // e.g. load another map (map file must exist!)
            };
            
        }

        public override void onLoad(TileMap fromMap, Team team) {
            if(fromMap is ForestMap) {
                team.Player.ForEach(player
                    => player.Position = new Vector2(49, 88));
            }

            // always call this last
            base.onLoad(fromMap, team);
        }

        /// Override this method to spawn and add
        /// entities on load
        protected override void spawnEntities() {
            base.spawnEntities(); // clears entity list by default

            // some example npcs
            Entity endBoss = new NeutralNPC();
            endBoss.SpriteSheet = "images/entities/kevin_sheet";
            endBoss.SpriteRectangle = new Rectangle(0, 0, 16, 16);
            endBoss.Facing = Facing.SOUTH;

            // position auf der map
            endBoss.Position = new Vector2(49, 32);


            addEntity(endBoss);

            
        }
    }
}