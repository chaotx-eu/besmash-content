namespace BesmashContent
{
    public class HealAbility : DeffensiveAbility
    {
        public int HealAmount{get;set;}
        public HealAbility(Creature user, int cost, string name, int range, int healing) : base(user, cost, name, range)
        {
            HealAmount = healing;
            this.type = Type.heal;
        }

        public override void useAbility()
        {
            this.determineTarget();
            FightingInfo source = this.AbilityUser.battleManager.fightingEntities.Find(e => e.Creature == this.AbilityUser);
            FightingInfo reciever = this.AbilityUser.battleManager.fightingEntities.Find(e => e.Creature == this.target);

            AbilityUser.battleManager.heal(source, reciever, HealAmount);

            if(followUpOnFail || followUpOnSuccess)
                FollowUpAbility.useAbility();
        }
    }
}