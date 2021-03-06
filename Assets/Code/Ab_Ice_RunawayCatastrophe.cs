using System.Collections.Generic;
using UnityEngine;


namespace Assets.Code
{
    public class Ab_Ice_RunawayCatastrophe: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);
            if (!castable(map, hex)) { return; }

            List<Location> targets = new List<Location>();
            targets.Add(hex.location);
            foreach (Location loc in hex.location.getNeighbours())
            {
                targets.Add(loc);
            }

            int prevHab = (int)(100 * hex.getHabilitability());
            int prevTemp = (int)(100 * hex.getTemperature());

            foreach (Hex[] row in map.grid)
            {
                foreach (Hex h in row)
                {
                    h.transientTempDelta += (float)(map.param.ability_runawayCatastrophyTempChange);
                }
            }

            map.world.prefabStore.popImgMsg("As the world begins to freeze, the white snow and ice reflect more of the incoming sunlight back into the sky, increasing the rate of cooling world-wide.",
                map.world.wordStore.lookup("ABILITY_RUNAWAY_CATASTROPHE"));


            hex.map.assignTerrainFromClimate();
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            float sum = 0;
            int div = 0;
            //foreach (Hex[] row in map.grid)
            //{
            //    foreach (Hex h in row)
            //    {
            //        div += 1;
            //        sum += h.transientTempDelta;
            //    }
            //}

            foreach (Location loc in World.staticMap.locations)
            {
                if (loc.isOcean) { continue; }
                div += 1;
                sum += loc.hex.transientTempDelta;
            }
            if (div > 0) { sum /= div; }
            return sum < World.staticMap.param.ability_runawayCatastropheRequirement;
        }

        public override int getCost()
        {
            return World.staticMap.param.ability_runawayCatastropheCost;
        }

        public override int getCooldown()
        {
            return World.staticMap.param.ability_runawayCatastropheCooldown;
        }

        public override string getDesc()
        {
            int div = 0;
            float sum = 0;
            //foreach (Hex[] row in World.staticMap.grid)
            //{
            //    foreach (Hex h in row)
            //    {
            //        if (h.terrain == Hex.terrainType.SEA) { continue; }
            //        div += 1;
            //        sum += h.transientTempDelta;
            //    }
            //}
            foreach (Location loc in World.staticMap.locations)
            {
                if (loc.isOcean) { continue; }
                div += 1;
                sum += loc.hex.transientTempDelta;
            }
            if (div > 0) { sum /= div; }

            return "If you have already cooled the world sufficiently using other powers, you may begin to spend power directly to continue the cooling effects world-wide"
                + "\n[Requires world cooling to be above "+  (int)(World.staticMap.param.ability_runawayCatastropheRequirement*100) + "% (Currently " + (int)(sum * 100) + "%)]";
        }

        public override string getName()
        {
            return "Runaway Catastrophe";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_snow;
        }
    }
}