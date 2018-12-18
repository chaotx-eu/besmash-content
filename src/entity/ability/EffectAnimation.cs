namespace BesmashContent
{
    using Microsoft.Xna.Framework;
    
    public struct EffectParameters
    {
        public string Sprite{get;set;}
        public Rectangle Rectangle{get;set;}
        public int Frames{get;set;}
        public EffectParameters(string sprite, Rectangle rectangle, int frames)
        {
            Sprite = sprite;
            Rectangle = rectangle;
            Frames = frames;
        }
    }
    public class EffectAnimation : Entity
    {
        private EffectManager effectManager{get;set;}
        public Microsoft.Xna.Framework.GameTime startingTime{get;set;}
        public int Frames{get;set;}
        private int currentFrame = 0;
        public bool running{get;set;}

        public EffectAnimation() {} // TODO quick hotfix for content serializer
        public EffectAnimation(string sheet, Rectangle rectangle, int frames, Vector2 position)
        {
            this.SpriteSheet = sheet;
            this.SpriteRectangle = rectangle;
            this.Frames = frames;
            this.Position = position;

            effectManager = EffectManager.newInstance();
        }
        public EffectAnimation(EffectParameters param, Vector2 position) : this(param.Sprite, param.Rectangle, param.Frames, position){}
        
        private int fps = 4;
        private int timer;
        public override void update(Microsoft.Xna.Framework.GameTime time)
        {
            base.update(time);
            timer += time.ElapsedGameTime.Milliseconds;

            if(timer > 1000f/fps) {
                this.updateSprite();
                if(running)
                {
                    currentFrame++;
                    if(currentFrame > Frames)
                        effectManager.removeEffect(this);
                }

                timer = 0;
            }
        }

        protected virtual void updateSprite()
        {
            int w = SpriteRectangle.Width;
            int h = SpriteRectangle.Height;
            int x = 16 * currentFrame;
            int y = running ? 0 : 16;
            SpriteRectangle = new Rectangle( x, y, w, h);
        }
        public void startEffect()
        {
            running = true;
        }
    }
}