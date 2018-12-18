namespace BesmashContent
{
    using Microsoft.Xna.Framework.Content;
    public abstract class Ability
    {
        public enum Type {attack, move, heal, cure, buff};
        [ContentSerializerIgnore]
        public Type type{get;set;}
        [ContentSerializerIgnore]
        public Creature AbilityUser{get; set;}
        public string AbilityName{get; set;}
        public int AbilityCost{get; set;}
        [ContentSerializer(Optional = true)]
        public Ability FollowUpAbility{get; set;}
        
        [ContentSerializer(Optional = true)]
        public bool followUpOnSuccess{get; set;}
        
        [ContentSerializer(Optional = true)]
        public bool followUpOnFail{get; set;}
        
        [ContentSerializer(Optional = true)]
        public EffectParameters animation{get;set;}

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