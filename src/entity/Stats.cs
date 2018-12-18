namespace BesmashContent {
    using System;
    using Microsoft.Xna.Framework.Content;
    public class Stats {
        public static float MIN_MOD {get;} = 0.25f;

        //Die Basisstats, die durch multiplikatoren verändert werden könnten
        //Normal Stats
        public int BaseVIT{get; set;}
        public int BaseATK{get; set;}
        public int BaseMGA{get; set;}
        public int BaseDEF{get; set;}
        public int BaseMGD{get; set;}
        public int BaseAGI{get; set;}
        [ContentSerializerIgnore]
        public int BaseTotal{get{return BaseVIT + BaseATK + BaseMGA + BaseDEF + BaseMGD + BaseAGI;}} //Summe der Basiswerte
        //Additional Stats
        public float BaseACC{get; set;}
        public float BaseDDG{get; set;}
        public int BaseSPD{get; set;}

        //Die Modifikatoren, der Basiswerte (im Normalfall immer x1)
        //Normal Stats
        private float vitModifier;
        public float VITModifier {
            get {return Math.Min(MIN_MOD, vitModifier);}
            set {vitModifier = value;}
        }

        private float atkModifier;
        public float ATKModifier{
            get {return Math.Min(MIN_MOD, atkModifier);}
            set {atkModifier = value;}
        }

        private float mgaModifier;
        public float MGAModifier{
            get {return Math.Min(MIN_MOD, mgaModifier);}
            set {mgaModifier = value;}
        }

        private float defModifier;
        public float DEFModifier{
            get {return Math.Min(MIN_MOD, defModifier);}
            set {defModifier = value;}
        }

        private float mgdModifier;
        public float MGDModifier{
            get {return Math.Min(MIN_MOD, mgdModifier);}
            set {mgdModifier = value;}
        }

        private float agiModifier;
        public float AGIModifier{
            get {return Math.Min(MIN_MOD, agiModifier);}
            set {agiModifier = value;}
        }


        //Additional Stats
        private float accModifier;
        public float ACCModifier{
            get {return Math.Min(MIN_MOD, accModifier);}
            set {accModifier = value;}
        }

        private float ddgModifier;
        public float DDGModifier{
            get {return Math.Min(MIN_MOD, ddgModifier);}
            set {ddgModifier = value;}
        }

        private float spdModifier;
        public float SPDModifier{
            get {return Math.Min(MIN_MOD, spdModifier);}
            set {spdModifier = value;}
        }

        //Die Tatsächlichen Werte werden automatisch errechnet
        public int VIT {get{return (int)(VITModifier * BaseVIT);}} //Vitalität (bestimmt die maximalen HP)
        public int ATK {get{return (int)(ATKModifier * BaseATK);}} //Angriff
        public int MGA {get{return (int)(MGAModifier * BaseMGA);}} //Magie Angriff
        public int DEF {get{return (int)(DEFModifier * BaseDEF);}} //Verteidigung
        public int MGD {get{return (int)(MGDModifier * BaseMGD);}} //Magie verteidigung
        public int AGI {get{return (int)(AGIModifier * BaseAGI);}} //Agilität (bestimmt wie oft (und schnell) man angreifen darf)
        public float ACC {get{return ACCModifier * BaseACC;}} //Accuracy (bestimmt, wie hoch die chance ist, das man mit einem Angriff trifft)
        public float DDG {get{return DDGModifier * BaseDDG;}} //Dodge (Chance gegngerischen Angriffen aus zu weichen)
        public int SPD {get{return (int)(ACCModifier * BaseSPD);}} //Speed (Anzahl der Felder, die man pro RUnde gehen kann)

        //Komplett lehre Stats (zum beispiel für Buffs)
        public Stats() {}
        // wird eh alles mit 0 initialisiert
        // eleganter waere jedoch:
        //
        // public Stats() : this(0, 0, 0, 0, 0, 0) {}
        // hier wuerden dann aber auch die modifier mit
        // entsprechenden werten initialisiert werden

        //Eine vereinfachte Reihe Stats wird erzeugt, mit standard modifikatoren
        public Stats(int vit, int atk, int mga, int def, int mgd, int agi)
        {
            this.BaseVIT = vit;
            this.BaseATK = atk;
            this.BaseMGA = mga;
            this.BaseDEF = def;
            this.BaseMGD = mgd;
            this.BaseAGI = agi;

            this.BaseACC = 1.0f;
            this.BaseDDG = 1.0f;
            this.BaseSPD = 4;

            this.VITModifier = 1.0f;
            this.ATKModifier = 1.0f;
            this.MGAModifier = 1.0f;
            this.DEFModifier = 1.0f;
            this.MGDModifier = 1.0f;
            this.AGIModifier = 1.0f;

            this.ACCModifier = 1.0f;
            this.DDGModifier = 1.0f;
            this.SPDModifier = 1.0f;
        }
        
        //Eine vollständige Reihe von Stats wird erzeugt mit standard modifikatoren
        public Stats(int vit, int atk, int mga, int def, int mgd, int agi, float acc, float ddg, int spd)
        {
            this.BaseVIT = vit;
            this.BaseATK = atk;
            this.BaseMGA = mga;
            this.BaseDEF = def;
            this.BaseMGD = mgd;
            this.BaseAGI = agi;

            this.BaseACC = acc;
            this.BaseDDG = ddg;
            this.BaseSPD = spd;

            this.VITModifier = 1.0f;
            this.ATKModifier = 1.0f;
            this.MGAModifier = 1.0f;
            this.DEFModifier = 1.0f;
            this.MGDModifier = 1.0f;
            this.AGIModifier = 1.0f;

            this.ACCModifier = 1.0f;
            this.DDGModifier = 1.0f;
            this.SPDModifier = 1.0f;
        }   

        //Eine Reihe einfacher Stats wird erzeugt, mit eingestellten modifikatoren
        public Stats(int vit, int atk, int mga, int def, int mgd, int agi, float vitm, float atkm, float mgam, float defm, float mgdm, float agim)
        {
            this.BaseVIT = vit;
            this.BaseATK = atk;
            this.BaseMGA = mga;
            this.BaseDEF = def;
            this.BaseMGD = mgd;
            this.BaseAGI = agi;

            this.BaseACC = 1.0f;
            this.BaseDDG = 1.0f;
            this.BaseSPD = 4;

            this.VITModifier = vitm;
            this.ATKModifier = atkm;
            this.MGAModifier = mgam;
            this.DEFModifier = defm;
            this.MGDModifier = mgdm;
            this.AGIModifier = agim;

            this.ACCModifier = 1.0f;
            this.DDGModifier = 1.0f;
            this.SPDModifier = 1.0f;
        }

        //Eine Reihe erweiterter Stats wird erzeugt, mit eingestellten modifikatoren
        public Stats(int vit, int atk, int mga, int def, int mgd, int agi,  float acc, float ddg, int spd, float vitm, float atkm, float mgam, float defm, float mgdm, float agim, float accm, float ddgm, float spdm)
        {
            this.BaseVIT = vit;
            this.BaseATK = atk;
            this.BaseMGA = mga;
            this.BaseDEF = def;
            this.BaseMGD = mgd;
            this.BaseAGI = agi;

            this.BaseACC = acc;
            this.BaseDDG = ddg;
            this.BaseSPD = spd;

            this.VITModifier = vitm;
            this.ATKModifier = atkm;
            this.MGAModifier = mgam;
            this.DEFModifier = defm;
            this.MGDModifier = mgdm;
            this.AGIModifier = agim;

            this.ACCModifier = accm;
            this.DDGModifier = ddgm;
            this.SPDModifier = spdm;
        }
        
        //Rechnet die Werte von zwei Statlisten zusammen (zum beispiel um Buffs zu addieren)
        public Stats combineStats(Stats stats)
        {
            return new Stats
            (   
                this.BaseVIT + stats.BaseVIT,
                this.BaseATK + stats.BaseATK, 
                this.BaseMGA + stats.BaseMGA, 
                this.BaseDEF + stats.BaseDEF, 
                this.BaseMGD + stats.BaseMGD, 
                this.BaseAGI + stats.BaseAGI,
                this.VITModifier + stats.VITModifier,
                this.ATKModifier + stats.ATKModifier,
                this.MGAModifier + stats.MGAModifier,
                this.DEFModifier + stats.DEFModifier,
                this.MGDModifier + stats.MGDModifier,
                this.AGIModifier + stats.AGIModifier
            );
        }
    }
}