namespace BesmashContent.Utility {
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using System;

    public class MapUtils {
        public static void swap(ref int a, ref int b) {
            a ^= b;
            b ^= a;
            a ^= b;
        }

        /// Rotates a point to the target facing relative
        /// to the default facing north.
        public static Point rotatePoint(Point point, Facing facing) {
            Point rotated;
            switch(facing) {
                case(Facing.EAST):
                    rotated = new Point(-point.Y, point.X);
                    break;
                case(Facing.SOUTH):
                    rotated = new Point(-point.X, -point.Y);
                    break;
                case(Facing.WEST):
                    rotated = new Point(point.Y, -point.X);
                    break;
                default:
                    rotated = point;
                    break;
            }

            return rotated;
        }

        /// Creates a line of points using the Bresenham-Algorithm
        /// https://www.codeproject.com/Articles/15604/Ray-casting-in-a-2D-tile-based-environment
        public static List<Point> getRay(int x1, int y1, int x2, int y2) {
            List<Point> result = new List<Point>();
            bool steep = Math.Abs(y2 - y1) > Math.Abs(x2 - x1);

            if(steep) {
                swap(ref x1, ref y1);
                swap(ref x2, ref y2);
            }

            if (x1 > x2) {
                swap(ref x1, ref x2);
                swap(ref y1, ref y2);
            }

            int deltax = x2 - x1;
            int deltay = Math.Abs(y2 - y1);
            int error = 0;
            int ystep;

            if(y1 < y2) ystep = 1;
            else ystep = -1;

            for(int x = x1, y = y1; x <= x2; x++) {
                if(steep) result.Add(new Point(y, x));
                else result.Add(new Point(x, y));
                error += deltay;

                if(2*error >= deltax) {
                    y += ystep;
                    error -= deltax;
                }
            }

            return result;
        }
        public static int ManhattenDistance(Point a, Point b)
        {
            int x = a.X - b.X;
            int y = a.Y - b.Y;
            return (x < 0 ? x * -1 : x) + (y < 0 ? y * -1 : y);
        }
        public static int ManhattenDistance(Vector2 a, Vector2 b)
        {
            int x = (int)a.X - (int)b.X;
            int y = (int)a.Y - (int)b.Y;
            return (x < 0 ? x * -1 : x) + (y < 0 ? y * -1 : y);
        }

        public static Point[] shortestPath(Vector2 start, Vector2 destination, int maxDistance, TileMap tileMap)
        {
            return shortestPath(new Point((int)start.X, (int) start.Y), new Point((int)destination.X, (int)destination.Y), maxDistance, tileMap);
        }
        public static Point[] shortestPath(Point start, Point destination, int maxDistance, TileMap tileMap)
        {
            int offset = maxDistance; //halbe Größe des Arrays (entfernung vom Mittelpunkt des Arrays (startpunkt) zu den Rändern)
            //Nur ein kleiner Teil der gesamtmap wird abgebildet.
            //Bitte nur gerade Zahlen nehmen.
            //Die bisherigen mindestkosten zu den Feldern
            int[][] map;
            //Array initialisierung
            map = new int[25][];
            for(int i = 0; i < map.Length; i++)
            {
                map[i] = new int[25];
                for(int j = 0; j < map.Length; j++)
                {
                    map[i][j] = -1; //Standard Wert -1 um unbesuchte Felder zu markieren
                }
            }

            List<PathfindingNode> openList = new List<PathfindingNode>();
            List<PathfindingNode> closedList = new List<PathfindingNode>();

            openList.Add(new PathfindingNode(0, start));    //startpunkt in der Mitte platzieren
            map[offset][offset] = 0;
            do
            {
                PathfindingNode current = new PathfindingNode(-1, new Point(0,0));
                int i = 0;
                while(current.step < 0 && openList.Count > 0)
                {
                    current = openList.Find(x => x.step == i);  //Die Node mit den niedrigsten steps wird der Liste entnommen
                }

                if (current.position == destination)    //Falls der Zielpunkt erreicht wurde, wird der Pfad ausgegeben
                    return current.path();
                
                closedList.Add(current);

                for(int j = 0; i < 4; i++)
                {
                    int step = current.step + 1;
                    PathfindingNode.Direction direction = PathfindingNode.Direction.Up;
                    Point nextPosition = new Point(0,0);

                    //Nach und nach werden die verschiedenen successor erstellt
                    switch (j)
                    {
                        case 0 : 
                            direction = PathfindingNode.Direction.Up;
                            nextPosition = new Point(current.position.X, current.position.Y - 1);
                            break;
                        case 1 :
                            direction = PathfindingNode.Direction.Down;
                            nextPosition = new Point(current.position.X, current.position.Y + 1);
                            break;
                        case 2 :
                            direction = PathfindingNode.Direction.Right;
                            nextPosition = new Point(current.position.X + 1, current.position.Y);
                            break;
                        case 3 :
                            direction = PathfindingNode.Direction.Left;
                            nextPosition = new Point(current.position.X - 1, current.position.Y);
                            break;
                    }

                    if(MapUtils.isSpaceFree(nextPosition, tileMap))
                    {
                        PathfindingNode successor = new PathfindingNode(step, nextPosition);
                        //Sicherstellen, dass die neue Position in dem Bereich ist, der getestet wird
                        if(nextPosition.X - start.X >= offset * -1 && nextPosition.X - start.X < offset && nextPosition.Y - start.Y >= offset * -1 && nextPosition.Y - start.Y < offset)
                        {
                            //Teste, ob es bereits einen effektiveren Weg gibt
                            if(map[nextPosition.X - start.X + offset][nextPosition.Y - start.Y + offset] == -1 || map[nextPosition.X - start.X + offset][nextPosition.Y - start.Y + offset] > step)
                            {
                                current.add(successor, direction);
                                map[nextPosition.X - start.X + offset][nextPosition.Y - start.Y + offset] = step; //neuen niedrigsten weg zu dme Feld erstellen
                                openList.Add(successor);
                            }
                        }
                    }                    
                }

            } while (openList.Count > 0);

            return new Point[0];
        }
        //Überprüft, ob ein Feld frei ist
        public static bool isSpaceFree(Point position, TileMap map)
        {
            Tile tile = map.getTile(position.X, position.Y);
            if(tile.Solid || map.getEntities(position.X, position.Y).Count > 0)
                return false;
            else
                return true;
        }
        //Überprüft ob ein Feld frei ist, unter der berücksichtigung, dass sich Kreaturen durch verbündete bewegen können.
        public static bool isSpaceFree(Point position, TileMap map, Creature mover)
        {
            Tile tile = map.getTile(position.X, position.Y);
            //Ist das Tile solid
            if(tile.Solid)
                return false;
            //Ist das Feld leer
            else if (map.getEntities(position.X, position.Y).Count > 0)
            {
                //Sind die Entities auf dem Feld "feindlich" zur bewegenden kreatur
                if (map.getEntities(position.X, position.Y).Exists
                (
                    x => FightingInfo.IsFriendlyTo
                    (
                        Creature.BattleManager.fightingEntities.Find(y => y.Creature == x),
                        Creature.BattleManager.fightingEntities.Find(y => y.Creature == mover)
                    )
                ))
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return true;
        }

        public static Creature NearestEnemy(Creature current, TileMap map)
        {
            List<Creature> enemies = new List<Creature>();
            foreach(Entity e in map.Entities)
            {
                if (e is Creature)
                {
                    if (!FightingInfo.IsFriendlyTo(Creature.BattleManager.fightingEntities.Find(x => x.Creature == current), Creature.BattleManager.fightingEntities.Find(x => x.Creature == e)))
                    {
                        enemies.Add((Creature)e);
                    }
                }
            }
            if(enemies.Count == 0)
                return null;
            Creature nearest = enemies[0];
            int distance = ManhattenDistance(new Point((int)current.Position.X, (int)current.Position.Y), new Point((int)nearest.Position.X, (int)nearest.Position.Y));

            foreach(Creature e in enemies)
            {
                int temp = ManhattenDistance(new Point((int)current.Position.X, (int)current.Position.Y), new Point((int)e.Position.X, (int)e.Position.Y));
                if(temp < distance)
                {
                    distance = 0;
                    nearest = e;
                }
            }
            return nearest;
        }
    }
}