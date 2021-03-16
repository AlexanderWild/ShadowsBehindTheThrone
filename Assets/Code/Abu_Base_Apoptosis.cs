using UnityEngine;


namespace Assets.Code
{
    public class Abu_Base_Apoptosis: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            u.die(map, "Was of no further use");
            GraphicalMap.selectedSelectable = null;
            u.location.map.world.prefabStore.popImgMsg(u.getName() + " is no longer of use. The night swallows them up, allowing someone new to take their place.",
                u.location.map.world.wordStore.lookup("ABILITY_UNIT_APOPTOSIS"));

        }
        public override bool castable(Map map, Unit u)
        {
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
            return "Kills this agent, so they may be replaced by someone more useful"
                + "\n[Requires a dark agent]";
        }

        public override string specialCost()
        {
            return " ";
        }
        public override string getName()
        {
            return "Apoptosis";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_enshadow;
        }
    }
}