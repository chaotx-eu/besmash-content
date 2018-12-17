namespace BesmashContent
{
    public class Enemy : NPC
    {
        public int XP{get; set;}
        public Enemy(Stats stats, Ability[] abilities, FightingStyle style, int xp) : base(stats, abilities, style)
        {
            XP = xp;
        }

    }
}