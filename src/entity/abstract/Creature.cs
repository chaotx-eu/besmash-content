namespace BesmashContent
{
    public abstract class Creature : Movable
    {
        /// Creatures may wear armor...
        public Helmet Helmet {get; set;}
        public Chestplate Chestplate {get; set;}
        public Pants Pants {get; set;}

        /// ...and hold a weapon
        public Weapon Weapon {get; set;}

        public int MaxHP {get{return (BaseStats.VIT + StatsModifier.VIT) * 5;}}
        public int CurrentHP {get{return CurrentHP;} 
            set
            {
                CurrentHP = value;
                if(CurrentHP<=0 && !this.status.immortal)
                    status.dead = true;
                else if(CurrentHP > MaxHP)
                    CurrentHP = MaxHP;
            }}
        public int MaxAP{get;set;}
        public int CurrentAP{get{return CurrentAP;}set{CurrentAP = value; if(CurrentAP > MaxAP) CurrentAP = MaxAP;}}

        public Stats BaseStats {get; set;}
        public Stats StatsModifier {get; set;}
        public Ability[] abilities{get; set;}
        public Status status{get; set;}

        public BattleManager battleManager{get; set;} //Wird übergeben damit die Creatures nen überblick über das Schlachtfeld haben

        public Creature()
        {
            
        }
        public Creature(BattleManager manager)
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
        public virtual void onDamage() //Creature hat schaden erhalten
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
        public virtual void onHit() //Creature greift erfolgreich einen Gegner an
        {
            //Kann überschrieben werden.
        }
        public virtual void onKill() //Creature tötet erfolgreich einen Gegner
        {
            //Kann überschrieben werden.
        }

        public virtual void onDeath() //Creature ist gestorben
        {
            battleManager.removeFromBattle(this);
        }
    }
}