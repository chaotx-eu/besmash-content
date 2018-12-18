namespace BesmashContent {
    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using BesmashContent.Collections;
    using BesmashContent.Utility;

    [DataContract(IsReference = true)]
    public class Team {
        [DataMember]
        /// Leader of the team
        public Player Leader {get; set;}

        [DataMember]
        /// Members of the team excluding the leader
        public List<Player> Members {get; set;}

        [DataMember]
        /// Positions of members relative to the leader
        /// while facing north
        public Dictionary<Player, Point> Formation {get; set;}

        [DataMember]
        /// List of the last spots the leader moved to
        private FixedList<Point> lastSpots;

        [DataMember]
        /// List of spots targeted by players in this team
        private Point[] targetSpots;

        /// All players in this team
        public List<Player> Player {get {
            List<Player> list = new List<Player>(Members);
            if(Leader != null) list.Add(Leader);
            return list;
        }}

        /// Creates a new empty team
        public Team() : this(null) {}

        /// Creates a new team
        public Team(Player leader, params Player[] members) {
            Formation = new Dictionary<Player, Point>();
            lastSpots = new FixedList<Point>();
            Members = new List<Player>();
            if(leader != null) add(leader, members);
        }

        /// Adds leader and members to the team and
        /// initializes required event handler
        public void add(Player leader, params Player[] members) {
            Leader = leader;
            addMembers(members);
        }

        /// Adds members to the team and initializes
        /// required event handler
        public void addMembers(params Player[] members) {
            members.ToList().ForEach(Members.Add);
            lastSpots.Limit += members.Length;
            targetSpots = new Point[Player.Count];
            initHandler();
        }

        /// Removes a member from the team
        public void removeMember(Player player) {
            if(Members.Remove(player)) {
                lastSpots.Limit -= 1;
                targetSpots = new Point[Player.Count];
            }
        }

        /// The time in milliseconds needed for the
        /// leader idling until the members are updated
        public int MaxIdleTime = 150;
        private int idleTime;

        /// Updates the team and aligns the members according
        /// to the formation strategy after the leader has not
        /// moved for MaxIdleTime milliseconds
        public void update(GameTime time) {
            if(Leader.Moving)
                idleTime = 0;
            else {
                if(idleTime < MaxIdleTime)
                    idleTime += time.ElapsedGameTime.Milliseconds;

                if(idleTime >= MaxIdleTime)
                    Members.ForEach(toFormation);
            }
        }

        /// Stops any movement of players in this team
        /// and clears the step buffers
        public void resetFormation() {
            lastSpots.Clear();
            targetSpots = new Point[Player.Count];
        }

        /// Trys to move the member to its associated
        /// position in the team line
        protected void toLine(Player member) {
            int index = Members.IndexOf(member);
            if(index >= lastSpots.Count) return;
            
            Point target = lastSpots[index];
            Point relative = target - member.Position.ToPoint();

            if(!targetSpots.Contains(target))
                member.move(relative.X, relative.Y, (x, y, mv) => null);
        }

        /// Trys to move the member to its position
        /// in the team formation (i.e. its offset),
        /// moves the member to line on failure
        protected void toFormation(Player member) {
            Point offset;
            if(Formation.TryGetValue(member, out offset)) {
                offset = MapUtils.rotatePoint(offset, Leader.Facing);
                Point target = targetSpots[Members.Count] + offset;
                Point relative = target - member.Position.ToPoint();

                if(!relative.Equals(Point.Zero)) {
                    if(!targetSpots.Contains(target)
                    && member.canSee(target.X, target.Y))
                        member.move(relative.X, relative.Y);
   
                    if(!member.Moving) toLine(member);
                }
            } else toLine(member);
        }

        /// Sets the speed of a member relative to its distance
        /// to its target. (Currently unused)
        ///  => TODO figure out how to sync Leader and
        ///     member moves if members fall to far off
        protected void setSpeed(Player member, Point position, Point target) {
            float distance = Math.Abs(target.X - position.X) + Math.Abs(target.Y - position.Y);
            member.StepTime = (int)(Leader.StepTime*(distance <= 1
                ? 1 : distance <= 2 ? 0.5f : (1f - 1f/(distance*2))));
        }

        /// Initializes event handler require for the leader
        /// and members of this team.
        protected void initHandler() {
            Leader.MoveStartedEvent += (mv, args) => {
                lastSpots.Insert(0, args.Position);
                targetSpots[Members.Count] = args.Target;
                
                // switch spot with member/s if he/they is/are standing at the target
                Members.Where(m => m.Position.ToPoint().Equals(args.Target))
                    .ToList().ForEach(m => m.move(
                        args.Position.X - (int)m.Position.X,
                        args.Position.Y - (int)m.Position.Y,
                        (x, y, mo) => null));

                // movement is smother if updated immediatley after event call
                Members.ForEach(member => toFormation(member));
            };

            Members.ForEach(m => m.MoveStartedEvent += (mv, args)
                => targetSpots[Members.IndexOf(m)] = args.Target);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) {
            /// Reinitialize event handler
            initHandler();
        }
    }
}