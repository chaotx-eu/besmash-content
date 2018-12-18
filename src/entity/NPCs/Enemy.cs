namespace BesmashContent
{
    public class Enemy : NPC
    {
        public int XP{get; set;}
        public Enemy() : base()
        {
            XP = 100;
        }
        public Enemy(Stats stats, Ability[] abilities, FightingStyle style, int xp) : base(stats, abilities, style)
        {
            XP = xp;
        }

    }
}