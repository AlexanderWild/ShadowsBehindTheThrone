using UnityEngine;


namespace Assets.Code
{
    public class Ab_Test: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            World.log("Test ability cast on hex " + hex.getName());
        }

        public override bool castable(Map map, Hex hex)
        {
            return true;
        }

        public override int getCost()
        {
            return 32;
        }

        public override string getDesc()
        {
            return "A demonstration ability, used to demonstrate how an ability could theoretically be used";
        }

        public override string getName()
        {
            return "Demonstration ability one. Long name.";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_convert;
        }
    }
}