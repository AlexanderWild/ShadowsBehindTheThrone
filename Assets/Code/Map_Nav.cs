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

                if (nSteps > 1024)
                {
                    throw new Exception("Map discontinuity detected");
                }
            }
        }
        /*
         * This should be replaced with A* as ASAP as possible
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
        /**
         * This function tells you "how much do you know about this particular social group?"
         * It is a function of how blotted out information is by intervening terrain
         * 
         * This implementation should 100% be replaced with A*
         */
        public double getInformationAvailability(Location a,SocialGroup b)
        {
            if (a == null) { return 0; }
            if (a.soc == b) { return 1; }
            if (a.information.ContainsKey(b))
            {
                return a.information.lookup(b);
            }
            else
            {
                return 0;
            }
        }


        public Location[] getPathTo(Location a, Location b)
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
                        World.log("Adding " + l2.hex.getName());
                        if (seen.Contains(l2)) { continue; }
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

                if (nSteps > 1024)
                {
                    throw new Exception("Map discontinuity detected");
                }
            }
            return null;
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

                if (nSteps > 1024)
                {
                    throw new Exception("Map discontinuity detected");
                }
            }
        }

        public void recomputeInformationAvailability(SocialGroup sg)
        {
            int CUTOFF = 128;
            HashSet<Location> closed = new HashSet<Location>();
            List<Location> working = new List<Location>();
            List<double> distances = new List<double>();
            foreach (Location a in locations)
            {
                if (a.soc == sg)
                {
                    closed.Add(a);
                    working.Add(a);
                    double dist = 1;
                    distances.Add(dist);
                    a.information.set(sg, dist);
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
                    double dist = distances[i];
                    dist *= l.getInformationAvailability();

                    dist =  Math.Max(param.minInformationAvailability, dist);

                    l.information.set(sg, dist);

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
