using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class GraphicalMap
    {
        public static float scale = 1;
        public static LinkedList<GraphicalHex> loaded = new LinkedList<GraphicalHex>();
        public static World world;
        public static Map map;
        public static float x;
        public static float y;
        public static int drawDist = 14;
        public static float hexSizeX = 1.31f;
        public static float hexSizeY = 1.11f;
        public static Hex selectedHex;
        public static object selectedSelectable;
        public static float maxScale = 1.5f;
        public static float minScale = 0.5f;
        public static int lastMapChange;

        public static float panX = 0;
        public static float panY = 0;
        public static int panStepsTaken;
        public static int panStepsToTake = 30;

        public static void tick()
        {
            if (world == null) { return; }
            if (world.map == null) { return; }


            if (panX != 0)
            {
                float deltaX = panX - x;
                float deltaY = panY - y;
                deltaX *= 0.2f;
                deltaY *= 0.2f;
                x += deltaX;
                y += deltaY;

                panStepsTaken += 1;
                if (panStepsTaken >= panStepsToTake)
                {
                    panX = 0;
                    panY = 0;
                    panStepsTaken = 0;
                }
                lastMapChange += 1;
            }

            checkLoaded();

            foreach (GraphicalHex hex in loaded)
            {
                hex.loc = getLoc(hex);
            }
        }

        public static void panTo(int panToX, int panToY)
        {
            panX = panToX;
            panY = panToY;
            panStepsTaken = 0;
        }

        public static void purge()
        {
            for (int x = 0; x < world.map.grid.Length; x++)
            {
                for (int y = 0; y < world.map.grid[0].Length; y++)
                {
                    world.map.grid[x][y].outer = null;
                    if (world.map.grid[x][y].settlement != null && world.map.grid[x][y].settlement.embeddedUnit != null)
                    {
                        world.map.grid[x][y].settlement.embeddedUnit.outer = null;
                    }
                }
            } 
            foreach (Unit u in map.units)
            {
                u.outer = null;
            }
            List<GraphicalHex> unNeeded = new List<GraphicalHex>();
            foreach (GraphicalHex hex in loaded)
            {
                unNeeded.Add(hex);
                hex.hex.outer = null;
                unNeeded.Add(hex);
            }
            foreach (GraphicalHex hex in unNeeded)
            {
                hex.needed = false;
                loaded.Remove(hex);
            }
        }

        public static Vector3 getLoc(GraphicalHex hex)
        {
            float off = 0;
            if (hex.hex.y % 2 == 1) { off = hexSizeX / 2; }
            return new Vector3(
                (-(hex.hex.x * hexSizeX) + (x * hexSizeX) + off) * scale,
                (-(hex.hex.y * hexSizeY) + (y * hexSizeY)) * scale,
                0
                );
        }
        public static Vector3 getLoc(Hex hex)
        {
            float off = 0;
            if (hex.y % 2 == 1) { off = hexSizeX / 2; }
            return new Vector3(
                (-(hex.x * hexSizeX) + (x * hexSizeX) + off) * scale,
                (-(hex.y * hexSizeY) + (y * hexSizeY)) * scale,
                0
                );
        }

        public static void checkData()
        {

            foreach (GraphicalHex hex in loaded)
            {
                hex.checkData();
                
                if (hex.hex.location != null)
                {
                    /*
                    foreach (Property u in hex.hex.location.getProperties())
                    {
                        if (u.outer != null)
                        {
                            u.outer.checkData();
                        }
                        else
                        {
                            world.prefabStore.getGraphicalProperty(u);
                        }
                    }
                    */
                    foreach (Unit u in hex.hex.location.units)
                    {
                        if (u.outer != null)
                        {
                            u.outer.checkData();
                        }
                    }
                }
            }
            
            
            //world.ui.uiLeftPrimary.checkData();
            //world.ui.uiScrollables.checkData();
            //world.ui.uiMidTop.checkData();
        }

        public static void checkLoaded()
        {
            int twiceDraw = drawDist * 2;

            int iX = (int)x;
            int iY = (int)y;
            foreach (GraphicalHex hex in loaded) { hex.needed = false; }
            for (int i = 0; i < twiceDraw; i++)
            {
                for (int j = 0; j < twiceDraw; j++)
                {
                    int mX = iX + i - drawDist;
                    int mY = iY + j - drawDist;

                    if (map.canGet(mX, mY))
                    {
                        if (map.grid[mX][ mY].outer == null)
                        {
                            map.grid[mX][ mY].outer = world.prefabStore.getGraphicalHex(map.grid[mX][ mY]);
                            map.grid[mX][ mY].outer.transform.position = getLoc(map.grid[mX][ mY].outer);
                            loaded.AddLast(map.grid[mX][ mY].outer);
                            if (map.grid[mX][ mY].location != null)
                            {
                                foreach (Link link in map.grid[mX][ mY].location.links)
                                {
                                    if (
                                        link.other(map.grid[mX][ mY].location).index >
                                        link.other(link.other(map.grid[mX][ mY].location)).index) { continue; }
                                    GraphicalLink grLink = world.prefabStore.getGraphicalLink(link);
                                    grLink.transform.parent = map.grid[mX][ mY].outer.transform;
                                }
                            }
                        }
                        map.grid[mX][ mY].outer.needed = true;
                    }
                }
            }

            List<GraphicalHex> unNeeded = new List<GraphicalHex>();
            foreach (GraphicalHex hex in loaded)
            {
                if (hex.needed == false)
                {
                    unNeeded.Add(hex);
                    hex.hex.outer = null;
                    unNeeded.Add(hex);
                }
            }
            foreach (GraphicalHex hex in unNeeded)
            {
                loaded.Remove(hex);
            }
        }

        public static GraphicalHex getHexUnderMouse(Vector3 mousePos)
        {
            float dist = 0;
            GraphicalHex closest = null;
            float minDist = 1000000;

            foreach (GraphicalHex hex in loaded)
            {
                Vector3 line = mousePos - world.outerCamera.WorldToScreenPoint(hex.transform.position);
                dist = line.magnitude;
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = hex;
                }
            }

            return closest;
        }
    }
}
