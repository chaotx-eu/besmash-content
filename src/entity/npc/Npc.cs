namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using System.Runtime.Serialization;
    using System.Collections.Generic;
    using System.Linq;


    [DataContract]
    public class Npc : Creature {
        /// The max distance this npc may move away
        /// from its spawn point while roaming.
        /// Negative values will allow it to roam
        /// over the whole map.
        [DataMember]
        [ContentSerializer(Optional = true)]
        public int RoamingRadius {get; set;}

        /// The time in seconds that have to pass before
        /// this npc chooses to roam to another position
        /// where X is the min and Y the max time
        [DataMember]
        [ContentSerializer(Optional = true)]
        public Point RoamingPause {get; set;} = new Point(5, 30);

        /// The position at which this npc was spawned
        [DataMember]
        [ContentSerializerIgnore]
        public Point SpawnPosition {get; set;}

        /// The pathfinder of this npc
        // [DataMember] // TODO saving this requires absurd amounts of memory
        [ContentSerializerIgnore]
        public Pathfinder Pathfinder {get; protected set;}

        private int pause;
        private int timer;
        private int moveTimer;
        private int movePause = 1000;
        private int blocks;
        private int maxBlocks = 10;

        public Npc() {
            Pathfinder = new Pathfinder(this);
        }

        /// Updates this npc and may evaluate a new random
        /// target for roaming or move to the next position
        /// in the path of the pathfinder towrads the target
        public override void update(GameTime gameTime) {
            base.update(gameTime);

            if(Pathfinder.IsAtWork) {
                Pathfinder.update();
                return;
            } else if(!Moving && Pathfinder.Path.Count > 0) {
                if(moveTimer >= movePause) {                
                    if(move(Pathfinder.Path[0] - Position.ToPoint())) {
                        Pathfinder.Path.RemoveAt(0);
                        blocks = 0;
                    } else {
                        moveTimer = 0;

                        if(++blocks >= maxBlocks)
                            Pathfinder.Path.Clear();
                    }
                } else moveTimer +=
                    gameTime.ElapsedGameTime.Milliseconds;
            } else if(timer > 1000*pause) {
                if(evaluateTarget()) {
                    pause = RNG.Next(RoamingPause.X, RoamingPause.Y+1);
                    timer = 0;
                }
            } else timer +=
                gameTime.ElapsedGameTime.Milliseconds;
        }

        /// Selects a random target positon within the
        /// roaming radius of the spawn position. Returns
        /// true if the position is a tile that an entity
        /// can step on false otherwise
        private bool evaluateTarget() {
            Point target = new Point(
                RNG.Next(
                    SpawnPosition.X - RoamingRadius,
                    SpawnPosition.X + RoamingRadius+1),
                RNG.Next(
                    SpawnPosition.Y - RoamingRadius,
                    SpawnPosition.Y + RoamingRadius+1)
            );

            if(!ContainingMap.getTiles(target).Any(t => t.Solid)) {
                Pathfinder.getShortestPath(
                    SpawnPosition - new Point(RoamingRadius, RoamingRadius),
                    SpawnPosition + new Point(RoamingRadius, RoamingRadius),
                    (point) => point.Equals(target)
                );

                return true;
            }

            return false;
        }
    }
}