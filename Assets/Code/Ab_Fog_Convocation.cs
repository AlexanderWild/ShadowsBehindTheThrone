using System.Collections.Generic;
using UnityEngine;


namespace Assets.Code
{
    public class Ab_Fog_Convocation: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);
            if (!castable(map, hex)) { return; }

            List<Unit> movers = new List<Unit>();
            foreach (Location loc in map.locations)
            {
                if (loc.hex.cloud is Cloud_Fog)
                {
                    foreach (Unit u in loc.units)
                    {
                        if (u.isEnthralled() && !u.isMilitary)
                        {
                            movers.Add(u);
                        }
                    }
                }
            }
            foreach (Unit u in movers)
            {
                u.location.units.Remove(u);
                hex.location.units.Add(u);
                u.location = hex.location;
                if (u.task is Task_Disrupted == false)
                {
                    u.task = null;
                }
            }

            map.world.prefabStore.popImgMsg(
                 movers.Count + " agent(s) have been moved to " + hex.location.getName() + ", travelling swiftly throughout the fog's warped geometries.",
                map.world.wordStore.lookup("FOG_CONVOCATION"),6);
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.cloud == null) { return false; }
            if (hex.cloud is Cloud_Fog == false) { return false; }
            return true;
        }

        public override int getCost()
        {
            return World.staticMap.param.ability_fog_convocationCost;
        }

        public override int getCooldown()
        {
            return World.staticMap.param.ability_fog_convocationCost;
        }

        public override string getDesc()
        {
            return "Causes all your agents in locations in the fog to instantly move to the selected fog-covered location."
                + "\n[Requires a location in the fog]";
        }

        public override string getName()
        {
            return "Convocation";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_fog;
        }
    }
}