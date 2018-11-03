namespace BesmashContent {
    using BesmashContent.Collections;
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System;

    /// Describes the way players in a team try to
    /// position them selfs while moving over a TileMap.
    public class FormationStrategy {
        /// The formation grids width and height.
        public Point FormationGrid {get; set;} = new Point(7, 5);

        /// The facing of players in this formation;
        public Facing Facing {get; set;}

        /// Position of the team leader in the
        /// formation grid.
        public Point LeaderPosition {
            get {
                return leaderPosition;
            }

            set {
                if(value.X >= 0 && value.X < FormationGrid.X
                && value.Y >= 0 && value.Y < FormationGrid.Y)
                    leaderPosition = value;
            }
        }

        private Point leaderPosition;

        /// Relative positions of team members to the
        /// team the leader.
        public Dictionary<Player, Point> RelativePositions {get;}

        public FormationStrategy(Team team) {
            RelativePositions = new Dictionary<Player, Point>();
            LeaderPosition = new Point(
                FormationGrid.X/2,
                FormationGrid.Y/2);

            team.Members.Where(m => m != team.Leader).ToList()
                .ForEach(m => RelativePositions[m] = Point.Zero);
        }
    }

    /// A team of player characters with a leader who
    /// is controllable by user input. Other players in
    /// this team will try to move according to the set
    /// FormationStrategy.
    public class Team {
        /// Leader of the team who must not be a member.
        public Player Leader {
            get {
                return leader;
            }

            set {
                if(!Members.Contains(value)) {
                    if(value != null && value != leader) {
                        value.MoveStartedEvent += leaderMoveStarted;
                        // value.MoveFinishedEvent += leaderMoveFinished;
                        if(leader != null)
                            leader.MoveStartedEvent -= leaderMoveStarted;
                            // leader.MoveFinishedEvent -= leaderMoveFinished;
                    }

                    leader = value;
                }
            }
        }

        private Player leader;

        /// Members of this team.
        public ReadOnlyCollection<Player> Members {
            get {
                return members.AsReadOnly();
            }
        }

        private List<Player> members = new List<Player>();

        /// FormationStategy of this team.
        public FormationStrategy FormationStrategy {get;}

        private MoveStartedHandler leaderMoveStarted;
        private MoveStartedHandler memberMoveStarted;
        private MoveFinishedHandler memberMoveFinished;
        private FixedList<Point> leaderSteps;
        private Point[] playerTargets;

        public Team(Player leader, ICollection<Player> members) {
            FormationStrategy = new FormationStrategy(this);

            memberMoveStarted = (mv, args) =>
                playerTargets[Members.IndexOf((Player)mv)] = args.Target;

            memberMoveFinished = (mv, args) => {
                mv.StepTime = Leader.StepTime;
                mv.MoveFinishedEvent -= memberMoveFinished;
            };

            leaderMoveStarted = (mv, args) => {
                leaderSteps.Insert(0, args.Position);
                playerTargets[Members.Count] = args.Target;

                if(leaderSteps.Count >= Members.Count) {
                    members.ToList().ForEach(m => {
                        Point tgt = leaderSteps[Members.IndexOf(m)];
                        if(!playerTargets.Where(p => p.Equals(tgt)).Any()) {
                            int mx = tgt.X - (int)m.Position.X;
                            int my = tgt.Y - (int)m.Position.Y;

                            toFormation(m, args.Target, mv.Facing);
                            if(!m.Moving) {
                                m.stop();
                                m.StepTime = Leader.StepTime/(Math.Abs(mx) + Math.Abs(my) > 1 ? 2 : 1);
                                m.MoveFinishedEvent += memberMoveFinished;
                                m.move(mx, my, (x, y, mo) => null);
                            }
                        }
                    });
                }
            };

            members.Remove(leader);
            members.ToList().ForEach(addMember);
            Leader = leader;
            leaderSteps = new FixedList<Point>(members.Count);
            playerTargets = new Point[members.Count+1];
        }

        public void addMember(Player recruit) {
            if(!members.Contains(recruit)) {
                members.Add(recruit);
                FormationStrategy.RelativePositions[recruit] = Point.Zero;
                recruit.MoveStartedEvent += memberMoveStarted;
            }
        }

        public void removeMember(Player member) {
            if(members.Contains(member)) {
                members.Remove(member);
                FormationStrategy.RelativePositions.Remove(member);
                member.MoveStartedEvent -= memberMoveStarted;
                member.MoveFinishedEvent -= memberMoveFinished;
            }
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
    }
}