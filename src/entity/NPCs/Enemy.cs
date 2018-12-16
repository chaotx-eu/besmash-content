namespace BesmashContent
{
    public class Enemy : NPC
    {
        public int XP{get; set;}
        public Enemy(BattleManager manager, Stats stats, Ability[] abilities, FightingStyle style, int xp) : base(manager, stats, abilities, style)
        {
            XP = xp;
        }

    }
}