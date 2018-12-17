namespace BesmashContent
{
    public abstract class Ability
    {
        public enum Type {attack, move, heal, cure, buff};
        public Type type{get;set;}
        public Creature AbilityUser{get; set;}
        public int AbilityCost{get; set;}
        public string AbilityName{get; set;}
        public Ability FollowUpAbility{get; set;}
        public bool followUpOnSuccess{get; set;}
        public bool followUpOnFail{get; set;}
        public EffectAnimation animation{get;set;}

        // damit diese klasse serialisiert werden kann benoetigt
        // sie einen parameterlosen konstruktor. Alternativ kann
        // man die klasse mit [DataContract], und gewünschte
        // serialisierbare attribute mit [DataMember] markieren
        // (siehe z.B. GameObject, MapObject, Team, TileMap).
        // [DataContract(IsReference=true)] nur falls die Klasse
        // referenzen auf andere bereits serialisierbare Objekte
        // enthält

        public Ability() : this(null, 0, "") {}
        public Ability (Creature user, int cost, string name)
        {
            AbilityUser = user;
            AbilityCost = cost;
            AbilityName = name;
        }
        public abstract void useAbility();
    }
}