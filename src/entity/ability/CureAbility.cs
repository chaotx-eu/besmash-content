namespace BesmashContent
{
    public class CureAbility : DeffensiveAbility
    {
        public Status.Type StatusType{get;set;}
        public CureAbility() {} // TODO quick fix for content serializer
        public CureAbility(Creature user, int cost, string name, int range, Status.Type type) : base(user, cost, name, range)
        {
            StatusType = type;
            this.type = Type.cure;
        }
        public override void useAbility()
        {
            this.determineTarget();
            Status.removeStatus(target, StatusType);

            if(followUpOnFail || followUpOnSuccess)
                FollowUpAbility.useAbility();
        }
    }
}