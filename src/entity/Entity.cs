namespace BesmashContent  {
    using System.Runtime.Serialization;
    using Microsoft.Xna.Framework.Content;

    /// Possible facings on a tile map
    public enum Facing {NORTH, EAST, SOUTH, WEST}
    
    [DataContract]
    public class Entity : MapObject  {
        /// The title of this entity
        [DataMember]
        [ContentSerializer(Optional = true)]
        public string Title {get; set;}

        /// The direction this entitiy is facing
        [DataMember]
        [ContentSerializerIgnore]
        public Facing Facing {get; set;}

        public Entity() {
            // default title
            Title = GetType().Name;
        }
    }
}