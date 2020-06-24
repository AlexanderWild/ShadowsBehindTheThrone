using UnityEngine;


namespace Assets.Code
{
    public class Abu_Base_EnthrallNoble: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            //u.task = new Task_SpreadShadow();

            //u.location.map.world.prefabStore.popImgMsg(u.getName() + " beings darkening the soul of " + u.location.person().getFullName() + "." +
            //    " This will take " + map.param.unit_spreadShadowTime + " turns, after which " + u.location.person().getFullName() + "'s shadow will increase by "
            //    + (int)(100*map.param.unit_spreadShadowAmount) + "%. This will leave " + ((int)(100*World.staticMap.param.unit_spreadShadowEvidence))+ "  evidence.",
            //    u.location.map.world.wordStore.lookup("ABILITY_UNIT_SPREAD_SHADOW"));

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.person == null) { return false; }
            if (u.location.person() == null) { return false; }
            if (u.location.settlement == null) { return false; }
            if (u.location.map.overmind.enthralled != null) { return false; }
            if (u.location.settlement.infiltration < World.staticMap.param.ability_unit_enthrallNobleReq) { return false; }
            return true;
        }

        public override void cast(Map map, Hex hex) { }
        public override bool castable(Map map, Hex hex)
        {
            return false;
        }

        public override int getCost()
        {
            return 0;
        }

        public override int getCooldown()
        {
            return 0;
        }

        public override string getDesc()
        {
            return "Takes full control of a noble, allowing you to vote through them, and to access the advanced abilities associated with aristocratic society."
                + "\n[Requires no other enthralled noble and infiltration above " + (int)(100*World.staticMap.param.ability_unit_spreadShadowInfiltrationReq) + "%]";
        }

        public override string getName()
        {
            return "Enthrall Noble";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_enshadow;
        }
    }
}