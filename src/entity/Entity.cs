namespace BesmashContent  {
    public enum Facing {NORTH, EAST, SOUTH, WEST}
    
    public class Entity : MapObject  {
        /// The direction this entitiy is facing
        public Facing Facing {get; set;} = Facing.SOUTH;

        /// The title of this entity
        public string Title {get; protected set;} = "Entity";
    }
}