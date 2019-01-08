namespace BesmashContent {
    using Microsoft.Xna.Framework;

    public interface IRoamingAI {
        /// Evaluates and returns the next point
        /// to be passed to the move command.
        /// Returns null if no move should happen
        /// which is different than returning
        /// Point.Zero in the way that a move of
        /// zero steps may still count as a move
        Point? nextMove();
    }
}