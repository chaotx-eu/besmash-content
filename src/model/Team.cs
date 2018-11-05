namespace BesmashContent {
    using BesmashContent.Collections;
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System;
    using System.Runtime.Serialization;

    /// Describes the way players in a team try to
    /// position them selfs while moving over a TileMap.
    [DataContract(IsReference = true)]
    public class FormationStrategy {
        /// The formation grids width and height.
        [DataMember]
        public Point Size {get; set;} = new Point(7, 5);

        /// The facing of players in this formation;
        [DataMember]
        public Facing Facing {get; set;}

        /// Position of the team leader in the
        /// formation grid.
        [DataMember]
        public Point LeaderPosition {
            get {
                return leaderPosition;
            }

            set {
                if(value.X >= 0 && value.X < Size.X
                && value.Y >= 0 && value.Y < Size.Y)
                    leaderPosition = value;
            }
        }

        /// Relative positions of team members to the
        /// team the leader.
        [DataMember]
        public Dictionary<Player, Point> RelativePositions {get {
            return relativePositions == null
                ? (relativePositions = new Dictionary<Player, Point>())
                : relativePositions;
        }}

        private Point leaderPosition;
        private Dictionary<Player, Point> relativePositions;

        public FormationStrategy(Team team) {
            LeaderPosition = new Point(
                Size.X/2,
                Size.Y/2);

            team.Members.Where(m => m != team.Leader).ToList()
                .ForEach(m => RelativePositions[m] = Point.Zero);
        }
    }

    /// A team of player characters with a leader who
    /// is controllable by user input. Other players in
    /// this team will try to move according to the set
    /// FormationStrategy.
    [DataContract(IsReference = true)]
    public class Team {
        /// Default value for maximum players a team can hold.
        public static int DEFAULT_SIZE = 3;

        /// Leader of the team who must not be a member.
        [DataMember]
        public Player Leader {
            get {
                return leader;
            }

            set {
                if(!Members.Contains(value)) {
                    if(value != null && value != leader) {
                        value.MoveStartedEvent += leaderMoveStarted;

                        if(leader != null)
                            leader.MoveStartedEvent -= leaderMoveStarted;
                    }

                    leader = value;
                }
            }
        }

        /// Members of this team excluding the leader.
        [DataMember]
        public List<Player> Members {get {
            return members == null
                ? (members = new List<Player>())
                : members;
        }}

        /// Max amount of players in this team can hold
        /// including the leader.
        [DataMember]
        public int MaxSize {get; private set;}

        /// Current amount of players this team holds
        /// including the leader
        [DataMember]
        public int Size {get; private set;}

        /// FormationStategy of this team.
        [DataMember]
        public FormationStrategy FormationStrategy {get; private set;}

        [DataMember]
        private Point[] playerTargets;

        [DataMember]
        private FixedList<Point> leaderSteps;

        private Player leader;
        private List<Player> members;

        private MoveStartedHandler leaderMoveStarted;
        private MoveStartedHandler memberMoveStarted;
        private MoveFinishedHandler memberMoveFinished;

        public Team() : this(DEFAULT_SIZE) {}
        public Team(int maxSize) : this(null) {MaxSize = maxSize;}
        public Team(Player leader) : this(leader, null) {}
        public Team(Player leader, params Player[] members) {
            if(members != null)
                members.ToList().ForEach(addMember);

            if(leader != null)
                Leader = leader;

            FormationStrategy = new FormationStrategy(this);
            Size = Members.Count + 1;
            MaxSize = Size;
            initEventHandler();
        }

        /// Adds a member to the team.
        public void addMember(Player recruit) {
            if(Size < MaxSize && !members.Contains(recruit)) {
                members.Add(recruit);
                FormationStrategy.RelativePositions[recruit] = Point.Zero;
                recruit.MoveStartedEvent += memberMoveStarted;

                leaderSteps = new FixedList<Point>(Size++);
                playerTargets = new Point[Size];
            }
        }

        /// Removes a member from the team.
        public void removeMember(Player member) {
            if(members.Contains(member)) {
                members.Remove(member);
                FormationStrategy.RelativePositions.Remove(member);
                member.MoveStartedEvent -= memberMoveStarted;
                member.MoveFinishedEvent -= memberMoveFinished;

                playerTargets = new Point[Size--];
                leaderSteps = new FixedList<Point>(Size);
            }
        }

        /// Checks whether the passed player is part
        /// of this team, member or leader.
        public bool contains(Player player) {
            return player == leader
                || Members.Contains(player);
        }

        /// Updates the team members positions according to
        /// the formation strategy.
        int timer = 0;
        public void update(GameTime time) {
            if(Leader.Moving) timer = 0;
            else if(timer < 200) timer += time.ElapsedGameTime.Milliseconds;

            if(timer > 200) {
                List<Point> memberTargets = new List<Point>();
                memberTargets.Add(Leader.Position.ToPoint());

                Members.ToList().ForEach(m =>
                    toFormation(m, Leader.Position.ToPoint(), Leader.Facing));
            }
        }

        private void initEventHandler() {
            memberMoveStarted = (mv, args) =>
                playerTargets[Members.IndexOf((Player)mv)] = args.Target;

            memberMoveFinished = (mv, args) => {
                mv.StepTime = Leader.StepTime;
                mv.MoveFinishedEvent -= memberMoveFinished;
            };

            leaderMoveStarted = (mv, args) => {
                leaderSteps.Insert(0, args.Position);
                playerTargets[Members.Count] = args.Target;

                Members.ToList().ForEach(m => {
                    int memberIndex = Members.IndexOf(m);

                    if(leaderSteps.Count > memberIndex) {
                        Point tgt = leaderSteps[memberIndex];

                        if(!playerTargets.Where(p => p.Equals(tgt)).Any()) {
                            int mx = tgt.X - (int)m.Position.X;
                            int my = tgt.Y - (int)m.Position.Y;

                            toFormation(m, args.Target, mv.Facing);
                            if(!m.Moving) {
                                // m.stop();
                                m.StepTime = Leader.StepTime/(Math.Abs(mx) + Math.Abs(my) > 1 ? 2 : 1);
                                m.MoveFinishedEvent += memberMoveFinished;
                                m.move(mx, my, (x, y, mo) => null);
                            }
                        }
                    }
                });
            };
        }

        private void toFormation(Player member, Point target, Facing facing) {
            int tx, ty;
            Point rp = FormationStrategy.RelativePositions[member];

            switch(facing) {
                case(Facing.NORTH):
                    tx = target.X + rp.X;
                    ty = target.Y + rp.Y;
                    break;
                case(Facing.EAST):
                    tx = target.X - rp.Y;
                    ty = target.Y + rp.X;
                    break;
                case(Facing.SOUTH):
                    tx = target.X - rp.X;
                    ty = target.Y - rp.Y;
                    break;
                case(Facing.WEST):
                    tx = target.X + rp.Y;
                    ty = target.Y - rp.X;
                    break;
                default:
                    tx = target.X;
                    ty = target.Y;
                    break;
            }

            CollisionResolver cr = (x, y, mo) => {
                if(mo is Tile && ((Tile)mo).Solid
                || mo is Entity && !Members.Contains(mo) && mo != Leader)
                    return Point.Zero;

                return null;
            };

            if(Leader.canSee(tx, ty) && member.canSee(tx, ty)
            && !playerTargets.Where(p => p.X == tx && p.Y == ty).Any()) {
                int mx = (int)(tx-member.Position.X);
                int my = (int)(ty-member.Position.Y);
                member.StepTime = Leader.StepTime/(Math.Abs(mx) + Math.Abs(my) > 1 ? 2 : 1);
                member.move(mx, my, cr);
            }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) {
            initEventHandler();
            Members.ForEach(m => m.MoveStartedEvent += memberMoveStarted);

            // needs to be assigned explicitly since the handler
            // was not accessable during deserialization
            if(Leader != null) Leader.MoveStartedEvent += leaderMoveStarted;
            leaderSteps.Limit = MaxSize;
        }
    }
}