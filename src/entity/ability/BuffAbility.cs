namespace BesmashContent
{
    public class BuffAbility : DeffensiveAbility
    {
        public Buff.Type BuffType{get;set;}
        public int roundDuration{get;set;}  //-1 for permanent
        public int turnDuration{get;set;}   //-1 to only count rounds
        public int BuffStrength{get;set;}   //multiple of 25% bonus
        public BuffAbility(Creature user, int cost, string name, int range, Buff.Type type, int rounds, int turns, int strength) : base(user, cost, name, range)
        {
            BuffType = type;
            roundDuration = rounds;
            turnDuration = turns;
            BuffStrength = strength;
            this.type = Type.buff;
        }
        public override void useAbility()
        {
            this.determineTarget();
            FightingInfo reciever = Creature.BattleManager.fightingEntities.Find(e => e.Creature == this.target);
            reciever.battleBuffs.Add(new Buff(reciever, BuffType, roundDuration, turnDuration, BuffStrength));

            if(followUpOnFail || followUpOnSuccess)
                FollowUpAbility.useAbility();
        }
    }
}