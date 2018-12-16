namespace BesmashContent
{
    using BesmashContent.Utility;
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    public class MovementAbility : Ability
    {
        public Point[] path{get;set;}
        public Point destination{get;set;}
        public Movable target{get;set;} //Die sich zu bewegende kreatur
        public int maxDistance{get;set;}
    //  public CollisionResolver resolver{get;set;}

        public MovementAbility(Creature user, int cost, string name, Point[] path, Movable target) : base(user, cost, name)
        {
            this.path = path;
        }
        public MovementAbility(Creature user, int cost, string name, Point destination, Movable target) : base(user, cost, name)
        {
            maxDistance = AbilityUser.battleManager.fightingEntities.Find(x => x.Creature == AbilityUser).stats.AGI;
            this.path = determineShortestPath(new Point((int)target.Position.X, (int)target.Position.Y), destination);

        }
        public override void useAbility()
        {
            foreach(Point step in path)
            {
                target.move((int)step.X, (int)step.Y);
            }
        }

        public Point[] determineShortestPath(Point start, Point destination)
        {
            if (MovementAbility.isPathPossible(start, destination, maxDistance, AbilityUser.battleManager.map))
            {
                return MapUtils.shortestPath(start, destination, maxDistance, AbilityUser.battleManager.map);
            }
            else 
                return new Point[0];
        }

        public static bool isPathPossible(Point start, Point destination, int maxDistance, TileMap map)
        {
            if (MapUtils.shortestPath(start, destination, 12, map).Length < maxDistance)
                return true;
            else
                return false;
        }
    }
}