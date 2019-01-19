namespace BesmashContent {
    using System;

    public delegate void DamageEventHandler(Creature sender, DamageEventArgs args);
    public class DamageEventArgs : EventArgs {
        public PropertyTarget DamageTarget {get; protected set;}
        public DamageType DamageType {get; protected set;}
        public Element DamageElement {get; protected set;}
        public int DamageAmount {get; protected set;}
        public bool WasCritical {get; protected set;}

        public DamageEventArgs(PropertyTarget target, DamageType type, Element element, int amount, bool critical) {
            DamageTarget = target;
            DamageType = type;
            DamageElement = element;
            DamageAmount = amount;
            WasCritical = critical;
        }
    }
}