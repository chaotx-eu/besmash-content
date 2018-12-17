namespace BesmashContent
{
    using System;
    using System.Collections.Generic;
    public class BattleManager
    {
        private static BattleManager instance = null;
        public List<FightingInfo> fightingEntities;   //Eine Liste in der alle am Kampf teilnehmenden Entities genau einmal enthalten sind
        public Random random;
        public TileMap map;
        public static BattleManager newInstance()
        {
            if(instance == null)
                instance = new BattleManager();
            return instance;
        }

        private BattleManager()
        {
            fightingEntities = new List<FightingInfo>();
            random = new Random();
        }

        //Berechnet die Reihenfolge in der die Kämpfenden Creatures agieren dürfen und arbeitet sie der Reihe nach ab
        public void ManageRound()
        {
            //Der Anfang der Runde
            for(int i = 0; i < fightingEntities.Count; i++)  
            {
                FightingInfo FightingInfo = fightingEntities[i];

                if (FightingInfo.Creature.status.poisoned)    //Arbeitet den Schaden durch gift ab
                    FightingInfo.Creature.isDealtDamage(12);
                
                foreach(Buff b in FightingInfo.battleBuffs) //prüft ob buffs/debuffs ablaufen
                {
                    if (!b.Over)
                        b.updateRound();
                }
            }

            //Die einzelnen Aktionen der Kämpfer
            List<FightingInfo> fightingOrder = calculateFightingOrder(); //Berechnet die Reihenfolge in der die Entities
            for(int i = 0; i < fightingOrder.Count; i++)
            {
                if(!fightingOrder[i].Creature.status.dead)
                    ManageTurn(fightingOrder[i]);
            }

            //Das Ende der Runde
        }

        public void ManageTurn(FightingInfo FightingInfo) //Ein einzelner Zug
        {
            foreach(Buff b in FightingInfo.battleBuffs) //prüft ob buffs/debuffs ablaufen
            {
                if (!b.Over)
                    b.updateTurn();
            }
            
            if(FightingInfo.Creature.status.stunned)
            {
                FightingInfo.Creature.status.stunned = false;
            }
            else if (FightingInfo.Creature.status.asleep)
            {
                FightingInfo.Creature.status.roundsAsleep++;
                if (random.Next(100) <= 10 * FightingInfo.Creature.status.roundsAsleep)
                {
                    FightingInfo.Creature.status.asleep = false;
                    FightingInfo.Creature.status.roundsAsleep = 0;
                }
            }
            else
                FightingInfo.Creature.nextTurn();
        }

        public void addToBattle(Creature Creature, FightingInfo.Faction alignment)
        {
            FightingInfo FightingInfo = new FightingInfo(Creature, alignment);     //Erstellt eine FightingInfo, die auf die angegebene Creature verweist

            fightingEntities.Add(FightingInfo);
        }

        public void removeFromBattle(Creature Creature)
        {
            if(fightingEntities.Count > 0)
            {
                int i = 0;
                bool removed = false;
                do
                {
                    if(fightingEntities[i].Creature == Creature)
                    {
                        fightingEntities.RemoveAt(i);
                        removed = true;
                    }
                    i++;
                } while(!removed && i < fightingEntities.Count -1);
            }
        }

        public List<FightingInfo> calculateFightingOrder()
        {
            //Rechnet übriggebliebende agilität aus letzter Runde dazu
            for(int i = 0; i < fightingEntities.Count; i++)
            {
                FightingInfo FightingInfo = fightingEntities[i];
                FightingInfo.temporalAgility += FightingInfo.stats.AGI;
            }

            //Die Priorität wird auf 0 gesetzt
            for(int i = 0; i < fightingEntities.Count; i++)
            {
                FightingInfo Creature = fightingEntities[i];
                Creature.priority = 0;
            }

            //Bestimmt die Priorität
            for(int i = 0; i < fightingEntities.Count; i++)
            {
                FightingInfo entitiyI = fightingEntities[i];
                entitiyI.priority = 1;
                for(int j = 0; j < fightingEntities.Count; j++)
                {
                    FightingInfo entitiyJ = fightingEntities[j];

                    if(i != j)
                    {
                        if(entitiyI.temporalAgility < entitiyJ.temporalAgility)
                        {
                            entitiyI.priority++;
                        }
                        else if(entitiyI.temporalAgility == entitiyJ.temporalAgility)
                        {
                            if (entitiyJ.priority == 0)
                                entitiyI.priority++;
                        }
                    }
                }
            }

            //Nimmt sich die geschwindigkeit des niedrigsten
            int lowestAgility = fightingEntities.Find(x => x.priority == fightingEntities.Count).priority;

            //Es wird berechnet wie oft jeder angreifen kann (n mal so oft, wie die eigene AGI höher ist als die des langsamsten)
            for(int i = 0; i < fightingEntities.Count; i++)
            {
                FightingInfo Creature = fightingEntities[i];
                Creature.times = 0;
                while(Creature.temporalAgility >= lowestAgility)
                {
                    Creature.temporalAgility -= lowestAgility;
                    Creature.times++;
                }
            }

            //Eine Liste wird erstellt, die der Reihenfolge in der die Entities nächste Runde agieren dürfen entspricht
            List<FightingInfo> fightingOrder = new List<FightingInfo>();
            bool wasAdded;
            do
            {
                wasAdded = false;                
                for (int i = 0; i < fightingEntities.Count; i++)
                {
                    for (int j = 0; j < fightingEntities.Count; j++)
                    {
                        FightingInfo Creature = fightingEntities[j];
                        if(Creature.priority == i && Creature.times > 0)
                        {
                            fightingOrder.Add(Creature);
                            Creature.times--;
                            wasAdded = true;
                        }
                    }
                }
            } while (wasAdded);

            return fightingOrder;
        }

        public bool attack(FightingInfo attacker, FightingInfo defender, OffensiveAbility attack)
        {
            bool success = false;   //Rückgabe, ob der Angriff erfolgreich war oder nicht (eventuell nötig für irgendwas)
            float damageMultiplier; //Multiplikator errechnet sich aus den Werten von Angreifer und Verteidiger
            float dodgeRate = (attacker.stats.ACC) / (defender.stats.DDG);
            if(attack.IsMagical)
                damageMultiplier = ((float)attacker.stats.MGA) / ((float)defender.stats.MGD);
            else
                damageMultiplier = ((float)attacker.stats.ATK) / ((float)defender.stats.DEF);

            if(random.Next(100) > dodgeRate * attack.BaseAcc)
                return false;
            success = defender.Creature.isDealtDamage((int) (damageMultiplier * attack.BaseDamage));
            
            if(defender.Creature.status.dead)
                attacker.Creature.onKill();

            return success;
        }

        public void heal(FightingInfo source, FightingInfo reciever, int amount)
        {
            reciever.Creature.CurrentHP += amount;
        }
    }
}