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

        public void addStatus(Type type)
        {
            switch (type)
            {
                case Type.poison : this.poisoned = true; break;
                case Type.stun : this.stunned = true; break;
                case Type.sleep : this.asleep = true; break;
                case Type.invincible : this.invincible = true; break;
                case Type.dead : this.dead = true; break;
                case Type.immortal : this.immortal = true; break;
            }
        }

        public void removeStatus(Type type)
        {
            switch (type)
            {
                case Type.poison : this.poisoned = false; break;
                case Type.stun : this.stunned = false; break;
                case Type.sleep : this.asleep = false; break;
                case Type.invincible : this.invincible = false; break;
                case Type.dead : this.dead = false; break;
                case Type.immortal : this.immortal = false; break;
            }
        }
    }
}