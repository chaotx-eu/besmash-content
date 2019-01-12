namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using System.Collections.Generic;
    using System.Linq;
    using System;

    public class Cursor : Movable {
        /// Default cursor sprite sheet
        public static string DEFAULT_CURSOR_SHEET = "images/world/entities/cursor/generic_cursor";

        /// Wether the object beneath this cursor is considered
        /// selected. Resets back to false on retreival
        [ContentSerializerIgnore]
        public bool IsSelected {
            get {
                bool value = isSelected;
                isSelected = false;
                return value;
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
            SpritesPerSecond = DEFAULT_SPS*2;
            SpritesPerStep = DEFAULT_SPS;
            StepTime = 200;
            CollisionResolver = (x, y, mo) => {
                isSelected = false;
                if(Math.Abs(Position.X + x - ContainingMap.BattleMap.Position.X) > ContainingMap.Viewport.X
                || Math.Abs(Position.Y + y - ContainingMap.BattleMap.Position.Y) > ContainingMap.Viewport.Y)
                    return Point.Zero;

                return null;
            };
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

        protected override void updateSprite(bool reset) {
            Facing = Facing.North;
            base.updateSprite(false);
        }
    }
}