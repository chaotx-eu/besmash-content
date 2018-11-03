namespace BesmashContent {
    public enum Facing {NORTH, EAST, SOUTH, WEST}

    public class Entity : MapObject {
        public Facing Facing = Facing.SOUTH;
    }
}