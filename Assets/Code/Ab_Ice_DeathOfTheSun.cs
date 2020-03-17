using System.Collections.Generic;
using UnityEngine;


namespace Assets.Code
{
    public class Ab_Ice_DeathOfTheSun: Ability
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

            double scaling = (24d*32d)/ (map.grid.Length * map.grid[0].Length);
            World.log("Scaling " + scaling);
            float delta = (float)(map.param.ability_deathOfTheSunTempChange * scaling);
            int count = 0;

            int debug = Eleven.random.Next();
            foreach (Location loc in map.locations)
            {
                foreach (Property p in loc.properties)
                {
                    if (p.proto is Pr_RecentHumanBattle)
                    {
                        count += 1;
                    }
                }
            }
            delta *= count;

            foreach (Hex[] row in map.grid)
            {
                foreach (Hex h in row)
                {
                    h.temporaryTempDelta += delta;
                }
            }

            map.world.prefabStore.popImgMsg("The entire world cools, as the dead sap the life from the sun, leaving every location on the planet colder. " + count + " recent battles have been drawn upon.",
                map.world.wordStore.lookup("ABILITY_DEATH_OF_THE_SUN"));

            foreach (Location loc in map.locations)
            {
                Property rem = null;
                foreach (Property p in loc.properties)
                {
                    if (p.proto is Pr_RecentHumanBattle)
                    {
                        rem = p;
                    }
                }
                if (rem != null)
                {
                    loc.properties.Remove(rem);
                }
            }

            hex.map.assignTerrainFromClimate();
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            foreach (Property p in hex.location.properties){
                if (p.proto is Pr_RecentHumanBattle)
                {
                    return true;
                }
            }
            return false;
        }

        public override int getCost()
        {
            return World.staticMap.param.ability_deathOfTheSunCost;
        }

        public override int getCooldown()
        {
            return World.staticMap.param.ability_deathOfTheSunCooldown;
        }

        public override string getDesc()
        {
            return "Causes the worldwide temperature to drop. Consumes all 'Recent Human Battle' properties on all locations on the map, and decreases the temperature slightly for each of them."
                + "\n[Requires a site of a Recent Human Battle]";
        }

        public override string getName()
        {
            return "Death of the Sun";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_fog;
        }
    }
}