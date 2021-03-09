using System.Collections.Generic;
using UnityEngine;


namespace Assets.Code
{
    public class Ab_Ice_Blizzard: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);
            if (!castable(map, hex)) { return; }

            Property.addProperty(hex.map,hex.location, "Blizzard");

            hex.location.recomputeLinkDisabling();
            foreach (Link l in hex.location.links)
            {
                l.other(hex.location).recomputeLinkDisabling();
            }

            int nDisabled = 0;
            Location disabledOther = null;
            foreach (Link l in hex.location.links)
            {
                if (l.disabled) { nDisabled += 1; disabledOther = l.other(hex.location); }
            }

            string add = "";
            if (nDisabled == 1) { add = "\nIts links to "  + disabledOther.getName() + " has been severed."; }
            if (nDisabled > 1) { add = "\nIts links to " + nDisabled + " other locations has been severed."; }
            map.world.prefabStore.popImgMsg("The snow falls deep in " + hex.location.getName() + ", and is driven by icy winds. Roads risk being inundated beneath impassable walls of snowfall if the blizzard moves across the land to nearby settlements."
               + add,
                map.world.wordStore.lookup("ABILITY_ICE_BLIZZARD"));

        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.terrain == Hex.terrainType.SNOW) { return true; }
            if (hex.terrain == Hex.terrainType.TUNDRA) { return true; }
            return false;
        }

        public override int getCost()
        {
            return World.staticMap.param.ability_blizzardCost;
        }

        public override int getCooldown()
        {
            return World.staticMap.param.ability_blizzardCooldown;
        }

        public override string getDesc()
        {
            return "Causes a blizzard in this location. If two locations both are experiencing a blizzard, any connection between them will be cut for the duration, preventing units from travelling (CANNOT CAUSE A LOCATION TO HAVE NO NEIGHBOURS)."
                + "\n[Requires a location in a hex with snowfall]";
        }

        public override string getName()
        {
            return "Blizzard";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_snow;
        }
    }
}