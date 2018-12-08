namespace BesmashContent
{
    using System;
    using System.Collections.Generic;
    public class BattleManager
    {
        private List<BattleEntity> fightingEntities;   //Eine Liste in der alle am Kampf teilnehmenden Entities genau einmal enthalten sind
        public Random random;
        public BattleManager()
        {
            fightingEntities = new List<BattleEntity>();
        }

        //Berechnet die Reihenfolge in der die Kämpfenden Entitys agieren dürfen und arbeitet sie der Reihe nach ab
        public void ManageRound()
        {
            //Der Anfang der Runde
            for(int i = 0; i < fightingEntities.Count; i++)     //Arbeitet den Schaden durch gift ab
            {
                BattleEntity battleEntity = fightingEntities[i];
                if (battleEntity.entity.status.poisoned)
                    battleEntity.entity.isDealtDamage(12);
            }

            //Die einzelnen Aktionen der Kämpfer
            List<BattleEntity> fightingOrder = calculateFightingOrder(); //Berechnet die Reihenfolge in der die Entities
            for(int i = 0; i < fightingOrder.Count; i++)
            {
                if(!fightingOrder[i].entity.status.dead)
                    ManageTurn(fightingOrder[i]);
            }

            //Das Ende der Runde
        }

        public void ManageTurn(BattleEntity battleEntity) //Ein einzelner Zug
        {
            if(battleEntity.entity.status.stunned)
            {
                battleEntity.entity.status.stunned = false;
            }
            else if (battleEntity.entity.status.asleep)
            {
                battleEntity.entity.status.roundsAsleep++;
                if (random.Next(100) <= 10 * battleEntity.entity.status.roundsAsleep)
                {
                    battleEntity.entity.status.asleep = false;
                    battleEntity.entity.status.roundsAsleep = 0;
                }
            }
            else
                battleEntity.entity.nextTurn();
        }

        public void addToBattle(Entity entity)
        {
            BattleEntity battleEntity = new BattleEntity(entity);     //Erstellt eine BattleEntity, die auf die angegebene Entity verweist

            fightingEntities.Add(battleEntity);
        }

        public void removeFromBattle(Entity entity)
        {
            if(fightingEntities.Count > 0)
            {
                int i = 0;
                bool removed = false;
                do
                {
                    if(fightingEntities[i].entity == entity)
                    {
                        fightingEntities.RemoveAt(i);
                        removed = true;
                    }
                    i++;
                } while(!removed && i < fightingEntities.Count -1);
            }
        }

        public List<BattleEntity> calculateFightingOrder()
        {
            //Rechnet übriggebliebende agilität aus letzter Runde dazu
            for(int i = 0; i < fightingEntities.Count; i++)
            {
                BattleEntity battleEntity = fightingEntities[i];
                battleEntity.temporalAgility += battleEntity.stats.AGI + battleEntity.battleBuffs.AGI;
            }

            //Die Priorität wird auf 0 gesetzt
            for(int i = 0; i < fightingEntities.Count; i++)
            {
                BattleEntity entity = fightingEntities[i];
                entity.priority = 0;
            }

            //Bestimmt die Priorität
            for(int i = 0; i < fightingEntities.Count; i++)
            {
                BattleEntity entitiyI = fightingEntities[i];
                entitiyI.priority = 1;
                for(int j = 0; j < fightingEntities.Count; j++)
                {
                    BattleEntity entitiyJ = fightingEntities[j];

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
                BattleEntity entity = fightingEntities[i];
                entity.times = 0;
                while(entity.temporalAgility >= lowestAgility)
                {
                    entity.temporalAgility -= lowestAgility;
                    entity.times++;
                }
            }

            //Eine Liste wird erstellt, die der Reihenfolge in der die Entities nächste Runde agieren dürfen entspricht
            List<BattleEntity> fightingOrder = new List<BattleEntity>();
            bool wasAdded;
            do
            {
                wasAdded = false;                
                for (int i = 0; i < fightingEntities.Count; i++)
                {
                    for (int j = 0; j < fightingEntities.Count; j++)
                    {
                        BattleEntity entity = fightingEntities[j];
                        if(entity.priority == i && entity.times > 0)
                        {
                            fightingOrder.Add(entity);
                            entity.times--;
                            wasAdded = true;
                        }
                    }
                }
            } while (wasAdded);

            return fightingOrder;
        }

        public bool attack(BattleEntity attacker, BattleEntity defender, OffensiveAbility attack)
        {
            bool success = false;   //Rückgabe, ob der Angriff erfolgreich war oder nicht (eventuell nötig für irgendwas)
            float damageMultiplier; //Multiplikator errechnet sich aus den Werten von Angreifer und Verteidiger
            float dodgeRate = (attacker.stats.ACC + attacker.battleBuffs.ACC) / (defender.stats.DDG + defender.battleBuffs.DDG);
            if(attack.IsMagical)
                damageMultiplier = ((float)attacker.stats.MGA + attacker.battleBuffs.MGA) / ((float)defender.stats.MGD + defender.battleBuffs.MGD);
            else
                damageMultiplier = ((float)attacker.stats.ATK + attacker.battleBuffs.ATK) / ((float)defender.stats.DEF + defender.battleBuffs.DEF);

            if(random.Next(100) > dodgeRate * 100)
                return false;
            success = defender.entity.isDealtDamage((int) (damageMultiplier * attack.BaseDamage));
            
            if(defender.entity.status.dead)
                attacker.entity.onKill();

            return success;
        }
    }
}