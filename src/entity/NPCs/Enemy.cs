namespace BesmashContent
{
    public class Enemy : NPC
    {
        public int XP{get; set;}
        private Group group;
        public Group Group 
        {
            get{return group;}
            set
            {
                if(value != null && !value.getMember().Exists(x => x == this))
                    value.addMember(this);
                group = value;
            }
        }

        public Enemy() : this(new Stats(), null, FightingStyle.Passive, 0, null){}
        public Enemy(Stats stats, Ability[] abilities, FightingStyle style, int xp, Group friendsAndStuffOrSo) : base (stats, abilities, style)
        {
            XP = xp;
            Group = friendsAndStuffOrSo;
        }
    }
}