namespace BesmashContent {
    using Utility;
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
            if(ContainingMap != null && ContainingMap.State == TileMap.MapState.Roaming
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

        private bool newPath;
        private int maxSteps = 3;
        private int step = -1;

        /// Moves towards the closest player in sight
        /// within the battle map (i.e. the maps viewport)
        /// (In case several are in the same range the
        /// one with the lowest health is targeted) TODO
        public Point? nextMove() {return nextMove(false);}
        public Point? nextMove(bool newPath) {
            if(AP < MoveAP) return Point.Zero;

            if(newPath) {
                step = 0;
                Pathfinder.Path.Clear();
                
                Pathfinder.Validator vl = pos => {
                    bool playerAdjacent = false;
                    Point p = new Point(0, -1);
                    for(int s = 0; s < 4; ++s) {
                        if(ContainingMap.getEntities(pos + MapUtils.rotatePoint(p, (Facing)s))
                        .Where(e => e is Player).Count() > 0) {
                            playerAdjacent = true;
                            break;
                        }
                    }

                    return playerAdjacent && ContainingMap
                        .getTiles(pos).Where(t => t.Solid)
                        .Count() == 0;
                };

                Point tl = ContainingMap.BattleMap.Position.ToPoint() - ContainingMap.Viewport;
                Point br = ContainingMap.BattleMap.Position.ToPoint() + ContainingMap.Viewport;
                Pathfinder.getShortestPath(tl, br, vl);
                return null;
            }

            if(!Pathfinder.IsAtWork){
                if(step < Pathfinder.Path.Count)
                    return Pathfinder.Path[step++];
                else return null;
            }

            return null;

            // if(step >= 0) {
            //     if(step < maxSteps && step < Pathfinder.Path.Count) {
            //         int s = step++;
            //         if(step >= maxSteps || step >= Pathfinder.Path.Count)
            //             step = -1;

            //         return Pathfinder.Path[s];
            //     } else {
            //         step = -1;
            //         return null;
            //         // return Point.Zero;
            //     }
            // } else {
            //     step = 0;
            //     Pathfinder.Path.Clear();
            // }

            // if(newPath) {
            //     newPath = false;
            //     // if(Pathfinder.Path.Count > 0) {
            //         // Point step = Pathfinder.Path[0];
            //         // Pathfinder.Path.Clear();
            //         // return step;
            //     if(step < maxSteps && step < Pathfinder.Path.Count) {
            //         return Pathfinder.Path[step++];
            //     } else return Point.Zero;
            // } else newPath = true;
            
            // Pathfinder.Validator vl = pos => {
            //     bool playerAdjacent = false;
            //     Point p = new Point(0, -1);
            //     for(int s = 0; s < 4; ++s) {
            //         if(ContainingMap.getEntities(pos + MapUtils.rotatePoint(p, (Facing)s))
            //         .Where(e => e is Player).Count() > 0) {
            //             playerAdjacent = true;
            //             break;
            //         }
            //     }

            //     return playerAdjacent && ContainingMap
            //         .getTiles(pos).Where(t => t.Solid)
            //         .Count() == 0;
            // };

            // Point tl = ContainingMap.BattleMap.Position.ToPoint() - ContainingMap.Viewport;
            // Point br = ContainingMap.BattleMap.Position.ToPoint() + ContainingMap.Viewport;
            // Pathfinder.getShortestPath(tl, br, vl);
            // return null;
        }

        /// Checks if there is an affordable ability
        /// that would hit a creature and returns it.
        /// In case several creatures were to be hit 
        /// the most expensive ability that hits the 
        /// creature with the lowest health is chosen
        public Ability nextAbility() {
            Ability ability = null;
            Facing facing = Facing;
            int minHP = -1;
            int maxAP = 0;

            Abilities.Where(a => a.APCost <= AP).ToList().ForEach(
                a => checkAbility(a).ForEach(c => {
                    if((minHP < 0 || minHP < c.HP)
                    || (minHP == HP && c.AP > maxAP)) {
                        ability = a;
                        maxAP = c.AP;
                        facing = c.Position.X > Position.X ? Facing.East
                            : c.Position.X < Position.X ? Facing.West
                            : c.Position.Y > Position.Y ? Facing.South
                            : c.Position.Y < Position.Y ? Facing.North : Facing;
                    }
                }));

            Facing = facing; // TODO test if turn happens before execution
            return ability;
        }

        /// Helper to check wether any targets are in sight
        /// and on the spots this ability is executed on
        /// and returns them in a list
        private List<Creature> checkAbility(Ability ability) {
            List<Creature> targets = new List<Creature>();

            // TODO check all four facings
            for(int s = 0; s < 4; ++s) ability.getTargetSpots()
                .Where(spot => canSee(Position.ToPoint() + MapUtils.rotatePoint(spot, (Facing)s)))
                .ToList().ForEach(spot => ContainingMap
                    .getEntities(Position.ToPoint() + MapUtils.rotatePoint(spot, (Facing)s))
                    .Where(e => e is Player)
                    .Cast<Player>().ToList()
                    .ForEach(targets.Add));

            return targets;
        }
    }
}