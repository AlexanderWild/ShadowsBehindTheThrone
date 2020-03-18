using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FullSerializer;

namespace Assets.Code
{
    public partial class Map
    {

        public int nSocieties = 12;
        public int minDistBetweenSocs = 12;
        public int citySpread = 5;
        public int townSpread = 12;
        public int sx = 48;
        public int sy = 32;
        public int sizeX;
        public int sizeY;
        public int nPrints;

        public float param_minLocSeparation = 4f;
        public float param_minSeaLocSeparation = 4f;

        //public int param_stepsPerIsland = 2;
        public double param_pPlaceOceanLoc = 0.75;
        public float param_maxLinkDist = 26f;//NB: This is comparing against distance squared, also note minLocSep to avoid issues
        public float param_maxSeaLinkDist = 49f;//NB: This is comparing against distance squared, also note minLocSep to avoid issues
        public int param_nLinksDefault = 2;
        public float param_landmassP = 0.5f;
        public float param_civMergerProportion = 1.5f;//Can actually validly go above 1
        public float param_civNumberTarget = 8;//How many to try to spawn
        public int param_nHexesPerCity = 20;
        public int param_nHexesPerLoc = 16;
        public int param_nLargeForests = 15;
        public int param_nSmallForests = 20;
        public float param_targetHexesPerTown = 20;
        public int param_isolatedTownMinIsolation = 3;//Dist between an isolated town and its nearest neighbour
        public int param_locations_per_province = 5;
        public int param_maxTerritoryRange = 7;
        public int param_margin = 0;
        public int param_nSerpents = 3;
        //public float param_cityPlacementMapP = 0.65f;
        public float param_seaMargin = 0;
        public double param_cityPlacementMult = 7;
        public float param_cityPlacementMapLimit = 0.35f;
        public int param_provinceStepSize = 5;

        public int countLandmassID;
        public int nLandmasses;

        public float[][] humidityMap;
        public float[][] tempMap;
        public float[][] cityPlacementMap;
        public bool[][] landmass;

        public int[][] landmassID;

        public List<List<Hex>> allLandmasses = new List<List<Hex>>();

        public List<Province> provinces = new List<Province>();
        public List<float[]> terrainPositions = new List<float[]>();
        public List<Hex.terrainType> terrainTypes = new List<Hex.terrainType>();

        public float globalTemporaryTempDelta = 0;


        public void loadGenerators()
        {
        }

        public void gen()
        {
            World.log("Map generating starting...");
            sx = param.mapGen_sizeX;
            sy = param.mapGen_sizeY;
            grid = new Hex[sx][];
            for (int i = 0; i < sx; i++) { grid[i] = new Hex[sy]; }
            sizeX = sx;
            sizeY = sy;


            landmass = new bool[sx][];
            for (int i = 0; i < sx; i++) { landmass[i] = new bool[sy]; }
            drawLandmass_islands();


            humidityMap = genHeightmap(sx, sy, 0.6f, 0.5f);
            tempMap = genHeightmap(sx, sy, 0.6f, 0.5f);
            //cityPlacementMap = genHeightmap(sx, sy, 0.6f, 0.5f);
            generateCityPlacementMap();
            //balanceCityPlacementMap();
            loadTerrainPositions();
            biasTempMap();


            for (int i = 0; i < sx; i++)
            {
                for (int j = 0; j < sy; j++)
                {
                    grid[i][j] = new Hex(i, j, this);
                }
            }


            assignTerrainFromClimate();
            setLandmassIDs();
            placeLocations();
            placeMinorLocations();
            placeLinks();
            placeAmphibPoints();
            placeOceanLocations();
            placeSeaLinks();
            assignTerritory();
            addLargeForests();
            addSmallForests();
            assignTerrainFromClimate();
            assignLocationNamesAndIndices();
            placeProvinces();
            checkConnectivity();

            placeInitialSettlements();

            //placeMinorSettlements();

        }

        public void placeInitialSettlements()
        {
            int nNonSea = 0;

            List<float> placementValues = new List<float>();
            foreach (Location loc in locations)
            {
                if (isSea(loc)) { continue; }
                nNonSea += 1;
                if (loc.hex.getHabilitability() < param.mapGen_minHabitabilityForHumans) { continue; }

                float cityPlaceVal = cityPlacementMap[loc.hex.x][loc.hex.y];
                placementValues.Add(cityPlaceVal);
            }

            //We want a set percentage of the land squares to be available for human habitation
            //Placement values is now all the available locations for human hab. We can remove n by setting the placement threshold
            int targetCount = (int)(nNonSea * param.mapGen_proportionOfMapForHumans);
            int nToRemove = placementValues.Count - targetCount;
            float habThreshold = 0;
            placementValues.Sort();//Lowest at [0]
            World.log("targetCount: " + targetCount + " nNow " + placementValues.Count);
            if (nToRemove > 0)
            {
                habThreshold = placementValues[nToRemove];
                World.log("Removing " + nToRemove + " locations from the human available board");
            }


            foreach (Location loc in locations)
            {
                if (isSea(loc)) { loc.isForSocieties = false; continue; }
                float cityPlaceVal = cityPlacementMap[loc.hex.x][loc.hex.y];
                if (cityPlaceVal < habThreshold) { loc.isForSocieties = false; continue; }
            }

            foreach (Location loc in locations)
            {
                if (!loc.isForSocieties) { continue; }
                if (loc.hex.getHabilitability() < param.mapGen_minHabitabilityForHumans) { continue; }

                if (loc.isMajor)
                {
                    loc.settlement = new Set_City(loc);
                }else
                {
                    int q = Eleven.random.Next(2);
                    if (q == 0)
                    {
                        loc.settlement = new Set_Abbey(loc);
                    }
                    else
                    {
                        loc.settlement = new Set_Fort(loc);
                    }
                }

                Society soc = new Society(this);
                soc.setName(loc.shortName);
                loc.soc = soc;

                socialGroups.Add(soc);
            }


            overmind = new Overmind(this);
        }


        public void placeProvinces()
        {
            for (int i = 0; i < sx; i += param_provinceStepSize)
            {
                for (int j = 0; j < sy; j += param_provinceStepSize)
                {
                    int x = Eleven.random.Next(5) + i - 2;
                    int y = Eleven.random.Next(5) + j - 2;

                    if (canGet(x, y))
                    {
                        Province p = new Province(grid[x][y]);
                        p.index = provinces.Count;
                        provinces.Add(p);
                    }
                }
            }

            //Assign all locations to provinces
            foreach (Location loc in locations)
            {
                int i = loc.hex.x;
                int j = loc.hex.y;
                double minDist = -1;
                foreach (Province p in provinces)
                {
                    //Provinces must map to either all land or all sea tiles
                    if (landmass[p.coreHex.x][p.coreHex.y] != landmass[i][j]) { continue; }

                    double dist = getDist(p.coreHex, grid[i][j]);
                    if (minDist == -1 || dist < minDist)
                    {
                        minDist = dist;
                        grid[i][j].province = p;
                    }
                }
                if (grid[i][j].location != null)
                {
                    grid[i][j].province.locations.Add(grid[i][j].location);
                }

            }
            //Hexes follow their parent loc
            for (int i = 0; i < sx; i += 1)
            {
                for (int j = 0; j < sy; j += 1)
                {
                    grid[i][j].province = grid[i][j].territoryOf.hex.province;
                }
            }

            foreach (Province p in provinces)
            {
                int c = 0;
                foreach (Location l in p.locations)
                {
                    l.province = p;
                    if (l.isMajor)
                    {
                        c += 1;
                        if (Eleven.random.Next(c) == 0)
                        {
                            p.capital = l;
                        }
                    }
                }
                if (p.capital != null)
                {
                    if (landmass[p.coreHex.x][p.coreHex.y])
                    {
                        p.name = p.capital.shortName + " province";
                        p.isSea = false;
                    }
                    else
                    {
                        p.name = p.capital.shortName + " sea";
                        p.isSea = true;
                    }
                }
                else
                {
                    if (landmass[p.coreHex.x][p.coreHex.y])
                    {
                        p.name = TextStore.getCityName() + " province";
                        p.isSea = false;
                    }
                    else
                    {
                        p.name = TextStore.getCityName() + " sea";
                        p.isSea = true;
                    }
                }
            }


            //We want the proportions to be almost exactly identical between all resources, but still randomised
            List<Province> duplicateProvinces = new List<Province>();
            duplicateProvinces.AddRange(provinces);
            //Shuffle these real quick
            int n = duplicateProvinces.Count;
            while (n > 1)
            {
                n--;
                int k = Eleven.random.Next(n + 1);
                Province value = duplicateProvinces[k];
                duplicateProvinces[k] = duplicateProvinces[n];
                duplicateProvinces[n] = value;
            }

            //Assign the econ type which is currently used by the fewest humans
            while (duplicateProvinces.Count > 0)
            {
                Province next = duplicateProvinces[duplicateProvinces.Count - 1];
                duplicateProvinces.RemoveAt(duplicateProvinces.Count - 1);
                if (next.isSea == false)
                {
                    int[] useCounts = new int[globalist.allEconTraits.Count];
                    foreach (Location loc in locations)
                    {
                        if (loc.isForSocieties == false) { continue; }
                        if (loc.isOcean) { continue; }
                        if (loc.province.econTraits.Count != 0)
                        {
                            int ind = globalist.allEconTraits.IndexOf(loc.province.econTraits[0]);
                            useCounts[ind] += 1;
                        }
                    }

                    int minV = useCounts[0];
                    int minInd = 0;
                    for (int i = 0; i < useCounts.Length; i++)
                    {
                        if (useCounts[i] < minV)
                        {
                            minV = useCounts[i];
                            minInd = i;
                        }
                    }
                    next.econTraits.Add(globalist.allEconTraits[minInd]);
                }
            }
        }

        public void generateCityPlacementMap()
        {
            float maxDist = ((sx / 2) * (sx / 2)) + ((sy / 2) * (sy / 2));
            maxDist = Mathf.Sqrt(maxDist);
            cityPlacementMap = new float[sx][];
            for (int j = 0; j < sx; j++) { cityPlacementMap[j] = new float[sy]; }

            for (int i = 0; i < sx; i++)
            {
                for (int j = 0; j < sy; j++)
                {
                    float dist = ((i - (sx / 2)) * (i - sx / 2)) + ((j - (sy / 2)) * (j - (sy / 2)));
                    dist = Mathf.Sqrt(dist);
                    cityPlacementMap[i][j] = 1 - (dist / maxDist);
                }
            }
        }

        public void placeStartingTraders()
        {
            //foreach (Location loc in locations)
            //{
            //    if (Eleven.random.NextDouble() < 0.3)
            //    {
            //        if (loc.settlement != null && loc.settlement is Set_City)
            //        {
            //            if (loc.settlement.population.size > 10)
            //            {
            //                Population off = loc.settlement.population.breakoff(5);
            //                Unit_Trader trader = new Unit_Trader(this, null, loc, off); ;
            //                allUnits.Add(trader);
            //                loc.units.Add(trader);
            //            }
            //        }
            //    }
            //}
        }

        public void checkConnectivity()
        {
            List<Location> primarySet = getLinkedBlock(locations[0]);
            if (primarySet.Count == locations.Count) { return; }//Already done

            for (int i = 0; i < 128; i++)
            {
                Location isolated = null;
                foreach (Location l in locations)
                {
                    if (primarySet.Contains(l) == false)
                    {
                        isolated = l;
                        break;
                    }
                }
                if (isolated == null) { return; }
                List<Location> secondSet = getLinkedBlock(isolated);

                double minDist = -1;
                Location bestA = null;
                Location bestB = null;
                foreach (Location l1 in secondSet)
                {
                    foreach (Location l2 in primarySet)
                    {
                        double dist = getSqrDist(l1.hex, l2.hex);
                        if (dist < minDist || minDist == -1)
                        {
                            minDist = dist;
                            bestA = l1;
                            bestB = l2;
                        }
                    }
                }
                if (bestB == null)
                {
                    World.log("Error in map connectivity generation");
                    return;
                }
                Link link = new Link(bestA, bestB);
                bestA.links.Add(link);
                bestB.links.Add(link);


                //Re-check what's now inside
                primarySet = getLinkedBlock(locations[0]);
                if (primarySet.Count == locations.Count) { return; }//Done
            }
        }

        public List<Location> getLinkedBlock(Location loc)
        {
            List<Location> closed = new List<Location>();
            List<Location> open = new List<Location>();
            closed.Add(loc);
            open.Add(loc);


            for (int i = 0; i < 1024; i++)
            {
                List<Location> nextBatch = new List<Location>();
                foreach (Location l in open)
                {
                    foreach (Link link in l.links)
                    {
                        if (closed.Contains(link.other(l))) { continue; }
                        nextBatch.Add(link.other(l));
                        closed.Add(link.other(l));
                    }
                }
                if (nextBatch.Count == 0) { break; }
                open = nextBatch;
            }
            return closed;
        }

        public void placeMinorSettlements()
        {

            foreach (Location loc in locations)
            {
                if (loc.isOcean) { continue; }
                if (loc.soc == null) { continue; }

            }
        }

        public void assignLocationNamesAndIndices()
        {
            string[] oceanNames = new string[] { "deeps", "expanse", "reef", "rocks", "depths" };
            string[] coastalNames = new string[] { "bay", "coast", "lagoon", "hill", "beach", "cliffs" };
            string[] landNames = new string[] { "hills", "plains", "wastes", "hill", "plain", "valley" };

            int index = 0;
            foreach (Location loc in locations)
            {
                loc.index = index;
                index += 1;

                loc.shortName = TextStore.getLocName();
                string name =  loc.shortName + " ";
                string[] opts = null;
                if (loc.isOcean)
                {
                    opts = oceanNames;
                }
                else if (loc.isCoastal)
                {
                    opts = coastalNames;
                }
                else
                {
                    opts = landNames;
                }

                int q = Eleven.random.Next(opts.Length);
                name += opts[q];
                loc.name = name;
            }
        }


        public void placeOceanLocations()
        {
            foreach (Location location in majorLocations)
            {
                int c = 0;
                Hex opt = null;
                if (location.isCoastal)
                {
                    foreach (Hex h in getNeighbours(location.hex))
                    {
                        if (landmass[h.x][h.y]) { continue; }
                        foreach (Hex h2 in getNeighbours(h))
                        {
                            if (h2 == location.hex) { continue; }
                            if (landmass[h2.x][h2.y]) { continue; }

                            bool isValid = true;
                            foreach (Hex h3 in getNeighbours(h2))
                            {
                                if (h3.location != null) { isValid = false; }
                                if (landmass[h3.x][h3.y]) { isValid = false; }
                            }

                            if (isValid)
                            {
                                c += 1;
                                if (Eleven.random.Next(c) == 0)
                                {
                                    opt = h2;
                                }
                            }
                        }
                    }
                }

                if (opt != null)
                {
                    Location seaLoc = new Location(this, opt, false);
                    locations.Add(seaLoc);
                    opt.location = seaLoc;
                    Link link = new Link(location, seaLoc);
                    seaLoc.links.Add(link);
                    location.links.Add(link);
                    seaLoc.isOcean = true;
                }
            }

            double minDist = param_minSeaLocSeparation * param_minSeaLocSeparation;
            for (int i = 0; i < sx; i++)
            {
                for (int j = 0; j < sy; j++)
                {
                    if (landmass[i][j]) { continue; }
                    if (Eleven.random.NextDouble() > param_pPlaceOceanLoc) { continue; }
                    if (grid[i][j].location != null) { continue; }

                    bool isValid = true;
                    foreach (Hex h in getNeighbours(grid[i][j]))
                    {
                        if (landmass[i][j]) { isValid = false; }
                        if (h.location != null) { isValid = false; }
                    }

                    foreach (Location loc in locations)
                    {
                        if (landmass[loc.hex.x][loc.hex.y]) { continue; }
                        double dist = getSqrDist(grid[i][j], loc.hex);
                        if (dist < minDist)
                        {
                            isValid = false;
                            break;
                        }
                    }
                    if (isValid == false) { continue; }

                    Location seaLoc = new Location(this, grid[i][j], false);
                    locations.Add(seaLoc);
                    grid[i][j].location = seaLoc;
                    seaLoc.isOcean = true;
                }
            }
        }

        public void placeSeaLinks()
        {
            foreach (Location loc in locations)
            {
                if (landmass[loc.hex.x][loc.hex.y]) { continue; }

                int nLinks = param_nLinksDefault;
                if (loc.isMajor)
                {
                    nLinks += 1;
                }
                for (int i = 0; i < nLinks; i++)
                {
                    double minDist = -1;
                    Location best = null;
                    foreach (Location l2 in locations)
                    {
                        if (landmass[l2.hex.x][l2.hex.y]) { continue; }
                        if (l2.index <= loc.index) { continue; }


                        bool alreadyPresent = false;
                        foreach (Link link in loc.links)
                        {
                            if (link.b == l2 || link.a == l2) { alreadyPresent = true; }
                        }
                        if (alreadyPresent) { continue; }

                        double dist = getSqrDist(loc.hex, l2.hex);

                        if (dist > param_maxSeaLinkDist) { continue; }

                        if (minDist == -1 || dist < minDist)
                        {
                            if (linkSeaPermissable(loc, l2))
                            {
                                minDist = dist;
                                best = l2;
                            }
                        }
                    }
                    if (best != null)
                    {
                        Link l = new Link(loc, best);
                        loc.links.Add(l);
                        best.links.Add(l);
                    }
                }
            }


            //Cull all monolinks
            for (int i = 0; i < 128; i++)
            {
                bool monolinkFound = false;
                List<Location> rems = new List<Location>();
                foreach (Location loc in locations)
                {
                    if (loc.isOcean == false) { continue; }
                    if (loc.links.Count < 2)
                    {
                        monolinkFound = true;
                        rems.Add(loc);
                    }
                }
                foreach (Location loc in rems)
                {
                    locations.Remove(loc);
                    loc.hex.location = null;
                    foreach (Link l in loc.links)
                    {
                        l.other(loc).links.Remove(l);
                    }
                }

                if (monolinkFound == false) { break; }
            }
        }

        public void placeLinks()
        {
            foreach (Location loc in locations)
            {
                int nLinks = param_nLinksDefault;
                if (loc.isMajor)
                {
                    nLinks += 1;
                }
                for (int i = 0; i < nLinks; i++)
                {
                    double minDist = -1;
                    Location best = null;
                    foreach (Location l2 in locations)
                    {
                        if (l2.index <= loc.index) { continue; }

                        if (landmassID[l2.hex.x][l2.hex.y] != landmassID[loc.hex.x][loc.hex.y]) { continue; }

                        bool alreadyPresent = false;
                        foreach (Link link in loc.links)
                        {
                            if (link.b == l2 || link.a == l2) { alreadyPresent = true; }
                        }
                        if (alreadyPresent) { continue; }

                        double dist = getSqrDist(loc.hex, l2.hex);

                        if (dist > param_maxLinkDist) { continue; }

                        if (minDist == -1 || dist < minDist)
                        {
                            if (linkPermissable(loc, l2))
                            {
                                minDist = dist;
                                best = l2;
                            }
                        }
                    }
                    if (best != null)
                    {
                        Link l = new Link(loc, best);
                        loc.links.Add(l);
                        best.links.Add(l);
                    }
                }
            }
        }

        public bool linkPermissable(Location a, Location b)
        {
            if (a == b) { return false; }

            float x1 = a.hex.x + 0.5f;
            float y1 = a.hex.y + 0.5f;
            float x2 = b.hex.x + 0.5f;
            float y2 = b.hex.y + 0.5f;
            float dX = x2 - x1;
            float dY = y2 - y1;
            float maxAbs = Math.Max(Math.Abs(dX), Math.Abs(dY));
            dX /= maxAbs;
            dY /= maxAbs;

            float x = x1;
            float y = y1;
            for (int i = 0; i < maxAbs * 1.5f; i++)
            {
                x += dX;
                y += dY;
                int iX = (int)x;
                int iY = (int)y;


                if (landmass[iX][iY] == false) { return false; }
                if (grid[iX][iY].location != null)
                {
                    if (grid[iX][iY].location == a) { }//Nothing
                    else if (grid[iX][iY].location == b) { return true; }
                    else { return false; }
                }
            }
            return true;
        }
        public bool linkSeaPermissable(Location a, Location b)
        {
            if (a == b) { return false; }

            float x1 = a.hex.x + 0.5f;
            float y1 = a.hex.y + 0.5f;
            float x2 = b.hex.x + 0.5f;
            float y2 = b.hex.y + 0.5f;
            float dX = x2 - x1;
            float dY = y2 - y1;
            float maxAbs = Math.Max(Math.Abs(dX), Math.Abs(dY));
            dX /= maxAbs;
            dY /= maxAbs;

            float x = x1;
            float y = y1;
            for (int i = 0; i < maxAbs * 1.5f; i++)
            {
                x += dX;
                y += dY;
                int iX = (int)x;
                int iY = (int)y;


                if (landmass[iX][iY] == true) { return false; }
                if (grid[iX][iY].location != null)
                {
                    if (grid[iX][iY].location == a) { }//Nothing
                    else if (grid[iX][iY].location == b) { return true; }
                    else { return false; }
                }
            }
            return true;
        }

        public void setLandmassIDs()
        {

            landmassID = new int[sx][];
            for (int i = 0; i < sx; i++) { landmassID[i] = new int[sy]; }

            for (int i = 0; i < sx; i++)
            {
                for (int j = 0; j < sy; j++)
                {
                    if (landmass[i][j] && landmassID[i][j] == 0)
                    {
                        floodfillLandmass(i, j);
                    }
                }
            }

            foreach (List<Hex> land in allLandmasses)
            {
                World.log("Landmass size: " + land.Count);
            }
        }

        public void floodfillLandmass(int x, int y)
        {
            int id = 1 + countLandmassID;
            countLandmassID += 1;

            List<Hex> all = new List<Hex>();
            List<Hex> open = new List<Hex>();
            open.Add(grid[x][y]);

            while (open.Count > 0)
            {
                List<Hex> o2 = new List<Hex>();
                foreach (Hex h in open)
                {
                    all.Add(h);
                    landmassID[h.x][h.y] = id;
                    foreach (Hex h2 in getNeighbours(h))
                    {
                        if (landmass[h2.x][h2.y] && landmassID[h2.x][h2.y] == 0)
                        {
                            o2.Add(h2);
                            landmassID[h2.x][h2.y] = id;
                        }
                    }
                }
                open = o2;
            }

            allLandmasses.Add(all);
        }

        private float getCityPlaceProp()
        {
            float reply = 0;
            float total = 0;
            for (int i = 0; i < sx; i++)
            {
                for (int j = 0; j < sy; j++)
                {
                    if (landmass[i][j]) { total += 1; if (cityPlacementMap[i][j] > 0.5f) { reply += 1; } }

                }
            }
            reply /= total;
            return reply;
        }

        public void assignTerrainFromClimate()
        {
            for (int i = 0; i < sx; i++)
            {
                for (int j = 0; j < sy; j++)
                {
                    assignTerrain(i, j);
                }
            }
        }

        public void biasTempMap()
        {
            float[][] map = tempMap;

            float sy = sizeY;

            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    //float old = map[i][j];

                    float distFromCentre = j - (sy / 2);
                    distFromCentre = -distFromCentre;//Flip it upside down
                    float proportionFromCentre = distFromCentre / (sy / 2);

                    if (distFromCentre > 0)
                    {
                        map[i][j] *= 1 - proportionFromCentre;
                    }
                    else
                    {
                        proportionFromCentre = -proportionFromCentre;//Make it positive
                        float complement = 1 - map[i][j];
                        complement *= 1 - proportionFromCentre;
                        map[i][j] = 1 - complement;
                    }

                    //World.Log("Old " + old + " now " + map[i][j] + " Y: " + j);
                }
            }
        }

        public void loadTerrainPositions()
        {
            //Terrain for temp/humidity, from 1 to 0
            terrainPositions.Add(new float[] { 0.5f, 0.5f });
            terrainTypes.Add(Hex.terrainType.GRASS);

            terrainPositions.Add(new float[] { 0.45f, 0.65f });
            terrainTypes.Add(Hex.terrainType.GRASS);
            terrainPositions.Add(new float[] { 0.65f, 0.65f });
            terrainTypes.Add(Hex.terrainType.GRASS);
            terrainPositions.Add(new float[] { 0.65f, 0.45f });
            terrainTypes.Add(Hex.terrainType.GRASS);
            terrainPositions.Add(new float[] { 0.45f, 0.45f });
            terrainTypes.Add(Hex.terrainType.GRASS);

            //Cold
            //Terrain for temp/humidity, from 1 to 0
            terrainPositions.Add(new float[] { 0.1f, 0f });
            terrainTypes.Add(Hex.terrainType.SNOW);
            //Terrain for temp/humidity, from 1 to 0
            terrainPositions.Add(new float[] { 0.1f, 0.33f });
            terrainTypes.Add(Hex.terrainType.SNOW);
            //Terrain for temp/humidity, from 1 to 0
            terrainPositions.Add(new float[] { 0.1f, 0.66f });
            terrainTypes.Add(Hex.terrainType.SNOW);
            //Terrain for temp/humidity, from 1 to 0
            terrainPositions.Add(new float[] { 0.1f, 1f });
            terrainTypes.Add(Hex.terrainType.TUNDRA);

            //Chilly
            //Terrain for temp/humidity, from 1 to 0
            terrainPositions.Add(new float[] { 0.2f, 0f });
            terrainTypes.Add(Hex.terrainType.SNOW);
            //Terrain for temp/humidity, from 1 to 0
            terrainPositions.Add(new float[] { 0.2f, 0.33f });
            terrainTypes.Add(Hex.terrainType.TUNDRA);
            //Terrain for temp/humidity, from 1 to 0
            terrainPositions.Add(new float[] { 0.2f, 0.66f });
            terrainTypes.Add(Hex.terrainType.TUNDRA);
            //Terrain for temp/humidity, from 1 to 0
            terrainPositions.Add(new float[] { 0.2f, 1f });
            terrainTypes.Add(Hex.terrainType.WETLAND);

            //Temperate
            //Terrain for temp/humidity, from 1 to 0
            terrainPositions.Add(new float[] { 0.5f, 0f });
            terrainTypes.Add(Hex.terrainType.MUD);
            //Terrain for temp/humidity, from 1 to 0
            terrainPositions.Add(new float[] { 0.5f, 0.33f });
            terrainTypes.Add(Hex.terrainType.GRASS);
            //Terrain for temp/humidity, from 1 to 0
            terrainPositions.Add(new float[] { 0.5f, 0.6f });
            terrainTypes.Add(Hex.terrainType.WETLAND);
            //Terrain for temp/humidity, from 1 to 0
            terrainPositions.Add(new float[] { 0.5f, 0.7f });
            terrainTypes.Add(Hex.terrainType.SWAMP);

            //Warm
            //Terrain for temp/humidity, from 1 to 0
            terrainPositions.Add(new float[] { 0.8f, 0f });
            terrainTypes.Add(Hex.terrainType.DESERT);
            //Terrain for temp/humidity, from 1 to 0
            terrainPositions.Add(new float[] { 0.8f, 0.33f });
            terrainTypes.Add(Hex.terrainType.DRY);
            //Terrain for temp/humidity, from 1 to 0
            terrainPositions.Add(new float[] { 0.8f, 0.66f });
            terrainTypes.Add(Hex.terrainType.DRY);
            //Terrain for temp/humidity, from 1 to 0
            terrainPositions.Add(new float[] { 0.8f, 1f });
            terrainTypes.Add(Hex.terrainType.WETLAND);

            //Hot
            //Terrain for temp/humidity, from 1 to 0
            terrainPositions.Add(new float[] { 0.9f, 0f });
            terrainTypes.Add(Hex.terrainType.DESERT);
            //Terrain for temp/humidity, from 1 to 0
            terrainPositions.Add(new float[] { 0.9f, 0.33f });
            terrainTypes.Add(Hex.terrainType.DESERT);
            //Terrain for temp/humidity, from 1 to 0
            terrainPositions.Add(new float[] { 0.9f, 0.66f });
            terrainTypes.Add(Hex.terrainType.DESERT);
            //Terrain for temp/humidity, from 1 to 0
            terrainPositions.Add(new float[] { 0.9f, 1f });
            terrainTypes.Add(Hex.terrainType.DESERT);
        }

        public void assignTerrain(int x, int y)
        {
            if (!landmass[x][y]) { grid[x][y].terrain = Hex.terrainType.SEA; return; }
            
            //Humidity and temp form a 2D grid with a centre at (0.5,0.5)
            float temp = grid[x][y].getTemperature();
            float wet = humidityMap[x][y];
            

            float minDist = -1;
            for (int i = 0; i < terrainPositions.Count; i++)
            {
                float dist = ((terrainPositions[i][0] - temp) * (terrainPositions[i][0] - temp))
                    + ((terrainPositions[i][1] - wet) * (terrainPositions[i][1] - wet));
                if (minDist == -1 || dist < minDist)
                {
                    minDist = dist;
                    grid[x][y].terrain = terrainTypes[i];
                }
            }
        }

        public void placeMinorLocations()
        {
            int n = 0;
            foreach (Location set in majorLocations)
            {
                int nFarms = 2;
                for (int i = 0; i < nFarms; i++)
                {

                    if (set.territory.Count == 0) { continue; }
                    int count = 0;
                    Hex target = null;
                    foreach (Hex hex in set.territory)
                    {
                        if (landmass[hex.x][hex.y] == false) { continue; }
                        if (hex.location != null) { continue; }

                        //Check for adjacent hexes
                        bool blocked = false;
                        foreach (Hex h2 in getNeighbours(hex))
                        {
                            if (h2.location != null) { blocked = true; break; }
                        }
                        if (blocked) { continue; }

                        count += 1;
                        if (Eleven.random.Next(count) == 0) { target = hex; }
                    }
                    if (target == null || target.settlement != null) { continue; }

                    bool isCoastal = false;
                    foreach (Hex h2 in getNeighbours(target))
                    {
                        if (landmass[h2.x][h2.y] == false) { isCoastal = true; }
                        if (h2.location != null) { World.log("Issue here, placing erroneously"); }
                    }

                    Location l = new Location(this, target, false);
                    locations.Add(l);
                    target.location = l;
                    l.isCoastal = isCoastal;
                    l.parent = set;
                    n += 1;
                }
            }
        }

        public void placeAmphibPoints()
        {
            foreach (Location set in majorLocations)
            {
                Hex lighthouse = null;
                foreach (Hex hex in getNeighbours(set.hex))
                {
                    if (landmass[hex.x][hex.y] == false)
                    {
                        lighthouse = hex;
                    }
                }
                if (lighthouse != null)
                {
                    lighthouse.amphibPoint = true;
                    set.hex.amphibPoint = true;
                    set.isCoastal = true;
                }
            }
        }

        public void placeLocations()
        {
            int tri = 0;
            int nCities = 0;
            float targetNLoCs = sizeX * sizeY;
            targetNLoCs /= param_nHexesPerLoc;
            int tries = (int)targetNLoCs * 10;


            while (nCities < targetNLoCs && tri < tries)
            {
                tri += 1;
                int x = Eleven.random.Next(sizeX);
                int y = Eleven.random.Next(sizeY);

                List<Hex> opts = getArea(x, y, 8, 8);

                List<Hex> validCoast = new List<Hex>();
                List<Hex> validNoncoast = new List<Hex>();
                foreach (Hex h in opts)
                {
                    if (isLocPlacementValid(h.x, h.y))
                    {
                        bool coastal = false;
                        foreach (Hex n in getNeighbours(h))
                        {
                            if (!landmass[n.x][n.y]) { coastal = true; break; }
                        }
                        if (coastal)
                        {
                            validCoast.Add(h);
                        }
                        else
                        {
                            validNoncoast.Add(h);
                        }
                    }
                }

                List<Hex> valids = validCoast;
                if (valids.Count == 0) { valids = validNoncoast; }
                if (valids.Count == 0) { continue; }

                int q = Eleven.random.Next(valids.Count);
                Hex chosen = valids[q];
                chosen.location = new Location(this, chosen, true);
                locations.Add(chosen.location);
                majorLocations.Add(chosen.location);
            }

            assignTerritory();
        }

        public bool isLocPlacementValid(int x2, int y2)
        {
            bool canPlace = true;
            double minPlacement = param_minLocSeparation * param_minLocSeparation;

            if (inMargin(grid[x2][y2])) { canPlace = false; }
            if (landmass[x2][y2] == false) { canPlace = false; }
            foreach (Location set in locations)
            {
                double dist = getSqrDist(grid[x2][y2], set.hex);
                if (dist < minPlacement)
                {
                    return false;
                }
            }

            return canPlace;
        }

        public void assignTerritory()
        {
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    //double minDist = param_maxTerritoryRange * param_maxTerritoryRange;
                    double minDist = -1;
                    Location bestLoc = null;
                    foreach (Location loc in locations)
                    {

                        double dist = getSqrDist(loc.hex, grid[i][j]);

                        //Add a penalty for being on the wrong landmass, but don't disable it
                        if (landmassID[i][j] != landmassID[loc.hex.x][loc.hex.y]) { dist *= 10; }

                        if (minDist == -1 || dist < minDist)
                        {
                            minDist = dist;
                            bestLoc = loc;
                        }
                    }
                    if (bestLoc != null)
                    {
                        grid[i][j].territoryOf = bestLoc;
                        bestLoc.territory.Add(grid[i][j]);
                    }
                }
            }
        }


        public void addLargeForests()
        {
            float mult = sizeX * sizeY;
            mult /= 1500;


            int nSpots = 5;
            int dSpot = 6;
            int nSteppers = 25;
            int stepSpread = 4;
            for (int i = 0; i < param_nLargeForests * mult; i++)
            {
                int mx = Eleven.random.Next(sizeX);
                int my = Eleven.random.Next(sizeY);

                int rnStep = Eleven.random.Next(nSpots);
                for (int s = 0; s < rnStep; s++)
                {
                    int lx = mx += Eleven.random.Next((dSpot * 2) + 1) - dSpot;
                    int ly = my += Eleven.random.Next((dSpot * 2) + 1) - dSpot;

                    for (int s2 = 0; s2 < nSteppers; s2++)
                    {
                        if (canGet(lx, ly))
                        {
                            Hex hex = grid[lx][ly];
                            for (int step = 0; step < stepSpread; step++)
                            {
                                List<Hex> list = getNeighbours(hex);
                                if (list.Count == 0) { break; }
                                int q = Eleven.random.Next(list.Count);
                                hex = list[q];
                            }
                            lx = hex.x;
                            ly = hex.y;
                            if (landmass[lx][ly] && grid[lx][ly].settlement == null && grid[lx][ly].flora == null)
                            {
                                //hex.terrain = Hex.terrainType.FOREST;
                                hex.flora = new Flora_Forest(hex);
                            }
                        }
                    }
                }
            }
        }
        public void addSmallForests()
        {
            float mult = sizeX * sizeY;
            mult /= 1500;

            int nSpots = 4;
            int dSpot = 3;
            int nSteppers = 12;
            int stepSpread = 3;
            for (int i = 0; i < param_nSmallForests * mult; i++)
            {
                int mx = Eleven.random.Next(sizeX);
                int my = Eleven.random.Next(sizeY);

                int rnStep = Eleven.random.Next(nSpots);
                for (int s = 0; s < rnStep; s++)
                {
                    int lx = mx + Eleven.random.Next((dSpot * 2) + 1) - dSpot;
                    int ly = my + Eleven.random.Next((dSpot * 2) + 1) - dSpot;

                    for (int s2 = 0; s2 < nSteppers; s2++)
                    {
                        if (canGet(lx, ly))
                        {
                            Hex hex = grid[lx][ly];
                            for (int step = 0; step < stepSpread; step++)
                            {
                                List<Hex> list = getNeighbours(hex);
                                if (list.Count == 0) { break; }
                                int q = Eleven.random.Next(list.Count);
                                hex = list[q];
                            }
                            lx = hex.x;
                            ly = hex.y;
                            if (landmass[lx][ly] && grid[lx][ly].settlement == null && grid[lx][ly].flora == null)
                            {
                                //hex.terrain = Hex.terrainType.FOREST;
                                hex.flora = new Flora_Forest(hex);
                            }
                        }
                    }
                }
            }
        }

        public bool inMargin(Hex hex)
        {
            //if (hex.x < param_margin) { return true; }
            //if (hex.y < param_margin) { return true; }
            //if (sizeX - hex.x < param_margin) { return true; }
            //if (sizeY - hex.y < param_margin) { return true; }
            return false;
        }
        public bool inMargin(int x, int y)
        {
            //if (x < param_margin) { return true; }
            //if (y < param_margin) { return true; }
            //if (sizeX - x < param_margin) { return true; }
            //if (sizeY - y < param_margin) { return true; }
            return false;
        }

        public float[][] genHeightmap(int sx, int sy, float decay, float targetBias)
        {
            float[][] map = new float[3][];
            for (int i = 0; i < 3; i++)
            {
                map[i] = new float[3];
                for (int j = 0; j < 3; j++)
                {
                    map[i][j] = (float)(Eleven.random.NextDouble() * 2) - 1;
                }
            }

            float mult = 1;
            while (map.Length < sx || map[0].Length < sy)
            {
                mult *= decay;
                map = heightmapSubstep(map, mult);
            }

            normaliseMap(map);

            for (int t = 0; t < 32; t++)
            {
                if (getAvrgMapValue(map) > targetBias)
                {
                    //Too large, need higher exponent
                    exponentiateMap(map, 1.05);
                }
                else
                {
                    exponentiateMap(map, 0.95);
                }
                normaliseMap(map);
            }

            return map;
        }

        public void exponentiateMap(float[][] map, double exponent)
        {
            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[0].Length; j++)
                {
                    map[i][j] = (float)Math.Pow(map[i][j], exponent);
                }
            }
        }

        public void normaliseMap(float[][] map)
        {
            float max = map[0][0];
            float min = map[0][0];
            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[0].Length; j++)
                {
                    if (map[i][j] > max)
                    {
                        max = map[i][j];
                    }
                    if (map[i][j] < min)
                    {
                        min = map[i][j];
                    }
                }
            }
            float range = max - min;
            if (range == 0) { range = 1; }

            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[0].Length; j++)
                {
                    map[i][j] -= min;
                    map[i][j] /= range;
                }
            }
        }

        public float[][] genArray(int i, int j)
        {
            float[][] reply = new float[i][];
            for (int k = 0; k < i; k++)
            {
                reply[k] = new float[j];
            }
            return reply;
        }

        public float getAvrgMapValue(float[][] map)
        {
            float avrg = 0;
            int count = 0;
            for (int i = 0; i < map.Length; i++)
            {
                for (int j = 0; j < map[0].Length; j++)
                {
                    //avrg += map[i][j];
                    if (map[i][j] > 0.5)
                    {
                        avrg += 1;
                    }
                    count += 1;
                }
            }
            if (count > 0)
            {
                avrg /= count;
            }
            return avrg;
        }

        public float[][] heightmapSubstep(float[][] old, float mult)
        {
            int lsx = (old.Length * 2) - 1;
            int lsy = (old[0].Length * 2) - 1;
            float[][] map = genArray(lsx, lsy);

            for (int i = 0; i < lsx; i++)
            {
                for (int j = 0; j < lsy; j++)
                {
                    float avrg = 0;
                    int ox = i / 2;
                    int oy = j / 2;
                    if (i % 2 == 0 && j % 2 == 0) { map[i][j] = old[ox][oy]; continue; }

                    int c = 1;
                    avrg += old[ox][oy];
                    if (ox < old.Length - 1) { avrg += old[ox + 1][oy]; c += 1; }
                    if (oy < old[0].Length - 1) { avrg += old[ox][oy + 1]; c += 1; }
                    if (ox < old.Length - 1 && oy < old[0].Length - 1) { avrg += old[ox + 1][oy + 1]; c += 1; }

                    avrg /= c;
                    float basis = (float)(Eleven.random.NextDouble() * 2) - 1;
                    avrg += basis * mult;
                    map[i][j] = avrg;
                }
            }

            return map;
        }

        public void debugFullBuildLandmass()
        {
            for (int i = 0; i < sizeX; i++)
            {
                for (int j = 0; j < sizeY; j++)
                {
                    landmass[i][j] = true;
                }
            }
        }

        public List<Hex> getArea(int x, int y, int w, int h)
        {
            List<Hex> reply = new List<Hex>();
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    int u = x + i;
                    int v = y + j;
                    if (canGet(u, v))
                    {
                        reply.Add(grid[u][v]);
                    }
                }
            }
            return reply;
        }

        public void drawLandmass_islands()
        {
            DateTime startTime = System.DateTime.Now;
            //Start at a point. Add this to the open list
            //Repeatedly select points around the edge, "paint" around them by selecting a circle of random size
            //Continue till sufficient map is covered

            double targetP = param_landmassP;
            double nLand = 1;
            double nTotal = sx * sy;
            List<int[]> open = new List<int[]>();


            int attempt = 0;
            int maxAttempts = 5000;
            int stepsLeftInIsland = 0;
            while (nLand < nTotal * targetP)
            {

                attempt += 1;
                if (attempt % 100 == 0)
                {
                    DateTime now = DateTime.Now;
                    TimeSpan span = now.Subtract(startTime);
                    if (span.Seconds > 15) { World.Log("Landmass placement failed due to overtime"); return; }
                }
                if (attempt > maxAttempts)
                {
                    World.Log("Terminated landmass placement early due to lack of progress");
                    break;
                }

                if (stepsLeftInIsland <= 0)
                {
                    int tx = Eleven.random.Next(sx);
                    int ty = Eleven.random.Next(sy);
                    if (inMargin(tx, ty)) { continue; }
                    if (landmass[tx][ty]) { continue; }
                    open.Clear();
                    nLand += paintCircle(tx, ty, 3, open);
                    stepsLeftInIsland = Eleven.random.Next(param.mapGen_stepsPerIsland) + 1;
                }

                int[] chosen = chooseFromOpen(open);
                if (chosen == null) { World.Log("Terminated landmass placement early due to lack of open hex"); stepsLeftInIsland = 0; continue; }

                bool bordersSea = false;
                foreach (int[] n in getNeighbours(chosen[0], chosen[1]))
                {
                    if (landmass[n[0]][n[1]] == false)
                    {
                        bordersSea = true;
                        break;
                    }
                }
                if (!bordersSea)
                {
                    open.Remove(chosen);
                }
                else
                {
                    int distToEdge = Math.Min(chosen[0], chosen[1]);
                    if (Math.Abs(chosen[0] - sizeX - 1) < distToEdge) { distToEdge = Math.Abs(chosen[0] - sizeX - 1); }
                    if (Math.Abs(chosen[1] - sizeY - 1) < distToEdge) { distToEdge = Math.Abs(chosen[1] - sizeY - 1); }
                    if (distToEdge < param_margin) { open.Remove(chosen); continue; }

                    int size = Eleven.random.Next(5) + 1;
                    if (size >= distToEdge) { size = distToEdge; }
                    nLand += paintCircle(chosen[0], chosen[1], size, open);
                    stepsLeftInIsland -= 1;
                }
            }
            //World.Log("Landmass placed " + (nLand / nTotal));
        }

        public void drawLandmass()
        {
            DateTime startTime = System.DateTime.Now;
            //Start at a point. Add this to the open list
            //Repeatedly select points around the edge, "paint" around them by selecting a circle of random size
            //Continue till sufficient map is covered

            double targetP = param_landmassP;
            double nLand = 1;
            double nTotal = sx * sy;
            List<int[]> open = new List<int[]>();

            int nSeeds = 1;
            for (int i = 0; i < nSeeds; i++)
            {
                while (true)
                {
                    int tx = Eleven.random.Next(sx);
                    int ty = Eleven.random.Next(sy);
                    if (inMargin(tx, ty)) { continue; }
                    if (landmass[tx][ty]) { continue; }
                    //open.Add(new int[] { tx, ty });
                    //landmass[tx, ty] = true;
                    nLand += paintCircle(tx, ty, 3, open);
                    break;
                }
            }
            //open.Add(new int[] { sx / 2, sy / 2});
            //landmass[sx/2,sy/2] = true;

            int attempt = 0;
            int maxAttempts = 5000;
            while (nLand < nTotal * targetP)
            {

                attempt += 1;
                if (attempt % 100 == 0)
                {
                    DateTime now = DateTime.Now;
                    TimeSpan span = now.Subtract(startTime);
                    if (span.Seconds > 15) { World.Log("Landmass placement failed due to overtime"); return; }
                }
                if (attempt > maxAttempts)
                {
                    World.Log("Terminated landmass placement early due to lack of progress");
                    break;
                }

                int[] chosen = chooseFromOpen(open);
                if (chosen == null) { World.Log("Terminated landmass placement early due to lack of open hex"); break; }

                bool bordersSea = false;
                foreach (int[] n in getNeighbours(chosen[0], chosen[1]))
                {
                    if (landmass[n[0]][n[1]] == false)
                    {
                        bordersSea = true;
                        break;
                    }
                }
                if (!bordersSea)
                {
                    open.Remove(chosen);
                }
                else
                {
                    int distToEdge = Math.Min(chosen[0], chosen[1]);
                    if (Math.Abs(chosen[0] - sizeX - 1) < distToEdge) { distToEdge = Math.Abs(chosen[0] - sizeX - 1); }
                    if (Math.Abs(chosen[1] - sizeY - 1) < distToEdge) { distToEdge = Math.Abs(chosen[1] - sizeY - 1); }
                    if (distToEdge < param_margin) { open.Remove(chosen); continue; }

                    int size = Eleven.random.Next(param.mapGen_maxBrushSize) + 1;
                    if (size >= distToEdge) { size = distToEdge; }
                    nLand += paintCircle(chosen[0], chosen[1], size, open);
                }
            }
            //World.Log("Landmass placed " + (nLand / nTotal));
        }

        //Try to get some bias towards building in the middle so it doesn't always hit the edge
        public int[] chooseFromOpen(List<int[]> open)
        {
            double bestDist = -1;
            int[] reply = null;
            for (int i = 0; i < 2; i++)
            {
                int count = 0;
                int[] chosen = null;
                foreach (int[] hex in open)
                {
                    count += 1;
                    if (Eleven.random.Next(count) == 0)
                    {
                        chosen = hex;
                    }
                }

                if (chosen == null) { return null; }

                double dist = Math.Abs(chosen[0] - (sx / 2)) + Math.Abs(chosen[1] - (sy / 2));
                dist *= Eleven.random.NextDouble();
                if (bestDist == -1 || dist < bestDist)
                {
                    reply = chosen;
                    bestDist = dist;
                }
            }
            return reply;

        }

        private int paintCircle(int x, int y, int size, List<int[]> open)
        {
            int nAdded = 0;
            size *= size;
            for (int i = -size - 2; i < size + 2; i++)
            {
                for (int j = -size - 2; j < size + 2; j++)
                {
                    if (canGet(x + i, j + y))
                    {
                        double sDist = (i * i) + (j * j);
                        if (sDist <= size)
                        {
                            if (landmass[x + i][j + y] == false)
                            {
                                nAdded += 1;

                                int distToEdge = Math.Min(x + i, j + y);
                                if (Math.Abs(sizeX - (x + i) - 1) < distToEdge) { distToEdge = Math.Abs(sizeX - (x + i) - 1); }
                                if (Math.Abs(sizeY - (y + j) - 1) < distToEdge) { distToEdge = Math.Abs(sizeY - (y + j) - 1); }
                                if (distToEdge >= param_seaMargin)
                                {
                                    landmass[x + i][j + y] = true;
                                    open.Add(new int[] { x + i, j + y });
                                }
                            }
                        }
                    }
                }
            }
            return nAdded;
        }

        //It's redundant if the roads already connect to one another
        //Return true if it's needed
        public bool checkRoadRedundancy(Hex core)
        {
            return false;
        }
    }
}
