namespace BesmashContent
{
    using Microsoft.Xna.Framework;
    public class PathfindingNode
    {
        public enum Direction{Up, Down, Left, Right}
        public int step{get;set;}
        public Point position{get;set;}

        public PathfindingNode up{get;set;}
        public PathfindingNode down{get;set;}
        public PathfindingNode left{get;set;}
        public PathfindingNode right{get;set;}
        public PathfindingNode previous{get;set;}
        public Direction pathBack{get;set;}

        public PathfindingNode(int step, Point position)
        {
            this.step = step;
            this.position = position;
        }
        public void add(PathfindingNode toAdd, Direction direction)
        {
            switch (direction)
            {
                case Direction.Up : 
                    up = toAdd;
                    toAdd.previous = this;
                    toAdd.pathBack = Direction.Down;
                    break;
                case Direction.Down : 
                    down = toAdd;
                    toAdd.previous = this;
                    toAdd.pathBack = Direction.Up;
                    break;
                case Direction.Left : 
                    left = toAdd;
                    toAdd.previous = this;
                    toAdd.pathBack = Direction.Right;
                    break;
                case Direction.Right : 
                    right = toAdd;
                    toAdd.previous = this;
                    toAdd.pathBack = Direction.Left;
                    break;
            }
        }

        public Point[] path()
        {
            PathfindingNode temp = this;
            Point[] path = new Point[step];
            int i = step;
            while(temp.previous != null && i <= 0)
            {
                switch (pathBack)
                {
                    case Direction.Up : path[i] = new Point(0,1); break;
                    case Direction.Down : path[i] = new Point(0,-1); break;
                    case Direction.Left : path[i] = new Point(1,0); break;
                    case Direction.Right : path[i] = new Point(-1,0); break;
                }
                i--;
            }
            return path;
        }
    }
}