namespace BesmashContent
{
    //Speichert für jeden Status im Spiel ab, ob eine entity ihn hat.
    public class Status
    {
        public enum Type{poison, stun, sleep, invincible, dead, immortal};
        public bool poisoned {get; set;} //suffers damage at the start of each round
        public bool stunned {get; set;} //can't act for one round
        public bool asleep{get; set;}  //can't act until awoken
        public int roundsAsleep{get; set;} 
        public bool invincible{get; set;}  //does not recieve damage
        public bool dead{get; set;} //is dead (basically just a flag for the game, that it should remove this entity)
        public bool immortal {get; set;} //Doesn't die, when HP reach 0.

        public static void addStatus(Creature creature, Type type)
        {
            EffectManager.newInstance().addEffect(new EffectAnimation(parameters(type), creature.Position));
            switch (type)
            {
                case Type.poison : creature.status.poisoned = true; break;
                case Type.stun : creature.status.stunned = true; break;
                case Type.sleep : creature.status.asleep = true; break;
                case Type.invincible : creature.status.invincible = true; break;
                case Type.dead : creature.status.dead = true; break;
                case Type.immortal : creature.status.immortal = true; break;
            }
        }

        public static void removeStatus(Creature creature, Type type)
        {
            switch (type)
            {
                case Type.poison : creature.status.poisoned = false; break;
                case Type.stun : creature.status.stunned = false; break;
                case Type.sleep : creature.status.asleep = false; break;
                case Type.invincible : creature.status.invincible = false; break;
                case Type.dead : creature.status.dead = false; break;
                case Type.immortal : creature.status.immortal = false; break;
            }
        }
        public static EffectParameters parameters(Type type)
        {
            EffectParameters param = new EffectParameters("why this not work", new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 16), 8);
            switch (type)
            {
                case Type.poison : param.Sprite = "images/Effects/Status/Poison_Effect"; break;
                case Type.stun : param.Sprite = "images/Effects/Status/Stun_Effect"; break;
                case Type.sleep : param.Sprite = "images/Effects/Status/Sleep_Effect"; break;
                case Type.invincible : param.Sprite = "images/Effects/Status/Invincible_Effect"; break;
                case Type.dead : param.Sprite = "images/Effects/Status/Dead_Effect(?)"; break;
                case Type.immortal : param.Sprite = "images/Effects/Status/Immortal_Effect"; break;
            }
            return param;
        }
    }
}