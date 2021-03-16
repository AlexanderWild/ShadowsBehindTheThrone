using UnityEngine;


namespace Assets.Code
{
    public class Abu_Base_SetLoose: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            map.overmind.availableEnthrallments -= 1;
            u.automated = true;
            u.location.map.world.prefabStore.popImgMsg(u.getName() + " is set loose on the world, to act independently as they see fit. They will attempt to infiltrate and spread shadow, but will be weak without your direct influence.",
                u.location.map.world.wordStore.lookup("ABILITY_UNIT_SET_LOOSE"));

        }
        public override bool castable(Map map, Unit u)
        {
            if (map.overmind.availableEnthrallments < 1) { return false; }
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
            return "Sets the unit free from your direct control at the cost of an enthrallment. It will attempt to infiltrate and spread shadow automatically, but is weakened and unreliable without your guidance. Allows you to enthrall other units instead."
                + "\n[Requires a dark agent and an enthrallment use]";
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