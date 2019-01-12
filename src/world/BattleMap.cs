namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;

    /// A rectengular movable map containing
    /// battle participitans
    public class BattleMap : Movable {
        /// List of all creatures participating
        /// the battle on this map
        private List<Creature> participants;
        public List<Creature> Participants {
            get {return participants == null
                ? (participants = new List<Creature>())
                : participants;}
            protected set {participants = value;}
        }

        /// The size of this map in tiles
        public Point Size {get; set;}

        public BattleMap() {}
        public BattleMap(TileMap tileMap) {
            ContainingMap = tileMap;
            CollisionResolver = (x, y, mvs) => null; // no collision ever
            StepTime = 500; // TODO remove hardcoded value
        }
    }
}