namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using System.Runtime.Serialization;
    using System.Collections.Generic;

    /// A spawn is a creature with a weight
    /// value attached to it which is compared
    /// with the weight of other spawns to
    /// determine the possibility of beeing
    /// placed on a map
    [DataContract]
    public class Spawn {
        /// Path to the npc object file
        [DataMember]
        public string NPC {get; set;}

        /// Weight of this spwan
        [DataMember]
        [ContentSerializer(Optional = true)]
        public int Weight {get; set;} = 1;
    }

    /// A position on a map on which one npc
    /// out of a pre defined list of npcs is
    /// placed when the containing map (TODO) is loaded
    [DataContract]
    public class SpawnPoint {
        /// Position on a tile map to spawn an npc on
        [DataMember]
        public Point Position {get; set;}

        /// Weight of this spawnpoint
        [DataMember]
        [ContentSerializer(Optional = true)]
        public int Weight {get; set;}

        /// List of spawns of which one may be chosen
        /// when the containing map is loaded (TODO)
        [DataMember]
        [ContentSerializer(CollectionItemName = "Spawn")]
        public List<Spawn> Spawns {get; set;}
    }
}