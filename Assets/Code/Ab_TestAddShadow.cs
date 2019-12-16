using UnityEngine;


namespace Assets.Code
{
    public class Ab_TestAddShadow : Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            hex.location.settlement.title.heldBy.shadow = 1;
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.settlement == null) { return false; }
            if (hex.location.settlement.title == null) { return false; }
            if (hex.location.settlement.title.heldBy == null) { return false; }
            return true;
        }

        public override int getCost()
        {
            return 0;
        }

        public override string getDesc()
        {
            return "Sets person's shadow to 1.0.";
        }

        public override string getName()
        {
            return "Set shadow to 1";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_enshadow;
        }
    }
}