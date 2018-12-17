namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using System.Linq;

    public class Dungeon1Map : TileMap {
        public Dungeon1Map() : base() {
            // nothing special required here
        }

        public override void init(Game game) {
            base.init(game);

            // define special tiles (doors, triggers, etc.)
            Tile cryptExit1 = getTile(49, 96);
            cryptExit1.TileSteppedEvent += (sender, args) => {
                // define here what should happen when stepped on this tile
                if(args.Movable == Slave)
                    loadOther("maps/forestMap"); // e.g. load another map (map file must exist!)
            };
        }

        public override void onLoad(TileMap fromMap, Team team) {
            if(fromMap is ForestMap) {
                team.Player.ForEach(player
                    => player.Position = new Vector2(49, 96));
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

            // position auf der map
            donald.Position = new Vector2(68, 60);
            dagobert.Position = new Vector2(43, 17);

            addEntity(donald);
            addEntity(dagobert);
        }
    }
}