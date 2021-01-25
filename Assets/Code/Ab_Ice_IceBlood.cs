using System.Collections.Generic;
using UnityEngine;


namespace Assets.Code
{
    public class Ab_Ice_IceBlood: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);
            if (!castable(map, hex)) { return; }


            List<Location> targets = new List<Location>();
            targets.Add(hex.location);
            foreach (Location loc in map.locations)
            {
                if (loc.settlement != null && loc.person() != null && loc.person().house == hex.location.person().house)
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
                        h.transientTempDelta += (float)map.param.ability_iceBloodTempChange;
                    }
                }
            }

            map.world.prefabStore.popImgMsg(hex.location.person().getFullName() + "'s sins freeze the world, the curse passing through to their bloodline." +
                " All those of house " + hex.location.person().house.name + " are affected, freezing the world in " + targets.Count + " locations."
                + "\n" + hex.location.getName() + " habitability goes from " + ((int)(prevHab)) + "% to " + ((int)(100 * hex.getHabilitability())) + "% habitable."
                + "\nTemperature changes from " + prevTemp + "% to " + ((int)(100 * hex.getTemperature())) + "% temperature.",
                map.world.wordStore.lookup("ABILITY_ICE_BLOOD"));

            hex.location.person().die("Died as their blood turned to ice under the weight of their sins.",true);
            hex.map.assignTerrainFromClimate();
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.person() == null) { return false; }
            if (hex.location.person().state != Person.personState.broken) { return false; }
            return true;
        }

        public override int getCost()
        {
            return World.staticMap.param.ability_iceBloodCost;
        }

        public override int getCooldown()
        {
            return World.staticMap.param.ability_iceBloodCooldown;
        }

        public override string getDesc()
        {
            return "Sacrifice a noble with a broken soul (usually caused by 100% shadow) in order to cool their settlement, as well as all others locations under the control of nobles of the same house."
                + "\n[Requires a settlement with a broken noble]";
        }

        public override string getName()
        {
            return "Ice in the Blood";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_snow;
        }
    }
}