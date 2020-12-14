using UnityEngine;


namespace Assets.Code
{
    public class Abu_Base_ChangeIdentity: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            u.task = new Task_ChangeIdentity();

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.location.settlement == null) { return false; }
            if (u.location.soc is Society == false) { return false; }
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

        public override string specialCost()
        {
            return "Major Evidence";
        }

        public override string getDesc()
        {
            return "Changes this agent's paperwork, allowing them to be forgotten by nobles, and allows them to enter societies they have been exiled from. Leaves major evidence behind."
                + "\n[Requires a human settlement in a society]";
        }

        public override string getName()
        {
            return "Adopt New Identity";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_mask;
        }
    }
}