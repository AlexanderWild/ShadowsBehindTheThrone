using UnityEngine;


namespace Assets.Code
{
    public class SG_WormHive : SocialGroup
    {
        public SG_WormHive(Map map,Location startingLocation) : base(map)
        {
            float colourReducer = 0.25f;
            color = new Color(
                (float)Eleven.random.NextDouble(),
                (float)Eleven.random.NextDouble(),
                (float)Eleven.random.NextDouble() * colourReducer);
            color2 = new Color(
                (float)Eleven.random.NextDouble(),
                (float)Eleven.random.NextDouble(),
                (float)Eleven.random.NextDouble() * colourReducer);
            this.setName(startingLocation.shortName + " Worm Hive");

            startingLocation.soc = this;
            startingLocation.settlement = new Set_WormNest(startingLocation);
            this.threat_mult = map.param.dark_evilThreatMult;
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
                    else if (loc.settlement is Set_WormNest)
                    {
                        size += 1;
                    }
                }
            }

            if (Eleven.random.NextDouble() < 0.25/(size+1))
            {
                Location expansion = null;
                int c = 0;
                foreach (Location loc in map.locations)
                {
                    if (loc.soc == this)
                    {
                        foreach (Location l2 in loc.getNeighbours())
                        {
                            if (l2.isOcean) { continue; }

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
                    expansion.settlement = new Set_WormNest(expansion);
                }
            }
        }

        public override void takeLocationFromOther(SocialGroup def, Location taken)
        {
            base.takeLocationFromOther(def, taken);

            if (def is Society && taken.settlement != null)
            {
                taken.settlement = new Set_Ruins(taken);
            }
        }
    }
}