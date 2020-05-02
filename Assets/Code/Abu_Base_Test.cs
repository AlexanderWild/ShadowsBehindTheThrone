using UnityEngine;


namespace Assets.Code
{
    public class Abu_Base_Test: Ability
    {

        public override void castInner(Map map, Unit u)
        {
            Evidence e = new Evidence(map.turn);
            e.weight = 1;
            e.pointsTo = u;
            u.location.evidence.Add(e);
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