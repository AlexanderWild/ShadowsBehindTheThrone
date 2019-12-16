using UnityEngine;


namespace Assets.Code
{
    public class Ab_Enth_MiliaryAid : Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            Property.addProperty(map, map.overmind.enthralled.getLocation(), "Military Aid Outgoing");
            Property.addProperty(map, hex.location, "Military Aid");

            map.world.prefabStore.popImgMsg(
                "You add military support to " + hex.getName() + ". This will increase its military cap, allowing more levies, once the men-at-arms are recruited. It also gives aid to nobles who"
                + " would rebel against their nation.",
                map.world.wordStore.lookup("ABILITY_MILITARY_AID"));
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.map.overmind.enthralled == null) { return false; }
            if (hex.location == null) { return false; }
            if (hex.location.settlement == null) { return false; }
            if (hex.location.settlement.getMilitaryCap() < map.param.ability_militaryAidAmount) { return false; }
            if (hex.location.soc == null || (hex.location.soc is Society == false)) { return false; }
            if (hex.location.person() != null && hex.location.person().state == Person.personState.enthralled) { return false; }

            return true;

        }

        public override int getCost()
        {
            return World.staticMap.param.ability_militaryAidCost;
        }

        public override string getDesc()
        {
            return "Transfers military capacity from your enthralled's city to another location, granting it +" + World.staticMap.param.ability_militaryAidAmount + " military cap for " + World.staticMap.param.ability_militaryAidDur + " turns."
                + " Useful for creating civil wars by arming dissidents."
                + "\n[Requires another noble, and your enthralled to have available military capacity]";
        }

        public override string getName()
        {
            return "Covert Military Aid";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_shield;
        }
    }
}