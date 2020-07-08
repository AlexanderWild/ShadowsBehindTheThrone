using UnityEngine;


namespace Assets.Code
{
    public class SG_UnholyFlesh : SocialGroup
    {
        public SG_UnholyFlesh(Map map,Location startingLocation) : base(map)
        {
            float colourReducer = 0.25f;
            color = new Color(
                (float)(Eleven.random.NextDouble()* colourReducer) + (1-colourReducer),
                (float)Eleven.random.NextDouble() * colourReducer,
                (float)Eleven.random.NextDouble() * colourReducer);
            color2 = new Color(
                (float)(Eleven.random.NextDouble() * colourReducer) + (1 - colourReducer),
                (float)Eleven.random.NextDouble() * colourReducer,
                (float)Eleven.random.NextDouble() * colourReducer);
            this.setName("Unholy Flesh from " + startingLocation.shortName);

            startingLocation.soc = this;
            this.threat_mult = map.param.dark_fleshThreatMult;
        }

        public override bool hostileTo(Unit u)
        {
            if (u.isEnthralled() == false) { return true; }
            return base.hostileTo(u);
        }

        public override bool isDark()
        {
            return true;
        }
        public override string getTypeName()
        {
            return "Player Controlled";
        }
        public override string getTypeDesc()
        {
            return "A forest of limbs, teeth and claws, stretching across hills, fields and valleys. An agressive life-form you can expand and command to attack human settlements." +
                " It is recommended to cause civil wars to weaken the human nations before you attack.";
        }
        public override void turnTick()
        {
            base.turnTick();
            foreach (Location loc in map.locations)
            {
                if (loc.soc == this)
                {
                    if (loc.settlement == null)
                    {
                        loc.soc = null;
                    }
                    else
                    {
                        loc.hex.purity = 0;
                    }
                }
            }
        }
        public override bool hasEnthralled()
        {
            return true;
        }

        public override void takeLocationFromOther(SocialGroup def, Location taken)
        {
            base.takeLocationFromOther(def, taken);
            
            if (taken.settlement != null)
            {
                taken.settlement = new Set_UnholyFlesh_Ganglion(taken);
            }
        }
    }
}