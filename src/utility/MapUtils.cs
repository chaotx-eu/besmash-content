namespace BesmashContent.Utility {
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using System.Linq;
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

        /// param start - position on a TileMap
        /// param destination - position on a TileMap
        /// param maxDistance - radius from the start postion
        ///                     to search for the shortest path
        ///
        /// returns a collection of points representing the steps
        //          of the shortest path from start to destination
        //          or null if no path could be found
        ///
        /// constraints: destination must be within the radius
        ///             (maxDistance) from the start position
        public static Point[] shortestPath(Point start, Point destination, int maxDistance) {
            // int offset = maxDistance; //halbe Größe des Arrays (entfernung vom Mittelpunkt des Arrays (startpunkt) zu den Rändern)
            // warum nicht einfach maxDistance nutzen?

            //Nur ein kleiner Teil der gesamtmap wird abgebildet.
            //Bitte nur gerade Zahlen nehmen.
            //Die bisherigen mindestkosten zu den Feldern
            int[][] map;
            map  = new int[2*maxDistance+1][]; // immer variablen verwenden

            for(int x, y = 0; y < map.Length; y++) {
                map[y] = new int[2*maxDistance+1];

                for(x = 0; x < map.Length; x++)
                    map[y][x] = -1; //Standard Wert -1 um unbesuchte Felder zu markieren
            }

            List<PathfindingNode> openList = new List<PathfindingNode>();
            List<PathfindingNode> closedList = new List<PathfindingNode>();

            openList.Add(new PathfindingNode(0, start)); //startpunkt in der Mitte platzieren
            map[maxDistance][maxDistance] = 0;
            PathfindingNode current = null; // nicht unnötig funktion aufrufen (konstruktoren)
            int i = 0;

            while(openList.Count > 0) { // ist doch ne super Abbruchbedingung
                current = openList.Find(node => node.step == openList.Min(node2 => node2.step));

                if(current.position == destination) // Falls der Zielpunkt erreicht wurde, wird der Pfad ausgegeben
                    return current.path();

                openList.Remove(current); // kann sofort entfernt werden
                closedList.Add(current);

                for(i = 0; i < 4; i++) {
                    int step = current.step + 1;
                    PathfindingNode.Direction direction = PathfindingNode.Direction.Up;
                    Point nextPosition = Point.Zero; // den Punkt(0, 0) gibts schon

                    //Nach und nach werden die verschiedenen successor erstellt
                    switch (i) {
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

                    if(MapUtils.isSpaceFree(nextPosition)) {
                        PathfindingNode successor = new PathfindingNode(step, nextPosition);
                        //Sicherstellen, dass die neue Position in dem Bereich ist, der getestet wird
                        if(Math.Abs(nextPosition.X - start.X) < maxDistance
                        && Math.Abs(nextPosition.Y - start.Y) < maxDistance) {
                            //Teste, ob es bereits einen effektiveren Weg gibt
                            if(map[nextPosition.X - start.X + maxDistance][nextPosition.Y - start.Y + maxDistance] == -1
                            || map[nextPosition.X - start.X + maxDistance][nextPosition.Y - start.Y + maxDistance] > step) {
                                current.add(successor, direction);
                                map[nextPosition.X - start.X + maxDistance][nextPosition.Y - start.Y + maxDistance] = step; //neuen niedrigsten weg zu dme Feld erstellen
                                openList.Add(successor);
                            }
                        }
                    }                    
                }
            }

            return null;
        }

        public static bool isSpaceFree(Point position)
        {
            //todo
            return true;
        }
    }
}