namespace BesmashContent
{
    using System.Runtime.Serialization;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    public abstract class Creature : Movable
    {
        /// Creatures may wear armor...
        public Helmet Helmet {get; set;}
        public Chestplate Chestplate {get; set;}
        public Pants Pants {get; set;}

        /// ...and hold a weapon
        public Weapon Weapon {get; set;}

        /// The name of this creature
        public string Name {get; protected set;}
        [ContentSerializerIgnore]

        public int MaxHP {get{return (BaseStats.VIT + StatsModifier.VIT) * 10;}}
        
        private int currentHP;
        [ContentSerializer(Optional=true)]
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
        
        [ContentSerializer(Optional=true)]
        public int MaxAP{get;set;}

        private int currentAP;
        [ContentSerializer(Optional=true)]
        public int CurrentAP {
            get {return currentAP;}
            set {
                currentAP = value;
                if(currentAP > MaxAP) currentAP = MaxAP;
            }
        }

        public Stats BaseStats {get; set;}
        
        [ContentSerializer(Optional=true)]
        public Stats StatsModifier {get; set;}
        private Ability[] abilities;
        public Ability[] Abilities{get{return abilities;}set
        {
            if(value != null)
            {
                foreach(Ability a in value)
                {
                    a.AbilityUser = this;
                }
            }
            abilities = value;
        }}
        [ContentSerializer(Optional=true)]
        public Status status{get; set;}

        public static BattleUtils BattleUtils = BattleUtils.newInstance();

        public Creature()
        {

            // duerfen nicht null sein sonst krachts z.B. bei
            // MaxHP.get oder CurrentHP.set. Wenn entities aus
            // einer xml datei geladen werden passiert dies nach
            // dem konstruktor aufruf und die hier gesetzten werte
            // werden ggf. ueberschrieben
            BaseStats = new Stats();
            StatsModifier = new Stats();
            currentHP = MaxHP;
            status = new Status(); // sicherheitshalber
        }
        public Creature(Stats stats, Ability[] abilities)
        {
            MaxAP = 100;
            BaseStats = stats;
            Abilities = abilities;
            currentHP = MaxHP;
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
                if (BattleUtils.random.Next(100) <= (10 * status.roundsAsleep) + 35)
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
            //Kann überschrieben werden
        }
    }
}