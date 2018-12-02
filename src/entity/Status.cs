namespace BesmashContent
{
    //Speichert für jeden Status im Spiel ab, ob eine entity ihn hat.
    public class Status
    {
        public bool poisoned {get; set;} //suffers damage at the start of each round (todo)
        public bool stunned {get; set;} //can't act for one round  (todo)
        public bool asleep{get; set;}  //can't act until awoken (todo)
        public bool invincible{get; set;}  //does not recieve damage
        public bool dead{get; set;} //is dead (basically just a flag for the game, that it should remove this entity)
        public bool immortal {get; set;} //Doesn't die, when HP reach 0.
    }
}