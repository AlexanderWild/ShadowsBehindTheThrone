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

            map.world.prefabStore.popImgMsg(
                "This location is now a source of fog. Fog will pour out from here, as long as the noble remains under your influence. Fog decreases locations' security, " +
                "allows your agents to operate in areas they were exiled from and slows the rate at which nobles become suspicious.",
                map.world.wordStore.lookup("FOG_WELL_OF_FOG"),6);
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
            return World.staticMap.param.ability_fog_wellOfFogCost;
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