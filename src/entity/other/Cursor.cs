namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using System;

    public class Cursor : Movable {
        /// default cursor sprite sheet
        public static string DEFAULT_CURSOR_SHEET = "images/entities/generic_cursor";

        /// default width in pixels of a single sprite
        /// in a cursor spritesheet
        public static int DEFAULT_SPRITE_W {get;} = 16;

        /// default height in pixels of a single sprite
        /// in a cursor spritesheet
        public static int DEFAULT_SPRITE_H {get;} = 16;

        /// default sprite count on horizontal pane
        public static int DEFAULT_SPRITE_C {get;} = 6;

        /// default amount of sprites shown per second,
        /// SpritesPerSecond value divided by 2 is used
        /// for SpritesPerStep
        public static int DEFAULT_SPS {get;} = 3;

        public Cursor() : this(Point.Zero) {}
        public Cursor(Point position) : this(position, DEFAULT_CURSOR_SHEET) {}
        public Cursor(Point position, string spriteSheet) {
            Position = new Vector2(position.X, position.Y);
            SpriteSheet = spriteSheet;
            SpriteRectangle = new Rectangle(0, 0, DEFAULT_SPRITE_W, DEFAULT_SPRITE_H);
            SpriteCount = DEFAULT_SPRITE_C;
            SpritesPerSecond = DEFAULT_SPS;
            SpritesPerStep = SpritesPerSecond/2;
            StepTime = 200;
        }

        public override void move(int distanceX, int distanceY, CollisionResolver resolve) {
            // cursor never collides but cannot get out of the viewport
            base.move(distanceX, distanceY, (x, y, mo) => {
                if(Math.Abs(Position.X + x - ContainingMap.BattleMapCenter.X) > ContainingMap.Viewport.X
                || Math.Abs(Position.Y + y - ContainingMap.BattleMapCenter.Y) > ContainingMap.Viewport.Y)
                    return Point.Zero;

                return null;
            });
        }

        protected override void updateSprite(bool reset) {
            Facing = Facing.NORTH;
            base.updateSprite(false);
        }
    }
}