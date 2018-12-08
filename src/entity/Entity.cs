namespace BesmashContent {
    public abstract class Entity : MapObject 
    {
        public int MaxHP {get{return (BaseStats.VIT + StatsModifier.VIT) * 5;}}
        public int CurrentHP {get{return CurrentHP;} set{CurrentHP = value; if(CurrentHP<=0 && !this.status.immortal) status.dead = true;}}
        public Stats BaseStats {get; set;}
        public Stats StatsModifier {get; set;}
        public Ability[] abilities{get; set;}
        public Status status{get; set;}

        public BattleManager battleManager{get; set;} //Wird übergeben damit die Entitys nen überblick über das Schlachtfeld haben

        public Entity()
        {
            
        }
        public Entity(BattleManager manager)
        {
            battleManager = manager;
        }

        public virtual void nextTurn()
        {
            //Do nothing by default
            
        }

        public bool isDealtDamage(int damage)
        {
            if (this.status.invincible)
                return false;
            
            this.onDamage();
            CurrentHP -= damage;
            return true;
        }
        
        //Bestimmte Events die überschrieben werde können um Effekte durch bestimmte taten auszulösen.
        public virtual void onDamage() //Entity hat schaden erhalten
        {
            /*
            Aufwachen, triggern von abilities etc.
            Will mir hier nur die Möglichkeit offen halten was einzubauen.
             */
            if(this.status.asleep)
            {
                if (battleManager.random.Next(100) <= (10 * status.roundsAsleep) + 35)
                {
                    status.asleep = false;
                    status.roundsAsleep = 0;
                }
            }
        }
        public virtual void onHit() //Entity greift erfolgreich einen Gegner an
        {
            //Kann überschrieben werden.
        }
        public virtual void onKill() //Entity tötet erfolgreich einen Gegner
        {
            //Kann überschrieben werden.
        }

        public virtual void onDeath() //Entity ist gestorben
        {
            battleManager.removeFromBattle(this);
        }
    }
}