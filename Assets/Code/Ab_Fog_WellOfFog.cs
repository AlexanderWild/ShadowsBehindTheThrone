using System.Collections.Generic;
using UnityEngine;


namespace Assets.Code
{
    public class Ab_Fog_WellOfFog: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);
            if (!castable(map, hex)) { return; }

            Property.addProperty(map, hex.location, "Well of Fog");
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.person() == null) { return false; }
            if (hex.location.person().state == Person.personState.broken) { return true; }
            if (hex.location.person().state == Person.personState.enthralled) { return true; }
            if (hex.location.person().shadow >= 0.5) { return true; }
            return false;
        }

        public override int getCost()
        {
            return World.staticMap.param.ability_fog_wellOfFog;
        }

        public override int getCooldown()
        {
            return 0;
        }

        public override string getDesc()
        {
            return "Creates a well of Fog, which will cover this location and nearby others in fog, reducing the security of the locations caught within and allowing your Fog abilities to be used"
                + "\n[Requires a location with a noble who is enthralled, broken or has shadow > 50%]";
        }

        public override string getName()
        {
            return "Well of Fog";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_fog;
        }
    }
}