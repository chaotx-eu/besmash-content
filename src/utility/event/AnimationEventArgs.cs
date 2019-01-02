namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using System;

    public class AnimationEventArgs : EventArgs {
        public Point Position {get; protected set;}
        public Facing Facing {get; protected set;}

        public AnimationEventArgs(Point position, Facing facing) {
            Position = position;
            Facing = facing;
        }
    }

    public delegate void AnimationStartedHandler(SpriteAnimation sender, AnimationEventArgs args);
    public delegate void AnimationFinishedHandler(SpriteAnimation sender, AnimationEventArgs args);
}