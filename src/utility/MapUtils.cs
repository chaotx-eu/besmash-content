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
                case(Facing.East):
                    rotated = new Point(-point.Y, point.X);
                    break;
                case(Facing.South):
                    rotated = new Point(-point.X, -point.Y);
                    break;
                case(Facing.West):
                    rotated = new Point(point.Y, -point.X);
                    break;
                default:
                    rotated = point;
                    break;
            }

            return rotated;
        }

        /// Rotates a rectangle and returns the result
        public static Rectangle rotateRectangle(Rectangle rectangle, int rotation) {
            Rectangle rotated = rectangle;
            rotated.X += rectangle.Width/2;
            rotated.Y += rectangle.Height/2;

            // TODO -> temporary solution: will not work properly
            // for rotations other than 0°, 90°, 180°, 270* or 360°
            if(rotation == 90 || rotation == 270) {
                rotated.Width = rectangle.Height;
                rotated.Height = rectangle.Width;
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

        /// The maximum layer for both layer dimensions
        public static int MaxLayer {get;} = 128;
        private static Dictionary<Type, int> LayerMap {get;}

        /// Initializes default layer levels
        static MapUtils() {
            // default layers [0 - MaxLayer]
            LayerMap = new Dictionary<Type, int>();
            LayerMap.Add(typeof(Tile), 1);
            LayerMap.Add(typeof(Entity), 2);
            LayerMap.Add(typeof(Creature), 3);
            LayerMap.Add(typeof(Player), 3);
            LayerMap.Add(typeof(Enemy), 3);
            LayerMap.Add(typeof(Projectile), 4);
            LayerMap.Add(typeof(Cursor), 5);
            LayerMap.Add(typeof(SpriteAnimation), 6);
        }

        /// Returns the default layer level for the passed game
        /// object dependent of its subtype. If no layer is defined
        /// for that type 0 will be returned
        public static int getLayer(Type type) {
            return !LayerMap.ContainsKey(type) ? 0
                : LayerMap[type];
        }
    }
}