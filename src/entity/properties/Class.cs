namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using System.Runtime.Serialization;
    using System.Collections.Generic;
    using System;

    /// A class defines how the stats of a
    /// creature should grow when it levels up
    [DataContract(IsReference = true)]
    public class Class : ICloneable {
        /// The title of the class
        [DataMember]
        public string Title {get; set;}

        /// Base stat points which will be distributed to
        /// the associated creature only once where X
        /// represents the min and Y the max value
        [DataMember]
        [ContentSerializer(Optional = true)]
        public Point BaseStatPoints {get; set;}

        /// The weight the vitality stat will get a
        /// point on level up of the creature where
        /// X represents the min and Y the max value
        [DataMember]
        [ContentSerializer(Optional = true)]
        public Point VitWeight {get; set;}

        /// The weight the attack stat will get a
        /// point on level up of the creature where
        /// X represents the min and Y the max value
        [DataMember]
        [ContentSerializer(Optional = true)]
        public Point AtkWeight {get; set;}

        /// The weight the intelligence stat will get
        /// a point on level up of the creature where
        /// X represents the min and Y the max value
        [DataMember]
        [ContentSerializer(Optional = true)]
        public Point IntWeight {get; set;}

        /// The weight the defense stat will get a
        /// point on level up of the creature where
        /// X represents the min and Y the max value
        [DataMember]
        [ContentSerializer(Optional = true)]
        public Point DefWeight {get; set;}

        /// The weight the wisdom stat will get a
        /// point on level up of the creature where
        /// X represents the min and Y the max value
        [DataMember]
        [ContentSerializer(Optional = true)]
        public Point WisWeight {get; set;}

        /// The weight the agility stat will get a
        /// point on level up of the creature where
        /// X represents the min and Y the max value
        [DataMember]
        [ContentSerializer(Optional = true)]
        public Point AgiWeight {get; set;}

        /// Reference to the creature
        /// this class belongs to
        [DataMember]
        [ContentSerializerIgnore]
        public Creature Creature {get; set;}

        /// Creates a new class with default properties
        public Class() {
            Title = "Entity";
            BaseStatPoints = new Point(50, 60);
            VitWeight = new Point(2, 4);
            AtkWeight = new Point(1, 3);
            IntWeight = new Point(1, 3);
            DefWeight = new Point(1, 3);
            WisWeight = new Point(1, 3);
            AgiWeight = new Point(1, 3);
        }

        /// Raises the stats of the creature by distributing
        /// Stats.PointsPerLevel stat points according to
        /// the set weights of this class. If this is the first
        /// time this method is called on this creature the
        /// base stat points are distributed aswell.
        public void raiseStats() {
            if(Creature == null) return;
            Random rng = Creature.RNG;
            int vitCount = rng.Next(VitWeight.X, VitWeight.Y+1);
            int atkCount = rng.Next(AtkWeight.X, AtkWeight.Y+1);
            int intCount = rng.Next(IntWeight.X, IntWeight.Y+1);
            int defCount = rng.Next(DefWeight.X, DefWeight.Y+1);
            int wisCount = rng.Next(WisWeight.X, WisWeight.Y+1);
            int agiCount = rng.Next(AgiWeight.X, AgiWeight.Y+1); // TODO ggt
            List<StatType> statTypes = new List<StatType>();

            int i;
            for(i = 0; i < vitCount; ++i) statTypes.Add(StatType.Vit);
            for(i = 0; i < atkCount; ++i) statTypes.Add(StatType.Atk);
            for(i = 0; i < intCount; ++i) statTypes.Add(StatType.Int);
            for(i = 0; i < defCount; ++i) statTypes.Add(StatType.Def);
            for(i = 0; i < wisCount; ++i) statTypes.Add(StatType.Wis);
            for(i = 0; i < agiCount; ++i) statTypes.Add(StatType.Agi);

            int points = Stats.LevelUpPoints;
            if(!Creature.IsBorn) {
                points += rng.Next(BaseStatPoints.X, BaseStatPoints.Y+1);
                Creature.setBorn();
            }

            for(i = 0; i < points; ++i)
                Creature.Stats.add(1, statTypes[rng.Next(statTypes.Count)]);
        }

        public object clone() {
            return MemberwiseClone();
        }
    }
}