namespace BesmashContent
{
    public class HealAbility : DeffensiveAbility
    {
        public int HealAmount{get;set;}
        public HealAbility(Entity user, int cost, string name, int range, int healing) : base(user, cost, name, range)
        {
            HealAmount = healing;
            this.type = Type.heal;
        }

        public override void useAbility()
        {
            this.determineTarget();
            BattleEntity source = this.AbilityUser.battleManager.fightingEntities.Find(e => e.entity == this.AbilityUser);
            BattleEntity reciever = this.AbilityUser.battleManager.fightingEntities.Find(e => e.entity == this.target);

            AbilityUser.battleManager.heal(source, reciever, HealAmount);

            if(followUpOnFail || followUpOnSuccess)
                FollowUpAbility.useAbility();
        }
    }
}