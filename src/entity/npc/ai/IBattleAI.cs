namespace BesmashContent {
    public interface IBattleAI : IRoamingAI {
        /// Evaluates and returns the next ability
        /// to be executed. Returns null if no
        /// ability should be executed
        Ability nextAbility();
    }
}