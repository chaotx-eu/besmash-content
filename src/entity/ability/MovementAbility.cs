namespace BesmashContent
{
    using Microsoft.Xna.Framework;
    public class MovementAbility : Ability
    {
        public Vector2[] path{get;set;}

        public MovementAbility(Entity user, int cost, string name, Vector2[] path) : base(user, cost, name)
        {
            this.path = path;
        }
        public override void useAbility()
        {
            foreach(Vector2 step in path)
            {
                //todo
            }
        }

        public static bool isPathPossible(Vector2 start, Vector2 destination, int speed)
        {
            bool possible = false;
            //todo
            return possible;
        }

        public static Vector2[] determineShortestPath(Vector2 start, Vector2 destination)
        {
            //todo
            return null;
        }
    }
}