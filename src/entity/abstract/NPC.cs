namespace BesmashContent
{
    using System.Collections.Generic;
    using Microsoft.Xna.Framework;
    using BesmashContent.Utility;
    public abstract class NPC : Creature
    {
        public enum FightingStyle{Meelee, Ranged, Mixed, Support, Passive}
        public struct AIParameters
        {
            public FightingStyle style{get;set;}
            public Ability mainAttack{get;set;}
            public Ability startingAttack{get;set;}
            public int aggression{get;set;} //1-10. 10: always attacks for max damage, does not care for AP or Effects. 1: Tries to stay at max HP and AP. Only attacks, rarely. TO DO

            public List<OffensiveAbility> ranged{get;set;}
            public List<OffensiveAbility> meelee{get;set;}
            public List<MovementAbility> move{get;set;}
            public List<BuffAbility> buff{get;set;}
    //todo  public List<Ability> debuff{get;set;}
            public List<HealAbility> heal{get;set;}
            
            public List<Ability> usedAbilities{get;set;}

            public AIParameters(FightingStyle style, Ability mainAttack, Ability startingAttack, int aggression, Ability[] abilities)
            {
                this.style = style;
                this.mainAttack = mainAttack;
                this.startingAttack = startingAttack;
                this.aggression = aggression;

                ranged = new List<OffensiveAbility>();
                meelee = new List<OffensiveAbility>();
                move = new List<MovementAbility>();
                buff = new List<BuffAbility>();
                heal = new List<HealAbility>();

                usedAbilities = new List<Ability>();

                foreach(Ability a in abilities)
                {
                    switch (a.type)
                    {
                        case Ability.Type.attack : 
                            if(((OffensiveAbility)a).maxRange == 1)
                                meelee.Add((OffensiveAbility)a);
                            else
                                ranged.Add((OffensiveAbility)a);
                            break;
                        case Ability.Type.move : move.Add((MovementAbility)a); break;
                        case Ability.Type.buff : buff.Add((BuffAbility)a); break;
                        case Ability.Type.heal : heal.Add((HealAbility)a); break;
                    }
                }
            }
            public void reCalculate(Ability[] abilities)
            {
                foreach(Ability a in abilities)
                {
                    switch (a.type)
                    {
                        case Ability.Type.attack : 
                            if(((OffensiveAbility)a).maxRange == 1)
                                meelee.Add((OffensiveAbility)a);
                            else
                                ranged.Add((OffensiveAbility)a);
                            break;
                        case Ability.Type.move : move.Add((MovementAbility)a); break;
                        case Ability.Type.buff : buff.Add((BuffAbility)a); break;
                        case Ability.Type.heal : heal.Add((HealAbility)a); break;
                    }
                }
            }
        }

        public AIParameters AI{get;set;}


        public NPC(BattleManager manager, Stats stats, Ability[] abilities, FightingStyle style) : base(manager, stats, abilities)
        {
            AI = new AIParameters(style, null, null, 5, abilities);
        }
        public Ability nextMove()
        {
            Ability toUse = null;
            switch (AI.style)
            {
                case FightingStyle.Meelee : 
                    if(MapUtils.ManhattenDistance(this.Position, MapUtils.NearestEnemy(this, battleManager.map).Position) == 1)
                    {
                        List<FightingInfo> possibleTargets = new List<FightingInfo>();
                        foreach(FightingInfo info in battleManager.fightingEntities)
                        {
                            if(MapUtils.ManhattenDistance(this.Position, info.Creature.Position) == 1 && !FightingInfo.IsFriendlyTo(battleManager.fightingEntities.Find(x => x.Creature == this), info))
                                possibleTargets.Add(info);
                        }
                        if(AI.heal.Count > 0)
                        {
                            if((float)MaxHP / (float)CurrentHP < 0.33)
                            {
                                if(AI.heal.Exists(x => x.AbilityCost < CurrentAP))
                                {                                            
                                    int missingHP = MaxHP - CurrentHP;
                                    int leastOverheal = MaxHP * -1;
                                    foreach(HealAbility h in AI.heal)
                                    {
                                        if(CurrentAP >= h.AbilityCost)
                                        {
                                            if((leastOverheal < 0 && h.HealAmount - missingHP > leastOverheal)|| h.HealAmount - missingHP < leastOverheal)
                                            {
                                                toUse = h;
                                                leastOverheal = h.HealAmount - missingHP;
                                            }
                                        }
                                    }
                                    ((HealAbility)toUse).target = this;
                                }
                            }
                        }
                        else
                        {
                            if(possibleTargets.Count > 0)
                            {
                                
                                if(AI.mainAttack.type == Ability.Type.attack && AI.mainAttack.AbilityCost <= CurrentAP)
                                {
                                    if(AI.usedAbilities.FindAll(x => x == AI.mainAttack).Count < (float)AI.usedAbilities.Count * 1.0f / (11.0f - AI.aggression))
                                        toUse = AI.mainAttack;
                                }
                                if(AI.meelee.Exists(x => x.AbilityCost < CurrentAP))
                                {
                                    while (toUse == null)
                                    {
                                        //Zufällig nen move wählen
                                        int i = battleManager.random.Next(AI.meelee.Count);
                                        if(AI.meelee[i].AbilityCost <= CurrentAP)
                                            toUse = AI.meelee[i];
                                    }  
                                    //Überprüfe, ob das Ziel von letzter Runde noch in Reichweite ist
                                    if(AI.usedAbilities[AI.usedAbilities.Count - 1] is OffensiveAbility)
                                    {
                                        if(possibleTargets.Exists(x => ((OffensiveAbility)AI.usedAbilities[AI.usedAbilities.Count - 1]).target == x.Creature))
                                        {
                                            ((OffensiveAbility)toUse).target = ((OffensiveAbility)AI.usedAbilities[AI.usedAbilities.Count - 1]).target;
                                        }
                                    }
                                    else
                                    {
                                        //Zufälliges Ziel wählen
                                        int i = battleManager.random.Next(AI.meelee.Count);
                                        ((OffensiveAbility)toUse).target = possibleTargets[i].Creature;
                                    }
                                }
                            }
                        }
                        if(toUse == null)
                        {
                            if(AI.move != null)
                            {
                                if(possibleTargets.Count > 0)
                                {
                                    //buffen oder so
                                }
                                else
                                {
                                    //zu wem will ich gehen?
                                    foreach(FightingInfo info in battleManager.fightingEntities)
                                    {
                                        if(!FightingInfo.IsFriendlyTo(battleManager.fightingEntities.Find(x => x.Creature == this), info))
                                        {
                                            possibleTargets.Add(info);
                                        }
                                    }
                                    List<Vector2> possibleSpaces = new List<Vector2>();
                                    //Welche flecken sind Frei?
                                    foreach(FightingInfo info in possibleTargets)
                                    {
                                        for(int i = 0; i < 4; i++)
                                        {
                                            Vector2 nextSpace = info.Creature.Position;
                                            switch (i)
                                            {
                                                case 0 : nextSpace.Y--; break;
                                                case 1 : nextSpace.Y++; break;
                                                case 2 : nextSpace.X--; break;
                                                case 3 : nextSpace.X++; break;
                                            }
                                            if(MapUtils.isSpaceFree(new Point((int)nextSpace.X, (int)nextSpace.Y), battleManager.map))
                                                possibleSpaces.Add(nextSpace);
                                        }
                                    }
                                    int min = 255;
                                    Vector2 target = new Vector2(0.0f, 0.0f);
                                    foreach(Vector2 space in possibleSpaces)
                                    {
                                        Point[] path = MapUtils.shortestPath(this.Position, space, battleManager.fightingEntities.Find(x => x.Creature == this).stats.AGI, battleManager.map);

                                        if(path.Length < min)
                                        {
                                            target = space; //Feld gefunden
                                            min = path.Length;
                                        }
                                    }
                                    if(AI.move.Exists(x => x.AbilityCost <= CurrentAP))
                                    {
                                        if(AI.move.Exists(x => x.AbilityCost <= CurrentAP && x.maxDistance >= min))
                                        {
                                            toUse =  AI.move.Find(x => x.AbilityCost <= CurrentAP && x.maxDistance >= min);
                                            ((MovementAbility)toUse).destination = new Point((int)target.X, (int)target.Y);
                                        }
                                    }
                                }
                            }
                        }
                    } break;
                
            }
            
            AI.usedAbilities.Add(toUse);
            return toUse;
        }
    }
}