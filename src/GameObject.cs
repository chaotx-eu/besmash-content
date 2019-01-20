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
        [ContentSerializer(Optional = true)]
        public string SpriteSheet {get; set;}

        /// Path to thumbnail image
        [DataMember]
        [ContentSerializer(ElementName = "Thumbnail", Optional = true)]
        public string ThumbnailFile {get; set;}

        /// Sprite Rectangle in SpriteSheet.
        [DataMember]
        [ContentSerializer(Optional = true)]
        public Rectangle SpriteRectangle {get; set;}

        /// Rotation of the Sprite.
        [DataMember]
        [ContentSerializer(Optional = true)]
        public float Rotation {get; set;}

        /// Wether the sprite should be mirrored when drawn
        [DataMember]
        [ContentSerializer(Optional = true)]
        public bool Mirror {get; set;}

        /// Layers are seperated in two dimensions where
        /// the first one, the layer level, will be evaluated
        /// automatically dependent on the type of this object
        /// if not set manually (see MapUtils.getLayer(Type)).
        /// Only values between 0 and MapUtils.MaxLayer are supported
        [ContentSerializer(Optional = true)]
        public int LayerLevel {
            get {return layerLevel;}
            set {layerLevel = Math.Max(0, Math.Min(MapUtils.MaxLayer, value));}
        }

        [DataMember]
        private int layerLevel;

        /// Layer when drawn on a map. Will share a range
        /// of MapUtils.MaxLayer layers with objects on the
        /// same layer level (i.e. the same type by default)
        [DataMember]
        [ContentSerializer(Optional = true)]
        public int MapLayer {get; set;}

        /// Rectangle to draw to on the screen.
        [DataMember]
        [ContentSerializerIgnore]
        public Rectangle DestinationRectangle {get; set;}

        /// Color of this GameObject, default Color.White
        [DataMember]
        [ContentSerializerIgnore]
        public Color Color {get; set;} = Color.White;

        /// True layer this object is drawn to
        [ContentSerializerIgnore]
        public float Layer {get {
            return LayerLevel/
            (10f*MapUtils.MaxLayer)
                + MapLayer/(100f*MapUtils.MaxLayer);
        }}

        /// Reference to SpriteSheet image
        [ContentSerializerIgnore]
        public Texture2D Image {get; protected set;}

        /// Reference to thumbnail image
        [ContentSerializerIgnore]
        public Texture2D Thumbnail {get; set;}

        public GameObject() {
            layerLevel = MapUtils.getLayer(GetType());
        }

        /// Loads required resources for this object
        public virtual void load(ContentManager content) {
            // TODO overthink conditions
            // if(Image == null && SpriteSheet != null)
            if(SpriteSheet != null)
                Image = content.Load<Texture2D>(SpriteSheet);

            // if(ThumbnailFile != null && Thumbnail == null)
            if(ThumbnailFile != null)
                Thumbnail = content.Load<Texture2D>(ThumbnailFile);
            else // no exception handling cause to slow :/ (TODO think of something else)
                Thumbnail = content.Load<Texture2D>("images/world/entities/thumbnails/anonymous_thumb");
        }

        /// Draws this object to the screen.
        /// The passed SpriteBatch will not be closed.
        public virtual void draw(SpriteBatch batch) {
            if(Image == null) return;
            SpriteEffects effects = !Mirror ? SpriteEffects.None
                :   SpriteEffects.FlipHorizontally;

            Vector2 origin = new Vector2(
                SpriteRectangle.Width/2f,
                SpriteRectangle.Height/2f);
                
            batch.Draw(Image, MapUtils.rotateRectangle(DestinationRectangle, (int)Rotation),
                SpriteRectangle, Color*TileMap.MapAlpha, Rotation*(float)Math.PI/180f,
                origin, effects, Layer);
        }
        
        /// Updates this object.
        public virtual void update(GameTime gameTime) {}

        public object clone() {
            return MemberwiseClone();
        }
    }
}