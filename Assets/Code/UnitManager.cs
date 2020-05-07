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
            if (map.turn % 10 == 0) {
                HashSet<Province> investigatedProvinces = new HashSet<Province>();
                int nInvestigators = 0;
                foreach (Unit u in map.units)
                {
                    if (u is Unit_Investigator && u.isEnthralled() == false)
                    {
                        if (u.parentLocation != null)
                        {
                            investigatedProvinces.Add(u.parentLocation.province);
                        }
                        nInvestigators += 1;
                    }
                }
                HashSet<Province> inhabitedProvinces = new HashSet<Province>();
                foreach (Location l in map.locations)
                {
                    if (l.soc is Society && l.settlement != null && l.settlement.isHuman)
                    {
                        inhabitedProvinces.Add(l.province);
                    }
                }
                if (investigatedProvinces.Count < inhabitedProvinces.Count)
                {
                    foreach (Province p in inhabitedProvinces)
                    {
                        if (investigatedProvinces.Contains(p)) { continue; }

                        int c = 0;
                        Location chosen = null;
                        foreach (Location loc in p.locations)
                        {
                            if (loc.soc is Society && loc.settlement != null && loc.settlement.isHuman)
                            {
                                c += 1;
                                if (Eleven.random.Next(c) == 0)
                                {
                                    chosen = loc;
                                }
                            }
                        }
                        if (chosen != null)
                        {
                            Unit_Investigator u = new Unit_Investigator(chosen, (Society)chosen.soc);
                            u.person = new Person((Society)chosen.soc);
                            u.person.unit = u;
                            u.person.traits.Clear();//Can't see traits, best to have them removed
                            map.units.Add(u);
                            u.parentLocation = chosen;
                        }
                    }
                }
            }
        }
    }
}
