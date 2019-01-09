namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using System.Linq;


    public class Enemy : Npc, IBattleAI {
        /// A node which is used for A*-search
        private class Node {
            public Point Pos {get; set;}
            public Node Pre {get; set;}
            public int Len {get;}

            public Node(Point pos) : this(pos, null) {}
            public Node(Point pos, Node pre) {
                Pos = pos;
                Pre = pre;
                Len = pre != null ? pre.Len+1 : 0;
            }
        }

        /// Moves towards the closest player in sight
        /// within the battle map (i.e. the maps viewport)
        /// (In case several are in the same range the
        /// one with the lowest health is targeted) TODO
        public Point? nextMove() {
            Validator vl = pos => ContainingMap
                .getEntities(pos)
                .Where(e => e is Player)
                .Count() > 0;

            Point tl = ContainingMap.BattleMapCenter - ContainingMap.Viewport;
            Point br = ContainingMap.BattleMapCenter + ContainingMap.Viewport;
            List<Point> path = getShortestPath(tl, br, vl);
            return path.Count > 1 ? (Point?)path[0] : null;
        }
        
        /// Checks condition of given point on a map
        /// and returns wether it is a valid target
        public delegate bool Validator(Point p);

        /// Searches for the shortest path to a target,
        /// within the rectangle defined by the two points
        /// for the top left and bottom right corner
        /// using A*-search, which is identified by the
        /// validator and returns an ordered list of points
        /// representing the steps required
        public List<Point> getShortestPath(Point tl, Point br, Validator vl) {
            List<Point> sides = new Point[] {
                new Point(0, 1), new Point(1, 0),
                new Point(0, -1), new Point(-1, 0)
            }.ToList();

            List<Node> open = new List<Node>();
            HashSet<Node> closed = new HashSet<Node>();
            open.Add(new Node(Position.ToPoint()));

            int i;
            Point pos;
            Node probe, node;
            while(open.Count > 0) {
                for(i = open.Count; i >= 0; --i) {
                    probe = open[i];
                    open.RemoveAt(i);
                    closed.Add(probe);
                    sides.ForEach(s => {
                        pos = s + probe.Pos;
                        node = new Node(pos, probe);

                        if(pos.X >= tl.X && pos.X <= br.X
                        && pos.Y >= tl.Y && pos.Y <= tl.Y
                        && !closed.Contains(node) && vl(pos))
                            open.Add(node);
                    });
                }
            }

            node = closed.OrderBy(n => n.Len).First();
            List<Point> path = new List<Point>();

            while(node != null) {
                path.Insert(0, node.Pos);
                node = node.Pre;
            }

            return path;
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