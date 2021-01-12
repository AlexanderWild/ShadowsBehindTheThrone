using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public partial class Map
    {

        public bool isSea(Location loc)
        {
            return isSea(loc.hex);
        }
        public bool isSea(Hex hex)
        {
            return !landmass[hex.x][hex.y];
        }
        public bool canGet(int x, int y)
        {
            return x >= 0 && y >= 0 && x < sizeX && y < sizeY;
        }
        public double getDist(Location a, Location b)
        {
            return getDist(a.hex, b.hex);
        }
        public double getDist(Hex a, Hex b)
        {
            return Math.Sqrt(getSqrDist(a, b));
        }

        /*
         * This should be replaced with A* as ASAP as possible
         */
        public int getStepDist(Location a, Location b)
        {
            if (a == b) { return 0; }

            //Expand in waves from the starting point, each time adding those on the border
            //We know the distance, since each border is 1 distance more
            HashSet<Location> seen = new HashSet<Location>();
            HashSet<Location> open = new HashSet<Location>();
            open.Add(a);
            seen.Add(a);
            int nSteps = 0;
            while (true)
            {
                HashSet<Location> border = new HashSet<Location>();
                nSteps += 1;
                foreach (Location loc in open)
                {
                    foreach (Location l2 in getNeighbours(loc))
                    {
                        if (seen.Contains(l2)) { continue; }
                        if (l2 == b) { return nSteps; }
                        border.Add(l2);
                        seen.Add(l2);
                    }
                }
                //The border is now used as the opens
                open.Clear();
                foreach (Location loc in border)
                {
                    open.Add(loc);
                }

                if (nSteps > 1024)
                {
                    throw new Exception("Map discontinuity detected");
                }
            }
        }


        /*
         * This should be replaced with A* as ASAP as possible
         */
        public int getStepDist(Location a, SocialGroup b)
        {
            if (a.soc == b) { return 0; }

            //Expand in waves from the starting point, each time adding those on the border
            //We know the distance, since each border is 1 distance more
            HashSet<Location> seen = new HashSet<Location>();
            HashSet<Location> open = new HashSet<Location>();
            open.Add(a);
            seen.Add(a);
            int nSteps = 0;
            while (true)
            {
                HashSet<Location> border = new HashSet<Location>();
                nSteps += 1;
                foreach (Location loc in open)
                {
                    foreach (Location l2 in getNeighbours(loc))
                    {
                        if (seen.Contains(l2)) { continue; }
                        if (l2.soc == b) { return nSteps; }
                        border.Add(l2);
                        seen.Add(l2);
                    }
                }
                //The border is now used as the opens
                open.Clear();
                foreach (Location loc in border)
                {
                    open.Add(loc);
                }

                if (nSteps > 128)
                {
                    return nSteps;
                    //throw new Exception("Map discontinuity detected");
                }
            }
        }

        /*
        * This should be replaced with A* as ASAP as possible
        * (It never will be)
        */
        public int getStepDist(SocialGroup a, SocialGroup b)
        {
            if (a == b) { return 0; }

            //Expand in waves from the starting point, each time adding those on the border
            //We know the distance, since each border is 1 distance more
            HashSet<Location> seen = new HashSet<Location>();
            HashSet<Location> open = new HashSet<Location>();
            foreach (Location loc in locations)
            {
                if (loc.soc == a)
                {
                    open.Add(loc);
                    seen.Add(loc);
                }
            }
            int nSteps = 0;
            while (true)
            {
                HashSet<Location> border = new HashSet<Location>();
                nSteps += 1;
                foreach (Location loc in open)
                {
                    foreach (Location l2 in getNeighbours(loc))
                    {
                        if (seen.Contains(l2)) { continue; }
                        if (l2.soc == b) { return nSteps; }
                        border.Add(l2);
                        seen.Add(l2);
                    }
                }
                //The border is now used as the opens
                open.Clear();
                foreach (Location loc in border)
                {
                    open.Add(loc);
                }

                if (nSteps > 1024)
                {
                    throw new Exception("Map discontinuity detected");
                }
            }
        }



        public Location[] getPathTo(Location a, Location b, Unit u = null, bool safeMove = false)
        {
            HashSet<Location> seen = new HashSet<Location>();
            List<Location> open = new List<Location>();
            List<Location[]> paths = new List<Location[]>();
            open.Add(a);
            seen.Add(a);
            paths.Add(new Location[] { a });

            int nSteps = 0;
            Location loc;
            while (true)
            {
                List<Location> border = new List<Location>();
                List<Location[]> newPaths = new List<Location[]>();
                nSteps += 1;
                for (int i = 0; i < open.Count; i++)
                {
                    loc = open[i];
                    foreach (Location l2 in getNeighbours(loc))
                    {
                        if (seen.Contains(l2)) { continue; }
                        if (safeMove && u != null && l2.soc != null && l2.soc.hostileTo(u)) { continue; }//Unsafe location
                        Location[] path = new Location[paths[i].Length + 1];
                        for (int j = 0; j < paths[i].Length; j++) { path[j] = paths[i][j]; }
                        path[path.Length - 1] = l2;
                        if (l2 == b) { return path; }
                        border.Add(l2);
                        newPaths.Add(path);
                        seen.Add(l2);
                    }
                }
                open = border;
                paths = newPaths;

                if (nSteps > 128)
                {
                    return null;
                    //throw new Exception("Map discontinuity detected");
                }
            }
        }

        public Location[] getPathTo(Location a, SocialGroup b,Unit u=null,bool safeMove=false)
        {
            HashSet<Location> seen = new HashSet<Location>();
            List<Location> open = new List<Location>();
            List<Location[]> paths = new List<Location[]>();
            open.Add(a);
            seen.Add(a);
            paths.Add(new Location[] { a });

            int nSteps = 0;
            Location loc;
            while (true)
            {
                List<Location> border = new List<Location>();
                List<Location[]> newPaths = new List<Location[]>();
                nSteps += 1;
                for (int i = 0; i < open.Count; i++)
                {
                    loc = open[i];
                    foreach (Location l2 in getNeighbours(loc))
                    {
                        if (seen.Contains(l2)) { continue; }
                        if (safeMove && u != null && l2.soc != null && l2.soc.hostileTo(u)) { continue; }//Unsafe location
                        Location[] path = new Location[paths[i].Length + 1];
                        for (int j = 0; j < paths[i].Length; j++) { path[j] = paths[i][j]; }
                        path[path.Length - 1] = l2;
                        if (l2.soc == b) { return path; }
                        border.Add(l2);
                        newPaths.Add(path);
                        seen.Add(l2);
                    }
                }
                open = border;
                paths = newPaths;

                if (nSteps > 128)
                {
                    return null;
                }
            }
        }
        public Location[] getEmptyPathTo(SocialGroup source, SocialGroup b)
        {
            HashSet<Location> seen = new HashSet<Location>();
            List<Location> open = new List<Location>();
            List<Location[]> paths = new List<Location[]>();
            foreach (Location a in locations)
            {
                if (a.soc == source) { 
                    open.Add(a);
                    seen.Add(a);
                    paths.Add(new Location[] { a });
                }
            }
            int nSteps = 0;
            Location loc;
            while (true)
            {
                List<Location> border = new List<Location>();
                List<Location[]> newPaths = new List<Location[]>();
                nSteps += 1;
                for (int i=0;i<open.Count;i++)
                {
                    loc = open[i];
                    foreach (Location l2 in getNeighbours(loc))
                    {
                        if (seen.Contains(l2)) { continue; }
                        if (l2.soc != null && l2.soc != b) { continue; }
                        Location[] path = new Location[paths[i].Length + 1];
                        for (int j = 0; j < paths[i].Length; j++) { path[j] = paths[i][j]; }
                        path[path.Length - 1] = l2;
                        if (l2.soc == b) { return path; }
                        border.Add(l2);
                        newPaths.Add(path);
                        seen.Add(l2);
                    }
                }
                if (border.Count == 0) { return null; }//Can't reach without crossing through other nation's terrain
                open = border;
                paths = newPaths;

                if (nSteps > 128)
                {
                    throw new Exception("Map discontinuity detected");
                }
            }
        }
        
        public bool moveTowards(Unit u,Location loc)
        {
            if (u.location == loc) { return true; }

            Location[] locations = getPathTo(u.location, loc);
            if (locations == null || locations.Length < 2) { return false; }
            adjacentMoveTo(u, locations[1]);
            return true;
        }
        public void adjacentMoveTo(Unit u,Location loc)
        {
            foreach (Unit u2 in loc.units)
            {
                if (u2.isMilitary)
                {
                    if (u != u2 && u.hostileTo(u2))
                    {
                        combatAction(u, u2, loc);
                        return;
                    }
                }
            }
            //Check agent combat after military unit combat
            foreach (Unit u2 in loc.units)
            {
                if (!u2.isMilitary)
                {
                    if (u != u2 && u.hostileTo(u2))
                    {
                        combatAction(u, u2, loc);
                        return;
                    }
                }
            }
            u.location.units.Remove(u);
            loc.units.Add(u);
            u.location = loc;
        }

        public void combatAction(Unit u, Unit u2, Location loc)
        {
            if (u.isMilitary == false && u2.isMilitary == false && u.isEnthralled() && u2.society == loc.soc)
            {
                //Enthralled attacking an agent in their homeland
                //Can be defended by military units locally stationned
                Unit defender = null;
                if (loc.settlement != null && loc.settlement.embeddedUnit != null)
                {
                    defender = loc.settlement.embeddedUnit;
                }
                foreach (Unit u3 in loc.units)
                {
                    if (u3.isMilitary && u3.society == u2.society)
                    {
                        defender = u3;
                    }
                }
                if (defender != null)
                {
                    loc.map.world.prefabStore.popMsg("You agent cannot attack " + u2.getName() + " as they are defended the military unit present (" + defender.getName() + ")");
                    return;
                }
            }

            double lethality = loc.map.param.combat_lethality;
            double lethalityDef = loc.map.param.combat_lethalityDefensive;

            bool hadAttackBonus = false;
            foreach (Unit bonusU in u.location.units)
            {
                if (bonusU.society == u.society && bonusU is Unit_Investigator && bonusU.task is Task_SupportMilitary)
                {
                    if (((Unit_Investigator)bonusU).state == Unit_Investigator.unitState.knight)
                    {
                        lethality *= param.unit_knightCombatBonus;
                        hadAttackBonus = true;
                        break;
                    }
                    if (((Unit_Investigator)bonusU).state == Unit_Investigator.unitState.basic)
                    {
                        lethality *= param.unit_agentCombatBonus;
                        hadAttackBonus = true;
                        break;
                    }
                }
            }
            foreach (Unit bonusU in u2.location.units)
            {
                if (bonusU.society == u2.society && bonusU is Unit_Investigator && bonusU.task is Task_SupportMilitary)
                {
                    if (((Unit_Investigator)bonusU).state == Unit_Investigator.unitState.knight)
                    {
                        lethalityDef *= param.unit_knightCombatBonus;
                        hadAttackBonus = true;
                        break;
                    }
                    if (((Unit_Investigator)bonusU).state == Unit_Investigator.unitState.basic)
                    {
                        lethalityDef *= param.unit_agentCombatBonus;
                        hadAttackBonus = true;
                        break;
                    }
                }
            }

            world.prefabStore.particleCombat(u.location.hex, u2.location.hex);
            int dmgDone = (int)(u.hp * (lethality + (Eleven.random.NextDouble() * lethality)));
            if (u2.isMilitary)
            {
                if (loc.settlement != null && loc.settlement.defensiveStrengthCurrent > 0 && u2.society == loc.soc)
                {
                    double ablated = Math.Min(loc.settlement.defensiveStrengthCurrent, dmgDone / 2d);
                    dmgDone -= (int)ablated;
                    loc.settlement.defensiveStrengthCurrent -= ablated;
                }

                int dmgReplied = (int)(u2.hp * (lethalityDef + (Eleven.random.NextDouble() * lethalityDef)));
                if (dmgReplied >= u.hp)
                {
                    dmgReplied = u.hp - 1;
                    if (dmgReplied < 0) { dmgReplied = 0; }
                }
                u.hp -= dmgReplied;
            }


            if (dmgDone < 1) { dmgDone = 1; }

            bool kicked = false;
            if (dmgDone > 1 && (u2.isMilitary == false)) { dmgDone = 1; kicked = true; }


            u2.hp -= dmgDone;
            if (u2.isEnthralled())
            {
                world.prefabStore.popMsgAgent(u, u2, u.getName() + " attacks " + u2.getName() + ", inflicting " + dmgDone + " HP damage!");
                    //world.prefabStore.popMsg(u.getName() + " attacks " + u2.getName() + ", inflicting " + dmgDone + " HP damage!");
            }
            if (u.isEnthralled() && u2 is Unit_Investigator && u.person != null && u2.person != null)
            {
                u2.person.getRelation(u.person).suspicion = 1;
            }
            if (u.hp <= 0)
            {
                u.die(this, "Took damage attacking " + u2.getName());
            }

            if (u2.hp <= 0)
            {
                u2.die(this, "Attacked by " + u.getName());
            }
            else if (kicked)
            {
                int c = 0;
                Location kickedTo = null;
                foreach (Location l2 in u2.location.getNeighbours())
                {
                    bool bad = false;
                    foreach (Unit u3 in l2.units)
                    {
                        if (u3.hostileTo(u2) || u2.hostileTo(u3)) { bad = true; break; }
                    }
                    if (!bad)
                    {
                        c += 1;
                        if (Eleven.random.Next(c) == 0)
                        {
                            kickedTo = l2;
                        }
                    }
                }
                if (kickedTo != null)
                {
                    u2.task = null;
                    adjacentMoveTo(u2, kickedTo);
                    if (u2.isEnthralled())
                    {
                        world.prefabStore.popMsgAgent(u, u2, u2.getName() + " is forced to retreat, and is now in " + u2.location.getName());
                        //world.prefabStore.popMsg(u2.getName() + " is forced to retreat, and is now in " + u2.location.getName());
                    }
                }
            }

            if (u.isMilitary && loc.settlement != null)
            {
                if (loc.settlement is Set_City)
                {
                    Set_City city = (Set_City)loc.settlement;
                    city.infrastructure *= (int)((Eleven.random.NextDouble() * 0.25) + 0.7);//0.05 to 0.3 dmg
                    city.population *= (int)((Eleven.random.NextDouble() * 0.25) + 0.7);//0.05 to 0.3 dmg
                    if (city.infrastructure < 1) { city.infrastructure = 1; }
                    if (city.population < 1) { city.population = 1; }
                }
            }

            if (u.isMilitary && u2.isMilitary && u.society is Society && u2.society is Society)
            {
                Property.addProperty(this, loc, "Recent Human Battle");
            }
        }

        public void recomputeInformationAvailability(SocialGroup sg)
        {
            int CUTOFF = 128;
            HashSet<Location> closed = new HashSet<Location>();
            List<Location> working = new List<Location>();
            List<int> distances = new List<int>();
            foreach (Location a in locations)
            {
                if (a.soc == sg)
                {
                    closed.Add(a);
                    working.Add(a);
                    int dist = 0;
                    distances.Add(dist);
                    a.distanceToTarget[sg] = 0;
                }

            }

            int steps = 0;
            while (working.Count > 0)
            {
                steps += 1;
                if (steps >= CUTOFF) { break; }
                List<Location> next = new List<Location>();
                List<int> nextDistances = new List<int>();
                for (int i = 0; i < working.Count; i++)
                {
                    Location l = working[i];
                    int dist = distances[i];

                    l.distanceToTarget[sg] = dist;

                    dist += 1;

                    foreach (Location n in l.getNeighbours())
                    {
                        if (closed.Contains(n)) { continue; }

                        closed.Add(n);
                        nextDistances.Add(dist);
                        next.Add(n);
                    }
                }

                working = next;
                distances = nextDistances;
            }
        }

        public double getSqrDist(Hex a, Hex b)
        {
            if (a.y % 2 == b.y % 2)
            {
                return ((a.x - b.x) * (a.x - b.x)) + ((a.y - b.y) * (a.y - b.y));
            }
            float x1 = a.x;
            float y1 = a.y;
            float x2 = b.x;
            float y2 = b.y;
            if (a.y % 2 == 1)
            {
                x1 += 0.5f;
            }
            else
            {
                x2 += 0.5f;
            }
            return ((x1 - x2) * (x1 - x2)) + ((y1 - y2) * (y1 - y2));
        }

        public List<int[]> getNeighbours(int x, int y)
        {
            List<int[]> reply = new List<int[]>();
            if (canGet(x + 1, y)) { reply.Add(new int[] { x + 1, y }); }
            if (canGet(x - 1, y)) { reply.Add(new int[] { x - 1, y }); }
            if (y % 2 == 0)
            {//Even, can add 1
                if (canGet(x + 1, y + 1)) { reply.Add(new int[] { x + 1, y + 1 }); }
                if (canGet(x, y + 1)) { reply.Add(new int[] { x, y + 1 }); }
                if (canGet(x + 1, y - 1)) { reply.Add(new int[] { x + 1, y - 1 }); }
                if (canGet(x, y - 1)) { reply.Add(new int[] { x, y - 1 }); }
            }
            else
            {//Odd, can subtract 1
                if (canGet(x - 1, y + 1)) { reply.Add(new int[] { x - 1, y + 1 }); }
                if (canGet(x, y + 1)) { reply.Add(new int[] { x, y + 1 }); }
                if (canGet(x - 1, y - 1)) { reply.Add(new int[] { x - 1, y - 1 }); }
                if (canGet(x, y - 1)) { reply.Add(new int[] { x, y - 1 }); }
            }

            return reply;
        }

        public int getRotation(Hex a, Hex b)
        {
            for (int i = 0; i < 6; i++)
            {
                if (getNeighbourRelative(a, i) == b) { return i; }
            }
            return -1;
        }

        public Hex getNeighbourRelative(Hex core, int rotation)
        {
            int i = rotation;
            bool right = false;
            int y = 0;
            if (i == 0) { right = false; y = 0; }
            else if (i == 1) { right = false; y = -1; }
            else if (i == 2) { right = true; y = -1; }
            else if (i == 3) { right = true; y = 0; }
            else if (i == 4) { right = true; y = 1; }
            else if (i == 5) { right = false; y = 1; }

            return getNeighbourRelative(core, y, right);
        }

        public Hex getNeighbourRelative(Hex core, int up, bool right)
        {
            if (up > 1) { up = 1; }
            if (up < -1) { up = -1; }
            int x = core.x; int y = core.y;
            if (up == 0)
            {
                if (!right)
                {
                    if (canGet(x - 1, y)) { return grid[x-1][ y]; } else { return null; }
                }
                else
                {
                    if (canGet(x + 1, y)) { return grid[x + 1][ y]; } else { return null; }
                }
            }
            else if (y % 2 == 0)
            {//Even, add one
                if (right)
                {
                    if (canGet(x + 1, y + up)) { return grid[x + 1][ y + up]; } else { return null; }
                }
                else
                {
                    if (canGet(x, y + up)) { return grid[x][ y + up]; } else { return null; }
                }
            }
            else
            {//Odd, remove one

                if (right)
                {
                    if (canGet(x, y + up)) { return grid[x][ y + up]; } else { return null; }
                }
                else
                {
                    if (canGet(x - 1, y + up)) { return grid[x-1][ y + up]; } else { return null; }
                }
            }
        }

        public List<Hex> getNeighbours(Hex hex)
        {
            List<Hex> reply = new List<Hex>();
            if (canGet(hex.x + 1, hex.y)) { reply.Add(grid[hex.x + 1][ hex.y]); }
            if (canGet(hex.x - 1, hex.y)) { reply.Add(grid[hex.x - 1][ hex.y]); }
            if (hex.y % 2 == 0)
            {//Even, can add 1
                if (canGet(hex.x + 1, hex.y + 1)) { reply.Add(grid[hex.x + 1][ hex.y + 1]); }
                if (canGet(hex.x, hex.y + 1)) { reply.Add(grid[hex.x][ hex.y + 1]); }
                if (canGet(hex.x + 1, hex.y - 1)) { reply.Add(grid[hex.x + 1][ hex.y - 1]); }
                if (canGet(hex.x, hex.y - 1)) { reply.Add(grid[hex.x][ hex.y - 1]); }
            }
            else
            {//Odd, can subtract 1
                if (canGet(hex.x - 1, hex.y + 1)) { reply.Add(grid[hex.x - 1][ hex.y + 1]); }
                if (canGet(hex.x, hex.y + 1)) { reply.Add(grid[hex.x][ hex.y + 1]); }
                if (canGet(hex.x - 1, hex.y - 1)) { reply.Add(grid[hex.x - 1][ hex.y - 1]); }
                if (canGet(hex.x, hex.y - 1)) { reply.Add(grid[hex.x][ hex.y - 1]); }
            }

            return reply;
        }

        public List<SocialGroup> getExtendedNeighbours(SocialGroup sg)
        {
            int CUTOFF = 128;
            List<SocialGroup> reply = new List<SocialGroup>();
            HashSet<Location> closed = new HashSet<Location>();
            List<Location> working = new List<Location>();
            foreach (Location a in locations)
            {
                if (a.soc == sg)
                {
                    closed.Add(a);
                    working.Add(a);
                }

            }

            int steps = 0;
            while (working.Count > 0)
            {
                steps += 1;
                if (steps >= CUTOFF) { break; }
                List<Location> next = new List<Location>();
                List<double> nextDistances = new List<double>();
                for (int i = 0; i < working.Count; i++)
                {
                    Location l = working[i];

                    if (l.soc != null && l.soc != sg)
                    {
                        if (reply.Contains(l.soc) == false)
                        {
                            reply.Add(l.soc);
                        }
                    }
                    else
                    {
                        foreach (Location n in l.getNeighbours())
                        {
                            if (closed.Contains(n)) { continue; }

                            closed.Add(n);
                            next.Add(n);
                        }
                    }
                }
                working = next;
            }
            return reply;
        }

        public List<Location> getNeighbours(Location loc)
        {
            List<Location> reply = new List<Location>();
            foreach (Link l in loc.links)
            {
                reply.Add(l.other(loc));
            }
            return reply;
        }
        public List<SocialGroup> getNeighbours(SocialGroup group)
        {
            List<SocialGroup> reply = new List<SocialGroup>();

            foreach (Location loc in locations)
            {
                if (loc.soc == group)
                {
                    foreach (Location l2 in loc.getNeighbours())
                    {
                        if (l2.soc != group && l2.soc != null && (reply.Contains(l2.soc) == false))
                        {
                            reply.Add(l2.soc);
                        }
                    }
                }
            }

            return reply;
        }
    }
}
