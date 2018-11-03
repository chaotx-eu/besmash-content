namespace BesmashContent
{
    using System.Collections.Generic;
    public class BattleManager
    {
        private List<BattleEntity> fightingEntities;   //Eine Liste in der alle am Kampf teilnehmenden Entities genau einmal enthalten sind

        public BattleManager()
        {
            fightingEntities = new List<BattleEntity>();
        }

        public void addToBattle(Entity entity)
        {
            BattleEntity battleEntity = new BattleEntity();     //Erstellt eine BattleEntity, die auf die angegebene Entity verweist
            battleEntity.entity = entity;
            battleEntity.priority = 0;
            battleEntity.times = 0;
            battleEntity.temporalAgility = 0;

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
                battleEntity.temporalAgility += battleEntity.entity.Stats.AGI;
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
    }
}