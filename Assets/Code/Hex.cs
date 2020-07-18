using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    //[Serializable,HideInInspector]
    public class Hex
    {
        public GraphicalHex outer;
        public Map map;
        public int x;
        public int y;
        public Location location;
        public enum terrainType { SEA, MOUNTAIN, GRASS, PATH,  SWAMP, MUD, DESERT, DRY, WETLAND, TUNDRA, SNOW };
        public terrainType terrain;
        public Location territoryOf;
        public Flora flora;

        public float purity = 1;

        public float transientTempDelta = 0;

        public float tX;
        public float tY;
        public int graphicalIndexer;

        public Province province;
        public static bool assertArmyPresence = true;

        public Hex() { }
        public Hex(int lX, int lY, Map mMap)
        {
            this.map = mMap;
            this.x = lX;
            this.y = lY;

            tX = x;
            tY = y;
            if (y % 2 == 0)
            {
                tX = x + 0.5f;
            }
            graphicalIndexer = Eleven.random.Next(10000);
        }

        public Settlement settlement
        {
            get
            {
                if (location == null) { return null; }
                else { return location.settlement; }
            }
        }
        public SocialGroup owner
        {
            get
            {
                if (territoryOf == null) { return null; }
                //if (territoryOf.settlement == null) { return null;}
                return territoryOf.soc;
            }
        }

        public string getName()
        {
            string reply = "";
            if (settlement != null) { reply += settlement.name; }
            else if (location != null) { reply += location.name; }
            else { reply += "Empty Hex"; }
            reply += " (" + x + "," + y + ")";
            return reply;
        }

        public float getTemperature()
        {
            float temp = map.tempMap[x][y];

            temp += transientTempDelta;
            temp += map.globalTemporaryTempDelta;
            if (temp < 0) { temp = 0; }
            if (temp > 1) { temp = 1; }
            return temp;
        }
        public float getHabilitability()
        {
            if (terrain == terrainType.MOUNTAIN) { return 0; }

            float basic = getTemperature() - 0.5f;//Now 0.5f to -0.5f
            basic *= 2;//-1 to 1
            float dist = Math.Abs(basic);//1 to 1
            dist /= 0.8f;
            float hab = 1 - dist;
            if (hab < 0) { hab = 0; }
            return hab;
        }

        public void turnTick()
        {

            if (location != null)
            {
                location.turnTick();
            }

            if (settlement != null)
            {

            }
            else
            {
                List<Hex> neighbours = this.map.getNeighbours(this);
                float newPurity = 0;
                foreach (Hex h in neighbours)
                {
                    newPurity += h.purity;
                }
                newPurity /= neighbours.Count;
                purity = (purity + newPurity) / 2;
            }
            if (Math.Abs(transientTempDelta) < map.param.map_tempTemperatureReversion)
            {
                transientTempDelta = 0;
            }else if (transientTempDelta > 0)
            {
                transientTempDelta -= map.param.map_tempTemperatureReversion;
            }
            else
            {
                transientTempDelta += map.param.map_tempTemperatureReversion;
            }
        }

        public override string ToString()
        {
            return "Hex(" + x + "," + y + ")";
        }
    }
}
