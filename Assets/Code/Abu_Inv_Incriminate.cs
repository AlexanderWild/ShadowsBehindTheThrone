using UnityEngine;


namespace Assets.Code
{
    public class Abu_Inv_Incriminate: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {

            Evidence e2 = new Evidence(map.turn);
            e2.pointsTo = u;
            e2.weight = 0.25;
            u.location.evidence.Add(e2);

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " plants evidence on " + u.location.person().getFullName() + ", increasing their evidence by 10%." +
                " They also leave behind evidence incriminating themselves (evidence strength 25%).",
                u.location.map.world.wordStore.lookup("ABILITY_UNIT_FALSE_EVIDENCE"));

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.location.person() == null) { return false; }
            if (u.location.settlement == null) { return false; }
            if (u.location.settlement.infiltration < map.param.ability_unit_falseEvidenceInfiltration) { return false; }
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
            return "Increases the evidence on a noble by 10%. Places findable evidence against your investigator (25%)"
                + "\n[Requires infiltration above " + (int)(100*World.staticMap.param.ability_unit_falseEvidenceInfiltration) + "%]";
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