namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Linq;
    using Collections;
    using Utility;

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

        /// Reference to the map object this
        /// pathfinder belongs to
        [DataMember]
        public MapObject Owner {get; protected set;}

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
        // [DataMember]
        // private List<Node> openList;

        /// List of positions that still have
        /// to be validated
        [DataMember]
        private PriorityQueue<int, Node> openList;

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
        [DataMember] private Node pathProbe;


        public Pathfinder() {}
        public Pathfinder(MapObject owner) {
            Owner = owner;
        }

        /// Searches for the shortest path from origin to a
        /// target, within the rectangle defined by the two
        /// points for the top left and bottom right corner
        /// using A*-search, which is identified by the
        /// validator and returns an ordered list of points
        /// representing the single steps required
        public void getShortestPath(Point tl, Point br, Validator vl) {
            // this.tl = tl;
            // this.br = br;
            // validator = vl;
            // IsAtWork = true;
            // targets = new List<Point>();
            // openList = new List<Node>();
            // openList.Add(new Node(Owner.Position.ToPoint()));
            // closedList = new Dictionary<Point, Node>();
            // Path = new List<Point>();
            // i = 0;

            this.tl = tl;
            this.br = br;
            targets = new List<Point>();
            for(int x, y = tl.Y; y <= br.Y; ++y)
            for(x = tl.X; x <= br.X; ++x) {
                Point pos = new Point(x, y);
                if(vl(pos)) targets.Add(pos);
            }

            openList = new PriorityQueue<int, Node>();
            openList.enqueue(0, new Node(Owner.Position.ToPoint()));
            closedList = new Dictionary<Point, Node>();
            Path = new List<Point>();
            IsAtWork = targets.Count > 0;
            pathProbe = null;
        }

        // /// Updates this pathfinder and validates
        // /// the next positions from the openList
        // public void update() {
        //     if(!IsAtWork) return;

        //     Point pos;
        //     Node probe, node, other;

        //     if(openList.Count > 0) {
        //         if(i >= 0) {
        //             probe = openList[i];
        //             openList.RemoveAt(i);
        //             closedList[probe.Pos] = probe;
        //             sides.ForEach(s => {
        //                 other = null;
        //                 pos = s + probe.Pos;
        //                 node = new Node(pos, probe);
        //                 closedList.TryGetValue(pos, out other);
        //                 if(validator(pos) && !targets.Contains(pos))
        //                     targets.Add(pos);

        //                 if(pos.X >= tl.X && pos.X <= br.X
        //                 && pos.Y >= tl.Y && pos.Y <= br.Y
        //                 && (other == null || other.Len > node.Len)
        //                 && !Owner.ContainingMap.getTiles(pos).Any(t => t.Solid)
        //                 && openList.Where(o => o.Pos == pos && o.Len <= node.Len).Count() == 0
        //                 && Owner.ContainingMap.getEntities(pos)
        //                 .Where(e => e is Creature).Count() == 0)
        //                     openList.Add(node);
        //             });

        //             --i;
        //         } else i = openList.Count-1;
        //     } else {
        //         node = null;
        //         IsAtWork = false;
        //         targets.ForEach(tgt =>  {
        //             if(node != null) return;
        //             closedList.TryGetValue((Point)tgt, out node);
        //         });

        //         while(node != null) {
        //             if(node.Pre != null) // skip first spot
        //                 Path.Insert(0, node.Pos);

        //             node = node.Pre;
        //         }
        //     }
        // }

        /// Updates this pathfinder and validates
        /// the next positions from the openList
        public void update() {
            if(!IsAtWork) return;

            if(pathProbe != null) {
                if(pathProbe.Pre == null) {
                    pathProbe = null;
                    IsAtWork = false;
                } else {
                    path.Insert(0, pathProbe.Pos);
                    pathProbe = pathProbe.Pre;
                }

                return;
            }

            Point pos;
            Node probe, node, other;

            if(openList.Count > 0) {
                probe = openList.dequeue();

                if(targets.Contains(probe.Pos)) {
                    pathProbe = probe;
                    // IsAtWork = false;
                    // while(probe != null) {
                    //     if(probe.Pre != null) // skip first spot
                    //         Path.Insert(0, probe.Pos);

                    //     probe = probe.Pre;
                    // }
                } else {
                    closedList[probe.Pos] = probe;
                    sides.ForEach(s => {
                        pos = s + probe.Pos;

                        if(pos.X >= tl.X && pos.X <= br.X
                        && pos.Y >= tl.Y && pos.Y <= br.Y
                        && !Owner.ContainingMap.getTile(pos).Occupied
                        && !Owner.ContainingMap.getTiles(pos).Any(t => t.Solid)) {
                            other = null;
                            node = new Node(pos, probe);
                            closedList.TryGetValue(pos, out other);

                            if(other == null) {
                                other = openList.Values.Find(n =>
                                    n.Pos.Equals(pos));

                                if(other != null && other.Len > node.Len)
                                    openList.remove(nodeValue(other), other);

                                if(other == null || other.Len > node.Len)
                                    openList.enqueue(nodeValue(node), node);
                            }
                        }
                    });
                }
            } else IsAtWork = false;
        }

        private int nodeValue(Node node) {
            int h = 999999;
            List<Point> ray;
            targets.ForEach(pos => {
                ray = MapUtils.getRay(node.Pos, pos);
                if(ray.Count < h) h = ray.Count;
            });

            return node.Len + h;
        }
    }
}