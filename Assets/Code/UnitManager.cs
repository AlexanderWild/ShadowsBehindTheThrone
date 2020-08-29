using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class UnitManager
    {
        public Map map;
        public bool hasSpawnedPaladin;

        public UnitManager(Map m)
        {
            this.map = m;
        }

        public void turnTick()
        {
            if (map.turn % 5 == 0) {
                checkInvestigators();
                checkMerchants();
                checkPaladins();
            }
        }

        public void checkPaladins() {

            if (map.param.usePaladins == 0) { return; }

            int nPaladins = 0;
            int targetPaladins = 0;


            if (map.param.useAwareness == 1)
            {
                if (map.worldPanic >= map.param.panic_paladinSpawn_1)
                {
                    targetPaladins += 1;
                }
                if (map.worldPanic >= map.param.panic_paladinSpawn_2)
                {
                    targetPaladins += 1;
                }
            }

            if (targetPaladins > 0) { 
                bool hasAgents = false;
                foreach (Unit u in map.units)
                {
                    if (u is Unit_Paladin)
                    {
                        nPaladins += 1;
                    }
                    if (u.isEnthralled())
                    {
                        hasAgents = true;
                    }
                }

                if (hasAgents && nPaladins < targetPaladins)
                {
                    double bestScore = 0;
                    Location l2 = null;
                    int c = 0;
                    foreach (Location loc in map.locations)
                    {
                        double score = 0;
                        if (loc.person() == null) { continue; }
                        if (loc.person().state == Person.personState.enthralled) { continue; }

                        bool occupied = false;
                        foreach (Unit u in loc.units)
                        {
                            if (u.isEnthralled()) { occupied = true; break; }
                        }
                        if (occupied) { continue; }

                        score += 0.1;
                        score -= loc.person().shadow;
                        score += loc.person().awareness;

                        if (score > bestScore)
                        {
                            c = 0;
                            bestScore = score;

                        }
                        if (score == bestScore)
                        {
                            c += 1;
                            if (Eleven.random.Next(c) == 0)
                            {
                                l2 = loc;
                            }
                        }
                    }
                    if (l2 != null)
                    {
                        if (!hasSpawnedPaladin)
                        {
                            map.world.prefabStore.popMsg("A Paladin has arrived in " + l2.getName() + ". A holy warrior, they will hunt down your agents.");
                        }
                        Unit_Paladin paladin = new Unit_Paladin(l2, map.soc_light);
                        paladin.person = new Person(map.soc_light);
                        paladin.person.unit = paladin;
                        paladin.person.traits.Clear();//Can't see traits, best to have them removed
                        map.units.Add(paladin);
                        hasSpawnedPaladin = true;
                    }
                }
            }
        }

        public void checkMerchants()
        {
            int n = 0;
            foreach (Unit u in map.units)
            {
                if (u is Unit_Merchant)
                {
                    n += 1;
                }
            }
            if (n < 5)
            {
                int c = 0;
                Location chosen = null;
                if (chosen == null)
                {
                    foreach (Province p in map.provinces)
                    {
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
                    }
                }
                if (chosen != null)
                {
                    Unit_Merchant u = new Unit_Merchant(chosen, (Society)chosen.soc);
                    u.person = new Person((Society)chosen.soc);
                    u.person.unit = u;
                    u.person.traits.Clear();//Can't see traits, best to have them removed
                    map.units.Add(u);
                    u.parentLocation = chosen;
                }
            }
        }

        public void checkInvestigators()
        {
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
            if (investigatedProvinces.Count < inhabitedProvinces.Count * map.param.unit_investigatorsPerProvince)
            {
                int c = 0;
                Location chosen = null;
                foreach (Province p in inhabitedProvinces)
                {
                    if (investigatedProvinces.Contains(p)) { continue; }

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
                }
                //Allow doubling up if you exceed cap
                if (chosen == null)
                {
                    foreach (Province p in inhabitedProvinces)
                    {
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
