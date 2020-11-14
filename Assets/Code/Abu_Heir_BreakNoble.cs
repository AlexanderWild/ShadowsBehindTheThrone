using UnityEngine;


namespace Assets.Code
{
    public class Abu_Heir_BreakNoble: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            if (u.location.person() != null)
            {
                u.location.person().state = Person.personState.broken;
            }
            else
            {
                map.world.prefabStore.popMsg("No noble present in this location.");
                return;
            }

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " shatters the soul of " + u.location.person().getFullName() + " and brings them into your cult. They will not oppose the dark forces," +
                " and do not care about evidence against your enthralled nobles or agents.",
                u.location.map.world.wordStore.lookup("ABILITY_UNIT_BREAK_NOBLE"),4);
        }
        public override bool castable(Map map, Unit u)
        {
            if (u.person == null) { return false; }
            if (u.location.person() == null) { return false; }
            if (u.location.settlement == null) { return false; }
            if (u.location.settlement.infiltration < World.staticMap.param.ability_unit_breakNobleReq) { return false; }
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
            return "Breaks the soul of a noble, adding them to your cult and preventing them from caring about evidence of darkness."
                + "\n[Requires infiltration above " + (int)(100*World.staticMap.param.ability_unit_breakNobleReq) + "%]";
        }

        public override string getName()
        {
            return "Break Soul";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_enshadow;
        }
    }
}