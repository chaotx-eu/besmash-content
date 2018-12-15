namespace BesmashContent
{
    public abstract class Ability
    {
        public enum Type {attack, move, heal, cure, buff};
        public Type type{get;set;}
        public Creature AbilityUser{get; set;}
        public int AbilityCost{get; set;}
        public string AbilityName{get; set;}
        public Ability FollowUpAbility{get; set;}
        public bool followUpOnSuccess{get; set;}
        public bool followUpOnFail{get; set;}
        public Ability (Creature user, int cost, string name)
        {
            AbilityUser = user;
            AbilityCost = cost;
            AbilityName = name;
        }
        public abstract void useAbility();
    }
}