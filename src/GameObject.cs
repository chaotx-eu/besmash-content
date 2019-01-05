namespace BesmashContent {
    using System;
    using System.Runtime.Serialization;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Content;
    using Utility;

    /// All objects existing in the game universe
    /// inherit from this class.
    [DataContract(IsReference = true)]
    public class GameObject : ICloneable {
        /// Source SpriteSheet url.
        [DataMember]
        public string SpriteSheet {get; set;}

        /// Sprite Rectangle in SpriteSheet.
        [DataMember]
        [ContentSerializer(Optional = true)]
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

        /// Loads required resources for this object
        public virtual void load(ContentManager content) {
            sheet = content.Load<Texture2D>(SpriteSheet);
        }

        /// Draws this object to the screen.
        /// The passed SpriteBatch will not be closed.
        public void draw(SpriteBatch batch) {
            Vector2 origin = new Vector2(
                SpriteRectangle.Width/2f,
                SpriteRectangle.Height/2f);
                
            batch.Draw(sheet, MapUtils.rotateRectangle(DestinationRectangle, (int)Rotation),
                SpriteRectangle, Color*TileMap.MapAlpha, Rotation*(float)Math.PI/180f,
                origin, SpriteEffects.None, Layer);
        }
        
        /// Updates this object.
        public virtual void update(GameTime gameTime) {}

        public object clone() {
            return MemberwiseClone();
        }
    }
}