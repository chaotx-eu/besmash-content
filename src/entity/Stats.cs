namespace BesmashContent
{
    public class Stats
    {
        //Die Basisstats, die durch multiplikatoren verändert werden könnten
        //Normal Stats
        public int BaseVIT{get; set;}
        public int BaseATK{get; set;}
        public int BaseMGA{get; set;}
        public int BaseDEF{get; set;}
        public int BaseMGD{get; set;}
        public int BaseAGI{get; set;}
        public int BaseTotal{get{return BaseVIT + BaseATK + BaseMGA + BaseDEF + BaseMGD + BaseAGI;}} //Summe der Basiswerte
        //Additional Stats
        public float BaseACC{get; set;}
        public float BaseDDG{get; set;}
        public int BaseSPD{get; set;}

        //Die Modifikatoren, der Basiswerte (im Normalfall immer x1)
        //Normal Stats
        public float VITModifier{get;set;}
        public float ATKModifier{get;set;}
        public float MGAModifier{get;set;}
        public float DEFModifier{get;set;}
        public float MGDModifier{get;set;}
        public float AGIModifier{get;set;}
        //Additional Stats
        public float ACCModifier{get;set;}
        public float DDGModifier{get;set;}
        public float SPDModifier{get;set;}


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
        public Stats()
        {
            this.BaseVIT = 0;
            this.BaseATK = 0;
            this.BaseMGA = 0;
            this.BaseDEF = 0;
            this.BaseMGD = 0;
            this.BaseAGI = 0;

            this.BaseACC = 1.0f;
            this.BaseDDG = 1.0f;
            this.BaseSPD = 0;

            this.VITModifier = 0.0f;
            this.ATKModifier = 0.0f;
            this.MGAModifier = 0.0f;
            this.DEFModifier = 0.0f;
            this.MGDModifier = 0.0f;
            this.AGIModifier = 0.0f;

            this.ACCModifier = 0.0f;
            this.DDGModifier = 0.0f;
            this.SPDModifier = 0.0f;
        }

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

        //Eine Reihe Stats wird erzeugt, mit eingestellten modifikatoren
        public Stats(int vit, int atk, int mga, int def, int mgd, int agi, float vitm, float atkm, float mgam, float defm, float mgdm, float agim)
        {
            this.BaseVIT = vit;
            this.BaseATK = atk;
            this.BaseMGA = mga;
            this.BaseDEF = def;
            this.BaseMGD = mgd;
            this.BaseAGI = agi;

            this.VITModifier = vitm;
            this.ATKModifier = atkm;
            this.MGAModifier = mgam;
            this.DEFModifier = defm;
            this.MGDModifier = mgdm;
            this.AGIModifier = agim;
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