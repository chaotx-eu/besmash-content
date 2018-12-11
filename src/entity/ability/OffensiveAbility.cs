namespace BesmashContent
{
    public class OffensiveAbility : Ability
    {
        public int BaseDamage{get;set;} //Der standard schaden der Fähigkeit. Angriff und Verteidigung werden später hinzu gerechnet
        public int BaseAcc{get; set;}   //100 = 100% Treffer chance, bei Standard ACC und DDG Werten für angreifer und verteidiger.
        public int maxRange{get;set;}   //Standard Reichweite für Fähigkeiten
        public bool IsMagical{get;set;} //Sollen ATK und DEF oder MGA und MGD benutzt werden?
        
        public struct PossibleStatus{public Status.Type type; public int chance /* 1-100 */;};
        public struct PossibleBuff{public Buff.Type type; public int rounds; public int turns; public int strength; public int chance /* 1-100 */;};
        public PossibleBuff[] potentialBuffs{get;set;}  //Werden bei einem Treffer BUffs oder Debuffs aufs target gelegt.
        public PossibleStatus[] potentialStatus{get;set;}   //werden bei einem Treffer Statuseffekte aufs Target gelegt
        public Entity target{get{return target;} set{if(isInRange(value)) target = value;}} //Wer soll das Ziel der Attacke sein? (Nur möglich, wennd as Ziel in Reichweite ist)

        public OffensiveAbility(Entity user, int cost, string name, int damage, int range, bool magical) : base(user, cost, name)
        {
            BaseDamage = damage;
            maxRange = range;
            IsMagical = magical;
            type = Type.attack;
        }
        public void determineTarget()
        {
            //todo
        }
        public bool isInRange(Entity target)    //Überprüft ob das Ziel in Reichweite des Angriffs ist
        {
            //todo
            return true;        //Bis ich es implementiert hab, ist erstmal alles in Reichweite
        }
        
        public override void useAbility()
        {
            this.determineTarget();
            bool success = false;   //Wird bei Erfolg mit true überschrieben
            BattleEntity attacker = this.AbilityUser.battleManager.fightingEntities.Find(e => e.entity == this.AbilityUser);    //Sucht die Battleentity des angreifers aus der Liste der fightingEntitys
            BattleEntity defender = this.AbilityUser.battleManager.fightingEntities.Find(e => e.entity == this.target);         //Sucht die Battleentity des verteidiger aus der Liste der fightingEntitys
            
            success = AbilityUser.battleManager.attack(attacker, defender, this);   //Der Angriff wird, über den Battlemanager ausgeführt

            if(success)
            {
                foreach (PossibleBuff b in potentialBuffs)  //Bei einem Erfolg werden Buffs und Debuffs applied
                {
                    if (b.chance >= this.AbilityUser.battleManager.random.Next(100))
                        defender.battleBuffs.Add(new Buff(defender, b.type, b.rounds, b.turns, b.strength));
                }

                foreach (PossibleStatus s in potentialStatus)   //Bei einem Erfolg werden Statuseffekte applied
                {
                    if (s.chance >= this.AbilityUser.battleManager.random.Next(100))
                        defender.entity.status.addStatus(s.type);
                }

                if(followUpOnSuccess)     //Bei einem Erfolg wird die Followup Ability gestartet
                    FollowUpAbility.useAbility();
            }

            if(followUpOnFail && !success)                                  //Bei einem Misserfolg wird die Followup Ability gestartet.
                    FollowUpAbility.useAbility();
        }
    }
}