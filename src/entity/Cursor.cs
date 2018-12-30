namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using System.Collections.Generic;
    using System.Linq;
    using System;

    public class Cursor : Movable {
        /// Default cursor sprite sheet
        public static string DEFAULT_CURSOR_SHEET = "images/entities/generic_cursor";

        /// Default width in pixels of a single sprite
        /// in a cursor spritesheet
        public static int DEFAULT_SPRITE_W {get;} = 16;

        /// Default height in pixels of a single sprite
        /// in a cursor spritesheet
        public static int DEFAULT_SPRITE_H {get;} = 16;

        /// Default sprite count on horizontal pane
        public static int DEFAULT_SPRITE_C {get;} = 6;

        /// Default amount of sprites shown per second,
        /// SpritesPerSecond value divided by 2 is used
        /// for SpritesPerStep
        public static int DEFAULT_SPS {get;} = 3;

        [ContentSerializerIgnore]
        /// Wether the object beneath this cursor is considered
        /// selected. Resets on retreival back to false
        public bool IsSelected {
            get {
                bool value = isSelected;
                isSelected = false;
                return isSelected;
            }
            protected set {isSelected = value;}
        }
        
        private bool isSelected;

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

        /// Returns the first map object found below the cursor
        public MapObject getObject() {
            if(ContainingMap == null) return null;

            int x = (int)Position.X;
            int y = (int)Position.Y;
            List<Entity> entities = ContainingMap.getEntities(x, y);

            if(entities.Count > 1)
                return entities.Find(e => e != this);

            return ContainingMap.getTiles(x, y)
                .OrderByDescending(t => t.MapLayer)
                .FirstOrDefault();
        }

        /// Returns a list of entities below the cursor
        public List<Entity> getEntities() {
            return ContainingMap == null ? new List<Entity>()
                : ContainingMap.getEntities((int)Position.X, (int)Position.Y);
        }

        /// Marks this cursor as selected on its current spot
        public void select() {
            IsSelected = true;
        }

        public override void move(int distanceX, int distanceY, CollisionResolver resolve) {
            // cursor never collides but cannot get out of the viewport
            base.move(distanceX, distanceY, (x, y, mo) => {
                if(Math.Abs(Position.X + x - ContainingMap.BattleMapCenter.X) > ContainingMap.Viewport.X
                || Math.Abs(Position.Y + y - ContainingMap.BattleMapCenter.Y) > ContainingMap.Viewport.Y)
                    return Point.Zero;

                return null;
            });

            isSelected = false;
        }

        protected override void updateSprite(bool reset) {
            Facing = Facing.NORTH;
            base.updateSprite(false);
        }
    }
}