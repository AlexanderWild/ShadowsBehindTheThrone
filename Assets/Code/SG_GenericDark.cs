using UnityEngine;


namespace Assets.Code
{
    public class SG_GenericDark : SocialGroup
    {
        public SG_GenericDark(Map map,Location startingLocation) : base(map)
        {
            float colourReducer = 0.25f;
            color = new Color(
                (float)Eleven.random.NextDouble()* colourReducer,
                (float)Eleven.random.NextDouble() * colourReducer,
                (float)Eleven.random.NextDouble() * colourReducer);
            color2 = new Color(
                (float)Eleven.random.NextDouble() * colourReducer,
                (float)Eleven.random.NextDouble() * colourReducer,
                (float)Eleven.random.NextDouble() * colourReducer);
            this.setName("Darkness in " + startingLocation.shortName);

            startingLocation.soc = this;
            startingLocation.settlement = new Set_DarkGeneric(startingLocation);
            this.threat_mult = 2;
        }
        public override bool hasEnthralled()
        {
            return true;
        }

        public override bool hostileTo(Unit u)
        {
            if (u.isEnthralled() == false) { return true; }
            return base.hostileTo(u);
        }

        public override void turnTick()
        {
            base.turnTick();
            int size = 0;
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
                        size += 1;
                        loc.hex.purity = 0;
                    }
                }
            }

            if (Eleven.random.NextDouble() < 0.25 && size < 3)
            {
                Location expansion = null;
                int c = 0;
                foreach (Location loc in map.locations)
                {
                    if (loc.soc == this)
                    {
                        foreach (Location l2 in loc.getNeighbours())
                        {
                            if (l2.soc == null)
                            {
                                c += 1;
                                if (Eleven.random.Next(c) == 0)
                                {
                                    expansion = l2;
                                }
                            }
                        }
                    }
                }
                if (expansion != null)
                {
                    expansion.soc = this;
                    expansion.settlement = new Set_DarkGeneric(expansion);
                }
            }
        }
    }
}