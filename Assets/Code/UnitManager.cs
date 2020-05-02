using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class UnitManager
    {
        public Map map;

        public UnitManager(Map m)
        {
            this.map = m;
        }

        public void turnTick()
        {
            if (map.units.Count < map.param.unit_targetUnitsPerLoc * map.locations.Count)
            {
                List<Society> socs = new List<Society>();
                List<int> socSizes = new List<int>();
                foreach (SocialGroup sg in map.socialGroups)
                {
                    if (sg is Society)
                    {
                        Society soc = (Society)sg;
                        if (soc.isDarkEmpire == false)
                        {
                            socs.Add(soc);
                            socSizes.Add(soc.getSize());
                        }
                    }
                }
                int tournRounds = 5;
                int bestSize = -1;
                Society chosen = null;
                if (socs.Count != 0)
                {
                    for (int t = 0; t < tournRounds; t++)
                    {
                        int q = Eleven.random.Next(socs.Count);
                        if (socSizes[q] > bestSize)
                        {
                            bestSize = socSizes[q];
                            chosen = socs[q];
                        }
                    }
                }
                if (chosen != null)
                {
                    Unit_Investigator u = new Unit_Investigator(chosen.getCapital(), chosen);
                    u.person = new Person(chosen);
                    u.person.unit = u;
                    u.person.traits.Clear();//Can't see traits, best to have them removed
                    map.units.Add(u);
                    u.parentLocation = chosen.getCapital();
                }
            }
        }
    }
}
