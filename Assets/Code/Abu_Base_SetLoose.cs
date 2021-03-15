using UnityEngine;


namespace Assets.Code
{
    public class Abu_Base_SetLoose: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            u.automated = true;
            u.location.map.world.prefabStore.popImgMsg(u.getName() + " is set loose on the world, to act independently as they see if. They will attempt to infiltrate and spread shadow, but will be weak without your direct influence.",
                u.location.map.world.wordStore.lookup("ABILITY_UNIT_SET_LOOSE"));

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
            return "Sets the unit free from your direct control. It will attempt to infiltrate and spread shadow automatically, but is weakened and unreliable without your guidance."
                + "\n[Requires a dark agent]";
        }

        public override string specialCost()
        {
            return " ";
        }
        public override string getName()
        {
            return "Set Loose";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_enshadow;
        }
    }
}