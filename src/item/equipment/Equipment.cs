namespace BesmashContent {
    using Microsoft.Xna.Framework.Content;

    public class Equipment : Item {
        /// Additional stats which will be added
        /// to the users stats wearing this equipment
        [ContentSerializer(Optional = true)]
        public Stats Stats {get; set;}

        /// Creates a new equipment
        /// object with default stats
        public Equipment() {
            Stats = new Stats();
        }
    }
}