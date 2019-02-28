namespace BesmashContent {
    using System;
    using System.Runtime.Serialization;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Utility;

    [DataContract(IsReference = true)]
    public class SpriteAnimation : MapObject, ICloneable {
        /// Total amount of sprites per row in this animation
        [DataMember]
        [ContentSerializer(Optional = true)]
        public int SpriteCount {get; set;}

        /// Size of a single sprite
        [DataMember]
        [ContentSerializer(Optional = true)]
        public Point SpriteSize {get; set;}

        /// Row index in the sprite sheet
        [DataMember]
        [ContentSerializer(Optional = true)]
        public int SpriteRow {get; set;}

        /// How many sprites are shown per second
        [DataMember]
        [ContentSerializer(Optional = true)]
        public int SpritesPerSecond {get; set;}

        /// How often this animation should be repeated.
        /// Values below 0 are interpreted as infinite
        [DataMember]
        [ContentSerializer(Optional = true)]
        public int MaxIterations {get; set;}

        /// Indicates wether the facing of the origin
        /// should be added to the set sprite row
        /// (North: 0, East: 1, South: 2, West: 3)
        [DataMember]
        [ContentSerializer(Optional = true)]
        public bool RowRelativeToFacing {get; set;}

        /// Indicates that this animations sprites should be
        /// rotated relative to the facing of another its
        /// origin (e.g. the user of an ability). It is adviced
        /// not to set this to true alongside RowPerFacing
        [DataMember]
        [ContentSerializer(Optional = true)]
        public bool RotateRelativeToFacing {get; set;}

        /// Wether this animation should stick to the position
        /// of its origin if any is defined
        [DataMember]
        [ContentSerializer(Optional = true)]
        public bool StickyPosition {get; set;}

        /// Animations may have an origin which is another
        /// map object, which may be used to determine this
        /// animations relative rotation and sprite row
        [DataMember]
        [ContentSerializerIgnore]
        public MapObject Origin {get; set;}

        /// The current iteration of this animation
        [DataMember]
        [ContentSerializerIgnore]
        public int Iterations {get; protected set;}

        /// The column index of the currently selected frame
        [DataMember]
        [ContentSerializerIgnore]
        public int CurrentFrame {get; protected set;}

        /// Wether this animation is currently running
        [DataMember]
        [ContentSerializerIgnore]
        public bool IsRunning {get; protected set;}

        /// Position offset
        [DataMember]
        [ContentSerializerIgnore]
        public Point Offset {get; set;} = Point.Zero;

        /// Event handler for when the animation has started
        public event AnimationStartedHandler AnimationStartedEvent;

        /// Event handler for when the animation has finished
        public event AnimationFinishedHandler AnimationFinishedEvent;

        private int column, row, timer;

        /// Starts the animation if contained by a map
        public void start() {
            if(ContainingMap != null) {
                CurrentFrame = column = timer = 0;
                spriteRotation = Rotation; // backup original rotation
                IsRunning = true; // moved to draw (TODO)
                onAnimationStarted();
                updateSprite();
                SpriteRectangle = new Rectangle(
                    SpriteSize.X*column, SpriteSize.Y*row,
                    SpriteSize.X, SpriteSize.Y
                );

                // TODO temp fix for non sticky animations
                if(Origin != null) Position = Origin.Position + MapUtils
                    .rotatePoint(Offset, Origin.Facing)
                    .ToVector2();
            }
        }

        public override void update(GameTime gameTime) {
            if(!IsRunning) return;
            updateSprite();
            base.update(gameTime);

            timer += gameTime.ElapsedGameTime.Milliseconds;
            if(timer > 1000f/SpritesPerSecond) {
                nextFrame();
                timer = 0;
            }
        }

        /// Stops this animation immediately
        public void stop() {
            ContainingMap.Animations.Remove(this);
            ContainingMap = null;
            IsRunning = false;
            onAnimationFinished();
        }

        /// Sets the sprite rectangle to the next
        /// frame in the sprite sheet or disables
        /// this animation in case there is none
        /// and the amount of iterations is greater
        /// than the allowed max iterations
        protected virtual void nextFrame() {
            ++CurrentFrame;
            if(++column >= SpriteCount) {
                if(++Iterations > MaxIterations && MaxIterations >= 0) {
                    stop();
                    return;
                } else column = 0;
            }

            SpriteRectangle = new Rectangle(
                SpriteSize.X*column, SpriteSize.Y*row,
                SpriteSize.X, SpriteSize.Y
            );
        }

        private float spriteRotation;
        private void updateSprite() {
            row = Math.Max(0, SpriteRow-1);
            if(Origin != null) {
                if(RowRelativeToFacing)
                    row += (int)Origin.Facing;

                if(RotateRelativeToFacing) {
                    int faceDiff = (int)Facing - (int)Origin.Facing;
                    Rotation = (spriteRotation + ((4 - faceDiff)%4)*90)%360;
                }

                if(StickyPosition) Position = Origin.Position + MapUtils
                    .rotatePoint(Offset, Origin.Facing)
                    .ToVector2();
            }
        }

        protected virtual void onAnimationStarted() {
            AnimationStartedHandler handler = AnimationStartedEvent;
            if(handler != null) handler(this, null);
        }

        protected virtual void onAnimationFinished() {
            AnimationFinishedHandler handler = AnimationFinishedEvent;
            if(handler != null) handler(this, null);
        }
    }
}