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
    }
}