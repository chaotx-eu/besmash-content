namespace BesmashContent {
    using Microsoft.Xna.Framework.Content;

    public class BasicAttack : Ability {
        public override void load(ContentManager content) {
            Title = "Attack";
            OnHitAnimation = content.Load<SpriteAnimation>(
                "objects/animations/sword_animation");

            base.load(content);
        }

        /// Shows animation regardless of enemies were hit
        public override void onHit(Creature target) {
            base.onHit(target);
            
            // TODO test
            if(target == null) OnHitAnimation
                .start(User.ContainingMap);
        }
    }
}