using UnityEngine;


namespace Assets.Code
{
    public class SG_Fishmen : SocialGroup
    {
        public SG_Fishmen(Map map,Location startingLocation) : base(map)
        {
            float colourReducer = 0.25f;
            float colourReducer2 = 0.5f;
            color = new Color(
                (float)(Eleven.random.NextDouble()* colourReducer),
                (float)Eleven.random.NextDouble() * colourReducer2,
                (float)Eleven.random.NextDouble() * colourReducer2);
            color2 = new Color(
                (float)(Eleven.random.NextDouble() * colourReducer),
                (float)Eleven.random.NextDouble() * colourReducer2,
                (float)Eleven.random.NextDouble() * colourReducer2);
            this.setName("Lights in the Deep");

            startingLocation.soc = this;
            this.threat_mult = map.param.dark_fishmanStartingThreatMult;
        }

        public override void turnTick()
        {
            base.turnTick();
            bool atWar = isAtWar();
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
                    //Withdraw from the land when you're done
                    if (loc.soc == this && (!loc.isOcean) && (atWar == false))
                    {
                        loc.soc = null;
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
                //taken.settlement = new Set_UnholyFlesh_Ganglion(taken);
                taken.settlement = new Set_Ruins(taken);
            }
        }
    }
}