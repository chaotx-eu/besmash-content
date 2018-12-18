namespace BesmashContent
{
    using BesmashContent.Utility;
    using Microsoft.Xna.Framework;
    
    using Microsoft.Xna.Framework.Content;
    using System.Collections.Generic;
    public class MovementAbility : Ability
    {
        
        [ContentSerializer(Optional = true)]
        public Point[] path{get;set;}
        
        [ContentSerializer(Optional = true)]
        public Point destination{get;set;}
        
        [ContentSerializer(Optional = true)]
        public Movable target{get;set;} //Die sich zu bewegende kreatur
        
        [ContentSerializer(Optional = true)]
        public int maxDistance{get;set;}
    //  public CollisionResolver resolver{get;set;}

        public MovementAbility() {} // TODO quick hotfix for content serializer
        public MovementAbility(Creature user, int cost, string name, Point[] path, Movable target) : base(user, cost, name)
        {
            this.path = path;
            this.type = Type.move;
        }
        public MovementAbility(Creature user, int cost, string name, Point destination, Movable target) : base(user, cost, name)
        {
            maxDistance = Creature.BattleManager.fightingEntities.Find(x => x.Creature == AbilityUser).stats.AGI;
            this.path = determineShortestPath(new Point((int)target.Position.X, (int)target.Position.Y), destination);
            this.type = Type.move;
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
            if (MovementAbility.isPathPossible(start, destination, maxDistance, Creature.BattleManager.map))
            {
                return MapUtils.shortestPath(start, destination, maxDistance, Creature.BattleManager.map);
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