namespace BesmashContent  {
    using Microsoft.Xna.Framework.Content;

    /// Possible facings on a tile map
    public enum Facing {NORTH, EAST, SOUTH, WEST}
    
    public class Entity : MapObject  {
        /// The title of this entity
        [ContentSerializer(Optional = true)]
        public string Title {get; set;}

        /// The direction this entitiy is facing
        [ContentSerializerIgnore]
        public Facing Facing {get; set;} = Facing.SOUTH;

        public Entity() {
            // default title
            Title = this.GetType().Name;
        }
    }
}