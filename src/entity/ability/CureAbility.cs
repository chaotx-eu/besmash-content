namespace BesmashContent
{
    public class CureAbility : DeffensiveAbility
    {
        public Status.Type StatusType{get;set;}
        public CureAbility(Creature user, int cost, string name, int range, Status.Type type) : base(user, cost, name, range)
        {
            StatusType = type;
            this.type = Type.cure;
        }
        public override void useAbility()
        {
            this.determineTarget();
            target.status.removeStatus(StatusType);

            if(followUpOnFail || followUpOnSuccess)
                FollowUpAbility.useAbility();
        }
    }
}