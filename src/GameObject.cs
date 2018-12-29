namespace BesmashContent {
    using System;
    using System.Runtime.Serialization;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Content;

    /// All objects existing in the game universe
    /// inherit from this class.
    [DataContract(IsReference = true)]
    public class GameObject {
        /// Source SpriteSheet url.
        [DataMember]
        public string SpriteSheet {get; set;}

        /// Sprite Rectangle in SpriteSheet.
        [DataMember]
        public Rectangle SpriteRectangle {get; set;}

        /// Rotation of the Sprite.
        [DataMember]
        [ContentSerializer(Optional = true)]
        public float Rotation {get; set;} = 0;

        /// Drawing layer of this GameObject, default 0
        [DataMember]
        [ContentSerializer(Optional = true)] // TODO ignore causes crash
        public virtual float Layer {get; set;} = 0;

        /// Rectangle to draw to on the screen.
        [DataMember]
        [ContentSerializerIgnore]
        public Rectangle DestinationRectangle {get; set;}

        /// Color of this GameObject, default Color.White
        [DataMember]
        [ContentSerializerIgnore]
        public Color Color {get; set;} = Color.White;

        /// Reference to SpriteSheet image
        private Texture2D sheet;

        public void load(ContentManager manager) {
            sheet = manager.Load<Texture2D>(SpriteSheet);
        }

        /// Draws this object to the screen.
        /// The passed SpriteBatch will not be closed.
        public void draw(SpriteBatch batch) {
            Vector2 origin = new Vector2(
                SpriteRectangle.Width/2f,
                SpriteRectangle.Height/2f);
                
            batch.Draw(sheet, rotateRectangle(DestinationRectangle, (int)Rotation),
                SpriteRectangle, Color*TileMap.MapAlpha, Rotation*(float)Math.PI/180f,
                origin, SpriteEffects.None, Layer);
        }
        
        /// Updates this object.
        public virtual void update(GameTime time) {}

        // TODO move to own libary (BesmashUtil?)
        private Rectangle rotateRectangle(Rectangle rectangle, int rotation) {
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
    }
}