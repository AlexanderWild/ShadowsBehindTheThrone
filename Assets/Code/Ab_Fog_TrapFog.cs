using System.Collections.Generic;
using UnityEngine;


namespace Assets.Code
{
    public class Ab_Fog_TrapFog: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);
            if (!castable(map, hex)) { return; }

            Property.addProperty(map, hex.location, "Trapped Fog");
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.cloud == null || (hex.cloud is Cloud_Fog == false)) { return false; }
            return true;
        }

        public override int getCost()
        {
            return World.staticMap.param.ability_fog_trapCost;
        }

        public override int getCooldown()
        {
            return 0;
        }

        public override string getDesc()
        {
            return "Force fog to remain in its current location, for " + World.staticMap.param.ability_fog_trapDuration + " turns, ensuring your agents can take advantage of it without risk of it dissipating."
                + "\n[Requires a location current under fog]";
        }

        public override string getName()
        {
            return "Trap Fog";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_fog;
        }
    }
}