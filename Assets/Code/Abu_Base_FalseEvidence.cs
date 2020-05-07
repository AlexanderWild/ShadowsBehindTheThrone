using UnityEngine;


namespace Assets.Code
{
    public class Abu_Base_FalseEvidence: Ability
    {

        public override void castInner(Map map, Unit u)
        {

            Evidence e2 = new Evidence(map.turn);
            e2.pointsTo = u;
            e2.weight = 0.5;
            u.location.evidence.Add(e2);

            Evidence e1 = new Evidence(map.turn);
            e1.pointsToPerson = u.location.person();
            e1.weight = 0.5;
            u.location.evidence.Add(e1);

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " plants evidence against " + u.location.person().getFullName() + ", which will be found by investigators (evidence strength 50%)." +
                " They also leave behind evidence incriminating themselves (evidence strength 50%).",
                u.location.map.world.wordStore.lookup("ABILITY_FALSE_EVIDENCE"));

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.location.person() == null) { return false; }
            if (u.location.person().titles.Count > 0) { return false; }
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
            return World.staticMap.param.unit_falseEvidenceCooldown;
        }

        public override string getDesc()
        {
            return "Places a piece of evidence against a low-ranking noble which will lead to 50% suspicion against them by any investigator which finds it. Places similar evidence against your investigator"
                + "\n[Requires a noble without an elected title]";
        }

        public override string getName()
        {
            return "False Evidence";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_mask;
        }
    }
}