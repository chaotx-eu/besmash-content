namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using System.Collections.Generic;
    using System;

    /// Custom EventArgs for MoveEvents.
    public class MoveEventArgs : EventArgs {
        public Point Position {get;}
        public Point Target {get;}

        public MoveEventArgs(Point position, Point target) {
            Position = position;
            Target = target;
        }
    }

    /// Defines the signature of a MoveStartedEvent handler.
    public delegate void MoveStartedHandler(Movable sender, MoveEventArgs args);

    /// Defines the signature of a MoveFinishedEvent handler.
    public delegate void MoveFinishedHandler(Movable sender, MoveEventArgs args);

    /// Defines how to react on collision with another MapObject.
    /// Where distanceX and distanceY are the originally planned
    /// movement values and others is a list of MapObjects this
    /// object would collide with. As return value a Point is
    /// expected representing the new movement values to be taken
    /// instead or null if nothing should happen (i.e. no collision)
    public delegate Point? CollisionResolver(int distanceX, int distanceY, List<MapObject> others);
}