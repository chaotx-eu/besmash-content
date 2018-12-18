namespace BesmashContent
{
    public class OffensiveAbility : Ability
    {
        public int BaseDamage{get;set;} //Der standard schaden der Fähigkeit. Angriff und Verteidigung werden später hinzu gerechnet
        public int BaseAcc{get; set;}   //100 = 100% Treffer chance, bei Standard ACC und DDG Werten für angreifer und verteidiger.
        public int maxRange{get;set;}   //Standard Reichweite für Fähigkeiten
        public bool IsMagical{get;set;} //Sollen ATK und DEF oder MGA und MGD benutzt werden?
        
        public struct PossibleStatus
        {
            public Status.Type type; public int chance /* 1-100 */;
            public PossibleStatus(Status.Type type, int chance){this.type = type; this.chance = chance;}
        };
        public struct PossibleBuff
        {
            public Buff.Type type; public int rounds; public int turns; public int strength; public int chance /* 1-100 */;
            public PossibleBuff(Buff.Type type, int rounds, int turns, int strength, int chance){this.type = type; this.rounds = rounds; this.turns = turns; this.strength = strength; this.chance = chance;}
        };
        public PossibleBuff[] potentialBuffs{get;set;}  //Werden bei einem Treffer BUffs oder Debuffs aufs target gelegt.
        public PossibleStatus[] potentialStatus{get;set;}   //werden bei einem Treffer Statuseffekte aufs Target gelegt

        private Creature priv_target;
        public Creature target{get{return priv_target;} set{if(isInRange(value)) priv_target = value;}} //Wer soll das Ziel der Attacke sein? (Nur möglich, wennd as Ziel in Reichweite ist)

        public OffensiveAbility()
        {
            
        }

        public OffensiveAbility(Creature user, int cost, string name, int damage, int range, bool magical) : base(user, cost, name)
        {
            BaseDamage = damage;
            maxRange = range;
            IsMagical = magical;
            type = Type.attack;
        }
        public OffensiveAbility(Creature user, int cost, string name, int damage, int range, bool magical, PossibleBuff[] buffs, PossibleStatus[] status) : base(user, cost, name)
        {
            BaseDamage = damage;
            maxRange = range;
            IsMagical = magical;
            type = Type.attack;
            potentialBuffs = buffs;
            potentialStatus = status;
        }
        public void determineTarget()
        {
            //todo
        }
        public bool isInRange(Creature target)    //Überprüft ob das Ziel in Reichweite des Angriffs ist
        {
            //todo
            return true;        //Bis ich es implementiert hab, ist erstmal alles in Reichweite
        }
        
        public override void useAbility()
        {
            this.determineTarget();
            bool success = false;   //Wird bei Erfolg mit true überschrieben
            FightingInfo attacker = Creature.BattleManager.fightingEntities.Find(e => e.Creature == this.AbilityUser);    //Sucht die FightingInfo des angreifers aus der Liste der fightingCreatures
            FightingInfo defender = Creature.BattleManager.fightingEntities.Find(e => e.Creature == this.target);         //Sucht die FightingInfo des verteidiger aus der Liste der fightingCreatures
            
            success = Creature.BattleManager.attack(attacker, defender, this);   //Der Angriff wird, über den Battlemanager ausgeführt

            if(success)
            {
                foreach (PossibleBuff b in potentialBuffs)  //Bei einem Erfolg werden Buffs und Debuffs applied
                {
                    if (b.chance >= Creature.BattleManager.random.Next(100))
                        defender.battleBuffs.Add(new Buff(defender, b.type, b.rounds, b.turns, b.strength));
                }

                foreach (PossibleStatus s in potentialStatus)   //Bei einem Erfolg werden Statuseffekte applied
                {
                    if (s.chance >= Creature.BattleManager.random.Next(100))
                        Status.addStatus(defender.Creature, s.type);
                }

                if(followUpOnSuccess)     //Bei einem Erfolg wird die Followup Ability gestartet
                    FollowUpAbility.useAbility();
            }

            if(followUpOnFail && !success)                                  //Bei einem Misserfolg wird die Followup Ability gestartet.
                    FollowUpAbility.useAbility();
        }
    }
}