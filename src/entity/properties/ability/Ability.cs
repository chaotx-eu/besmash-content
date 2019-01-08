namespace BesmashContent {
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Linq;
    using System;
    using Utility;

    /// An ability which is defined by its ability
    /// components and their behaviour
    [DataContract(IsReference = true)]
    public class Ability : ICloneable {
        /// The title of this ability
        [DataMember]
        [ContentSerializer(Optional = true)]
        public string Title {get; set;}

        /// The ap required to execute this ability
        [DataMember]
        [ContentSerializer(Optional = true)]
        public int APCost {get; set;}

        /// List of root components this ability is made of
        [DataMember]
        [ContentSerializer(CollectionItemName = "Component")]
        public List<AbilityComponent> Components {get; set;}

        /// User of this ability (make sure this is set)
        [DataMember]
        [ContentSerializerIgnore]
        public MapObject User {get; set;}

        /// Wether this ability is currently executed
        [DataMember]
        [ContentSerializerIgnore]
        public bool IsExecuting {get; protected set;}

        /// Returns a list of all points relative to the
        /// user position this abilities components will
        /// be executed on
        public List<Point> getTargetSpots() {
            List<Point> targets = new List<Point>();
            Components.ForEach(c => getTargets(c, targets));
            return targets;
        }

        /// Helper to for getting target spots traversing
        /// recursivly through the components
        private void getTargets(AbilityComponent component, List<Point> targets) {
            AbilityComponent comp = component;
            Point pos = Point.Zero;

            while(comp != null) {
                pos += comp.Position;
                comp = comp.Parent;
            }

            targets.Add(pos);
            component.Children.ForEach(c => getTargets(c, targets));
        }

        /// Executes this ability
        public virtual void execute() {
            IsExecuting = true;
            Components.ForEach(component => component.execute());
        }

        /// Loads required resources for this abilities components
        public void load(ContentManager content) {
            Components.ForEach(component => {
                component.Ability = this;
                component.load(content);
            });
        }

        /// Updates this abilities components
        public void update(GameTime gameTime) {
            IsExecuting = Components.Find(
                component => component.IsExecuting) != null;
                
            if(IsExecuting) Components.ForEach(
                component => component.update(gameTime));
        }

        /// Creates and returns a clone of this ability
        /// where the components are deep clones
        public object clone() {
            Ability copy = MemberwiseClone() as Ability;
            AbilityComponent component;
            List<AbilityComponent> components = new List<AbilityComponent>();

            Components.ForEach(orig => {
                component = orig.clone() as AbilityComponent;
                component.Ability = copy;
                components.Add(component);
            });

            copy.Components = components;
            return copy;
        }
    }
}