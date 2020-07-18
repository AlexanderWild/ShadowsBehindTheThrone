using System.Collections.Generic;
using UnityEngine;


namespace Assets.Code
{
    public class Ab_Ice_ColdAsDeath: Ability
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
                    if (targets.Contains(h.territoryOf))
                    {
                        h.transientTempDelta += (float)map.param.ability_coldAsDeathTempChange;
                    }
                }
            }

            map.world.prefabStore.popImgMsg("The world cools around " + hex.location.getName() + ", dropping the temperature in " + targets.Count + " locations."
                + "\n" + hex.location.getName() + " habitability goes from " + ((int)(prevHab)) + "% to " + ((int)(100*hex.getHabilitability())) + "% habitable."
                + "\nTemperature changes from " + prevTemp + "% to " + ((int)(100*hex.getTemperature())) + "% temperature.",
                map.world.wordStore.lookup("ABILITY_COLD_AS_DEATH"));

            Property rem = null;
            foreach (Property p in hex.location.properties)
            {
                if (p.proto is Pr_RecentHumanBattle)
                {
                    rem = p;
                }
            }
            hex.location.properties.Remove(rem);
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
            return World.staticMap.param.ability_coldAsDeathCost;
        }

        public override int getCooldown()
        {
            return World.staticMap.param.ability_coldAsDeathCooldown;
        }

        public override string getDesc()
        {
            return "Causes the dead of a recent battle to drain warmth from the land, potentially rendering it uninhabitable to human life, if the temperature drops low enough."
                + "\n[Requires a site of a Recent Human Battle]";
        }

        public override string getName()
        {
            return "Cold As Death";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_fog;
        }
    }
}