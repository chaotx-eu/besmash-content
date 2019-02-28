namespace BesmashContent {
    using Utility;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using System.Collections.Generic;
    using System.Linq;
    using System;


    public class Enemy : Npc, IBattleAI {
        /// The range at which this enemy will drag
        /// the active team into a battle
        [ContentSerializer(Optional = true)]
        public int AggroRange {get; set;} = 2; // TODO remove default value

        /// Fires whenever a player gets in aggro range
        /// of this enemy
        public event PlayerEventHandler PlayerInRangeEvent;

        /// Updates this enemy, checks if there is
        /// a player within aggro range and starts
        /// a battle if this is the case
        public override void update(GameTime gameTime) {
            base.update(gameTime);
            Player slave = ContainingMap != null
                ? ContainingMap.Slave as Player : null;

            if(slave != null) {
                int vx = (int)slave.Position.X - ContainingMap.Viewport.X/2;
                int vy = (int)slave.Position.Y - ContainingMap.Viewport.Y/2;
                int vw = ContainingMap.Viewport.X;
                int vh = ContainingMap.Viewport.Y;
                int x = (int)Position.X;
                int y = (int)Position.Y;
                if(!IsFighting && x >= vx && x <= vx+vw
                && y >= vy && y <= vy+vh) {
                    int r = AggroRange;
                    if(ContainingMap.State == TileMap.MapState.Fighting) r *= 2;
                    if(playerInRange(r)) onPlayerInRange(null); // TODO (fix and optimize!)
                }
            }
        }

        /// Checks wether a player is within aggro range
        public bool playerInRange() {
            return playerInRange(AggroRange);
        }

        /// Checks wether a player is within
        /// range of the passed radius
        public bool playerInRange(int r) {
            int px = (int)Position.X;
            int py = (int)Position.Y;
            for(int x, y = -r; y <= r; ++y)
            for(x = -r; x <= r; ++x) {
                if(canSee(px + x, py + y)
                && ContainingMap.getEntities(px + x, py + y)
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

            if(!Pathfinder.IsAtWork) {
                if(step < Pathfinder.Path.Count)
                    return Pathfinder.Path[step++];
                else return null;
            }

            return null;
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
                    || (minHP == HP && a.APCost > maxAP)) {
                        ability = a;
                        maxAP = a.APCost;
                        facing = c.Position.X > Position.X ? Facing.East
                            : c.Position.X < Position.X ? Facing.West
                            : c.Position.Y > Position.Y ? Facing.South
                            : c.Position.Y < Position.Y ? Facing.North : Facing;
                    }
                }));

            // no offensive ability available
            if(ability == null) {
                Ability rest = Abilities
                    .Where(a => a.Title.Equals("Rest"))
                    .FirstOrDefault();

                if(playerInRange(1)) ability = Abilities
                    .Where(a => a.Title.Equals("Defend"))
                    .FirstOrDefault();
                else if(AP < MaxAP/2) ability = rest;
            }

            Facing = facing; // TODO test if turn happens before execution
            return ability;
        }

        /// Helper to check wether any targets are in sight
        /// and on the spots this ability is executed on
        /// and returns them in a list
        private List<Creature> checkAbility(Ability ability) {
            List<Creature> targets = new List<Creature>();

            for(int s = 0; s < 4; ++s) ability.getTargetSpots()
                .Where(spot => canSee(Position.ToPoint() + MapUtils.rotatePoint(spot, (Facing)s)))
                .ToList().ForEach(spot => ContainingMap
                    .getEntities(Position.ToPoint() + MapUtils.rotatePoint(spot, (Facing)s))
                    .Where(e => e is Player)
                    .Cast<Player>().ToList()
                    .ForEach(targets.Add));

            return targets;
        }

        protected void onPlayerInRange(Player player) {
            PlayerEventHandler handler = PlayerInRangeEvent;
            if(handler != null) handler(this, new PlayerEventArgs(player));
        }
    }
}