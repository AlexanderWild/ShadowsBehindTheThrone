using UnityEngine;


namespace Assets.Code
{
    public class Ab_Easy_ReduceSuspicion: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            castInner(map, hex.location.person());
        }
        public override void castInner(Map map, Person person)
        {
            
            map.world.prefabStore.popImgMsg(
                person.getFullName() + "'s suspicions abate, they fear less now. All their suspicions are halved.",
                map.world.wordStore.lookup("ABILITY_REDUCE_SUSPICION"));
            foreach (RelObj rel in person.relations.Values){
                rel.suspicion /= 2;
            }
        }

        public override bool castable(Map map, Person person)
        {
            foreach (RelObj rel in person.relations.Values)
            {
                if (rel.suspicion > 0) { return true; }
            }
            return false;
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.person() == null) { return false; }

            return castable(map, hex.location.person());

        }

        public override int getCost()
        {
            return World.staticMap.param.ability_reduceSuspicionCost;
        }

        public override string getDesc()
        {
            return "Halves the amount of suspicion a noble has towards other nobles, reducing how much they believe they are corrupted by the darkness."
                + "\n[Requires a noble with suspicions]";
        }

        public override string getName()
        {
            return "Reduce Suspicion";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_vampire;
        }
    }
}