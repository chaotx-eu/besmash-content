namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using System.Collections.Generic;
    using System.Linq;


    public class Enemy : Npc, IBattleAI {
        /// The range at which this enemy will drag
        /// the active team into a battle
        [ContentSerializer(Optional = true)]
        public int AggroRange {get; set;} = 2; // TODO remove default value

        /// Updates this enemy, checks if there is
        /// a player within aggro range and starts
        /// a battle if this is the case
        public override void update(GameTime gameTime) {
            base.update(gameTime);
            if(ContainingMap.State == TileMap.MapState.Roaming
            && playerInRange()) ContainingMap.setFightingState();
        }

        /// Checks wether a player is within aggro range
        /// of this enemies position and returns true
        /// if this is the case false otherwise
        protected bool playerInRange() {
            for(int x, y = -AggroRange; y <= AggroRange; ++y)
            for(x = -AggroRange; x <= AggroRange; ++x) {
                if(ContainingMap.getEntities(
                    Position.ToPoint().X + x,
                    Position.ToPoint().Y + y)
                .Any(e => e is Player))
                    return true;
            }

            return false;
        }

        /// Moves towards the closest player in sight
        /// within the battle map (i.e. the maps viewport)
        /// (In case several are in the same range the
        /// one with the lowest health is targeted) TODO
        public Point? nextMove() {
            Pathfinder.Validator vl = pos => ContainingMap
                .getEntities(pos)
                .Where(e => e is Player)
                .Count() > 0;

            Point tl = ContainingMap.BattleMap.Position.ToPoint() - ContainingMap.Viewport;
            Point br = ContainingMap.BattleMap.Position.ToPoint() + ContainingMap.Viewport;
            // List<Point> path = getShortestPath(tl, br, vl);
            // return path.Count > 1 ? (Point?)path[0] : null;
            // TODO
            return Point.Zero;
        }

        /// Checks if there is an affordable ability
        /// that would hit a creature and returns it.
        /// In case several creatures were to be hit 
        /// the most expensive ability that hits the 
        /// creature with the lowest health is chosen
        public Ability nextAbility() {
            Ability ability = null;
            int minHP = -1;
            int maxAP = 0;

            Abilities.Where(a => a.APCost <= AP).ToList().ForEach(
                a => checkAbility(a).ForEach(c => {
                    if((minHP < 0 || minHP < c.HP)
                    || (minHP == HP && c.AP > maxAP)) {
                        ability = a;
                        maxAP = c.AP;
                    }
                }));

            return ability;
        }

        /// Helper to check wether any targets are in sight
        /// and on the spots this ability is executed on
        /// and returns them in a list
        private List<Creature> checkAbility(Ability ability) {
            List<Creature> targets = new List<Creature>();

            // TODO check all four facings
            ability.getTargetSpots()
                .Where(spot => canSee(Position.ToPoint() + spot))
                .ToList().ForEach(spot => ContainingMap
                    .getEntities(Position.ToPoint() + spot)
                    .Where(e => e is Player)
                    .Cast<Player>().ToList()
                    .ForEach(targets.Add));

            return targets;
        }
    }
}