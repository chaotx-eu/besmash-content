namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Linq;

    /// Utility class for which implements search
    /// algorithms to provide paths to specific
    /// target locations on a map
    [DataContract(IsReference = true)]
    public class Pathfinder {
        /// A node which is used for A*-search
        [DataContract]
        protected class Node {
            [DataMember] public Point Pos {get; set;}
            [DataMember] public Node Pre {get; set;}
            [DataMember] public int Len {get; private set;}

            public Node(Point pos) : this(pos, null) {}
            public Node(Point pos, Node pre) {
                Pos = pos;
                Pre = pre;
                Len = pre != null ? pre.Len+1 : 0;
            }
        }

        /// Checks condition for given point on a map
        /// and returns wether it is a valid target
        public delegate bool Validator(Point p);

        /// All four directions a movable may step to
        private static List<Point> sides = new Point[] {
            new Point(0, 1), new Point(1, 0),
            new Point(0, -1), new Point(-1, 0)
        }.ToList();

        /// Wether a new path is currently
        /// beeing searched for
        [DataMember]
        public bool IsAtWork {get; protected set;}

        /// Reference to the npc this pathfinder
        /// belongs to
        [DataMember]
        public Npc Owner {get; protected set;}

        /// An ordered list of points representing
        /// the single steps required from origin
        /// to a valid target
        [DataMember] private List<Point> path;
        public List<Point> Path {
            get {return path == null ? (path = new List<Point>()) : path;}
            protected set {path = value;}
        }

        /// List of positions that still have
        /// to be validated
        [DataMember]
        private List<Node> openList;

        /// Collection of positions that have been
        /// validated
        [DataMember]
        private Dictionary<Point, Node> closedList;

        /// The currently active validator
        // [DataMember] // TODO this will not work
        private Validator validator;

        /// List of possbile target position
        [DataMember]
        private List<Point> targets;

        /// Top left and bottom right coordinates of the rectangle
        /// in which a path to the target is searched for
        [DataMember] private Point tl;
        [DataMember] private Point br;

        /// Current index within the open list
        [DataMember] private int i;

        public Pathfinder() {}
        public Pathfinder(Npc owner) {
            Owner = owner;
        }

        /// Searches for the shortest path from origin to a
        /// target, within the rectangle defined by the two
        /// points for the top left and bottom right corner
        /// using A*-search, which is identified by the
        /// validator and returns an ordered list of points
        /// representing the single steps required
        public void getShortestPath(Point tl, Point br, Validator vl) {
            this.tl = tl;
            this.br = br;
            validator = vl;
            IsAtWork = true;
            targets = new List<Point>();
            openList = new List<Node>();
            openList.Add(new Node(Owner.Position.ToPoint()));
            closedList = new Dictionary<Point, Node>();
            Path = new List<Point>();
        }

        /// Updates this pathfinder and validates
        /// the next positions from the openList
        public void update() {
            if(!IsAtWork) return;

            Point pos;
            Node probe, node, other;

            if(openList.Count > 0) {
                if(i >= 0) {
                    other = null;
                    probe = openList[i];
                    openList.RemoveAt(i);
                    closedList[probe.Pos] = probe;
                    sides.ForEach(s => {
                        pos = s + probe.Pos;
                        node = new Node(pos, probe);
                        closedList.TryGetValue(pos, out other);
                        if(validator(pos)) targets.Add(pos);

                        if(pos.X >= tl.X && pos.X <= br.X
                        && pos.Y >= tl.Y && pos.Y <= br.Y
                        && !Owner.ContainingMap.getTiles(pos).Any(t => t.Solid)
                        && (other == null || other.Len > node.Len))
                            openList.Add(node);
                    });

                    --i;
                } else i = openList.Count-1;
            } else {
                node = null;
                IsAtWork = false;
                targets.ForEach(tgt =>  {
                    if(node != null) return;
                    closedList.TryGetValue((Point)tgt, out node);
                });

                while(node != null) {
                    if(node.Pre != null) // skip first spot
                        Path.Insert(0, node.Pos);

                    node = node.Pre;
                }
            }
        }
    }
}