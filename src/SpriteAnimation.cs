namespace BesmashContent {
    using System;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;

    public class SpriteAnimation : MapObject {
        /// Total amount of sprites per row in this animation
        public int SpriteCount {get; set;}

        /// Size of a single sprite
        public Point SpriteSize {get; set;}

        /// How many sprites are shown per second
        public int SpritesPerSecond {get; set;}

        /// Row index in the sprite sheet
        [ContentSerializer(Optional = true)]
        public int SpriteRow {get; set;}

        /// How often this animation should be repeated.
        /// Values below 0 are interpreted as infinite
        [ContentSerializer(Optional = true)]
        public int MaxIterations {get; set;}

        /// Path to the follow up animation file
        [ContentSerializer(ElementName = "FollowUpAnimation", Optional = true)]
        public string FollowUpAnimationFile {get; set;}

        /// An animation that is started once this finished
        [ContentSerializerIgnore]
        public SpriteAnimation FollowUpAnimation {get; set;}

        /// Animations may have a facing applied to them.
        /// This will have no effect on the sprite image
        /// use Rotate instead
        [ContentSerializerIgnore]
        public Facing Facing {get; set;}

        /// The amount of iterations since this animation
        /// started (in case this value was not modified)
        [ContentSerializerIgnore]
        public int Iterations {get; set;}

        /// Wether this animation is currently running
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
                column = timer = 0;
                IsRunning = true;
                onAnimationStarted();
            }
        }

        /// Loads required resources for this animation
        /// and its follow up animation if defined
        public override void load(ContentManager content) {
            base.load(content);

            if(FollowUpAnimationFile != null) {
                FollowUpAnimation = content
                    .Load<SpriteAnimation>(FollowUpAnimationFile);

                FollowUpAnimation.load(content);
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
            if(++column >= SpriteCount) {
                if(++Iterations > MaxIterations && MaxIterations >= 0) {
                    if(FollowUpAnimation != null) {
                        FollowUpAnimation.Position = Position;
                        ContainingMap.addAnimation(FollowUpAnimation);  
                    }

                    ContainingMap.Animations.Remove(this);
                    ContainingMap = null;
                    IsRunning = false;
                    onAnimationFinished();
                    return;
                } else column = 0;
            }

            SpriteRectangle = new Rectangle(
                SpriteSize.X*column, SpriteRow,
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