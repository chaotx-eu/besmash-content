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

        private int currentHP;
        public int CurrentHP {
            get {return currentHP;} 
            set {
                currentHP = value;
                if(currentHP<=0 && !this.status.immortal)
                    status.dead = true;
                else if(currentHP > MaxHP)
                    currentHP = MaxHP;
            }
        }

        public int MaxAP{get;set;}

        private int currentAP;
        public int CurrentAP {
            get {return currentAP;}
            set {
                currentAP = value;
                if(currentAP > MaxAP) currentAP = MaxAP;
            }
        }

        public Stats BaseStats {get; set;}
        public Stats StatsModifier {get; set;}
        public Ability[] Abilities{get; set;}
        public Status status{get; set;}

        public BattleManager battleManager{get; set;} //Wird übergeben damit die Creatures nen überblick über das Schlachtfeld haben

        public Creature() : this(null) {} // vermeidet doppelten code
        public Creature(BattleManager manager)
        {
            battleManager = manager;

            // duerfen nicht null sein sonst krachts z.B. bei
            // MaxHP.get oder CurrentHP.set. Wenn entities aus
            // einer xml datei geladen werden passiert dies nach
            // dem konstruktor aufruf und die hier gesetzten werte
            // werden ggf. ueberschrieben
            BaseStats = new Stats();
            StatsModifier = new Stats();
            status = new Status(); // sicherheitshalber
        }
        public Creature(BattleManager manager, Stats stats, Ability[] abilities)
        {
            battleManager = manager;
            MaxAP = 100;
            BaseStats = stats;
            Abilities = abilities;
            StatsModifier = new Stats();
            status = new Status(); // sicherheitshalber
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