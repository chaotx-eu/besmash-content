namespace BesmashContent
{
    public class Buff
    {
        public enum Type{VIT, ATK, MGA, DEF, MGD, AGI, ACC, DDG, SPD}

        private FightingInfo target{get;set;}
        private Type type{get;set;}
        public int roundsLeft{get;set;} //Set -1 for a permanent Buff
        public int turnsLeft{get;set;} //Set -1, for a Buff, that last's an entire Round
        private int Strength{get;set;}  //Negative Values for debuffs
        public bool Over{get;set;}

        public Buff(FightingInfo target, Type type, int rounds, int turns, int strength)
        {
            this.target = target;
            this.type = type;
            this.roundsLeft = rounds;
            this.turnsLeft = turns;
            this.Strength = Strength;   //Beispiele: 1 = +25%, 2 = +50%, 4 = +100%, -3 = -75%

            this.applyBuff();
        }

        public void updateTurn()
        {
            if(turnsLeft < 0)
                return;

            turnsLeft--;
            if(turnsLeft == 0)
            {
                this.removeBuff();
            }
        }

        public void updateRound()
        {
            if(roundsLeft < 0)
                return;

            roundsLeft--;
            if(roundsLeft == 0)
            {
                this.removeBuff();
            }
        }

        private void applyBuff()
        {
            switch (type)
            {
                case Type.VIT : target.stats.VITModifier += (0.25f * Strength); break;
                case Type.ATK : target.stats.ATKModifier += (0.25f * Strength); break;
                case Type.MGA : target.stats.MGAModifier += (0.25f * Strength); break;
                case Type.DEF : target.stats.DEFModifier += (0.25f * Strength); break;
                case Type.MGD : target.stats.MGDModifier += (0.25f * Strength); break;
                case Type.AGI : target.stats.AGIModifier += (0.25f * Strength); break;
                case Type.ACC : target.stats.ACCModifier += (0.25f * Strength); break;
                case Type.DDG : target.stats.DDGModifier += (0.25f * Strength); break;
                case Type.SPD : target.stats.SPDModifier += (0.25f * Strength); break;
            }

            Over = false;
        }
        private void removeBuff()
        {
            switch (type)
            {
                case Type.VIT : target.stats.VITModifier -= (0.25f * Strength); break;
                case Type.ATK : target.stats.ATKModifier -= (0.25f * Strength); break;
                case Type.MGA : target.stats.MGAModifier -= (0.25f * Strength); break;
                case Type.DEF : target.stats.DEFModifier -= (0.25f * Strength); break;
                case Type.MGD : target.stats.MGDModifier -= (0.25f * Strength); break;
                case Type.AGI : target.stats.AGIModifier -= (0.25f * Strength); break;
                case Type.ACC : target.stats.ACCModifier -= (0.25f * Strength); break;
                case Type.DDG : target.stats.DDGModifier -= (0.25f * Strength); break;
                case Type.SPD : target.stats.SPDModifier -= (0.25f * Strength); break;
            }

            Over = true;
        }
    }
}