using UnityEngine;


namespace Assets.Code
{
    public class Ab_Easy_ReduceEvidence: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            castInner(map, hex.location.person());
        }
        public override void castInner(Map map, Person person)
        {
            
            map.world.prefabStore.popImgMsg(
                "You cast your shadow over " + person.getFullName() + " and hide away some of the evidence against them.",
                map.world.wordStore.lookup("ABILITY_REDUCE_EVIDENCE"));

            person.evidence /= 2;
        }

        public override bool castable(Map map, Person person)
        {
            return person.evidence > 0;
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.person() == null) { return false; }

            return (hex.location.person().evidence > 0);

        }

        public override int getCost()
        {
            return World.staticMap.param.ability_reduceEvidenceCost;
        }

        public override string getDesc()
        {
            return "Halves the amount of evidence on a given noble, reducing their risk of drawing suspicion from their fellow nobles."
                + "\n[Requires a noble with evidence]";
        }

        public override string getName()
        {
            return "Reduce Evidence";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_vampire;
        }
    }
}