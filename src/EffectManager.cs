namespace BesmashContent
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    public class EffectManager
    {
        private static EffectManager instance = null;
        public List<EffectAnimation> effectList{get;set;}
        public static EffectManager newInstance()
        {
            if(instance == null)
                instance = new EffectManager();
            return instance;
        }

        private EffectManager()
        {
            effectList = new List<EffectAnimation>();
        }
        public void addEffect(EffectAnimation animation)
        {
            effectList.Add(animation);
            if(!effectList.Exists(x => x.running == true))
                this.next();
        }
        public static void addEffect(EffectParameters parameters, Vector2 position)
        {
            EffectAnimation animation = new EffectAnimation(parameters, position);
            newInstance().addEffect(animation);
            BattleUtils.newInstance().map.addEntity(animation);
        }
        public void removeEffect(EffectAnimation animation)
        {
            effectList.Remove(animation);
            BattleUtils.newInstance().map.removeEntity(animation);
            if(effectList.Count > 0)
                this.next();
        }
        public void next()
        {
            EffectAnimation nextEffect = effectList.Find(x => x.running == false);
            if(nextEffect != null)
                nextEffect.running = true;
        }
    }
}