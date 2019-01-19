namespace BesmashContent {
    using System;
    using System.Runtime.Serialization;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;

    [DataContract]
    public class SpriteAnimation : MapObject, ICloneable {
        /// Total amount of sprites per row in this animation
        [DataMember]
        [ContentSerializer(Optional = true)]
        public int SpriteCount {get; set;}

        /// Size of a single sprite
        [DataMember]
        [ContentSerializer(Optional = true)]
        public Point SpriteSize {get; set;}

        /// How many sprites are shown per second
        [DataMember]
        [ContentSerializer(Optional = true)]
        public int SpritesPerSecond {get; set;}

        /// Row index in the sprite sheet
        [DataMember]
        [ContentSerializer(Optional = true)]
        public int SpriteRow {get; set;}

        /// How often this animation should be repeated.
        /// Values below 0 are interpreted as infinite
        [DataMember]
        [ContentSerializer(Optional = true)]
        public int MaxIterations {get; set;}

        /// Indicates that this animations sprites should be
        /// rotated relative to the facing of another map
        /// object (e.g. the user of an ability)
        [DataMember]
        [ContentSerializer(Optional = true)]
        public bool RotateRelative {get; set;}

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

        /// Event handler for when the animation has started
        public event AnimationStartedHandler AnimationStartedEvent;

        /// Event handler for when the animation has finished
        public event AnimationFinishedHandler AnimationFinishedEvent;

        private int column, timer;

        /// Starts the animation if contained by a map
        public void start() {
            if(ContainingMap != null) {
                SpriteRectangle = new Rectangle(0, SpriteRow*SpriteSize.Y, SpriteSize.X, SpriteSize.Y);
                CurrentFrame = column = timer = 0;
                IsRunning = true;
                onAnimationStarted();
            }
        }

        public override void update(GameTime gameTime) {
            if(!IsRunning) return;
            base.update(gameTime);

            timer += gameTime.ElapsedGameTime.Milliseconds;
            if(timer > 1000f/SpritesPerSecond) {
                nextFrame();
                timer = 0;
            }
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
                    ContainingMap.Animations.Remove(this);
                    ContainingMap = null;
                    IsRunning = false;
                    onAnimationFinished();
                    return;
                } else column = 0;
            }

            SpriteRectangle = new Rectangle(
                SpriteSize.X*column, Math.Max(0, SpriteRow-1)*SpriteSize.Y,
                SpriteSize.X, SpriteSize.Y
            );
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