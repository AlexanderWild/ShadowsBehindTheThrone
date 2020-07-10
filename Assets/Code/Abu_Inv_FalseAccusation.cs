using UnityEngine;


namespace Assets.Code
{
    public class Abu_Base_FalseAccusation: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            Unit_Investigator inv = (Unit_Investigator)u;
            u.task = new Task_PleadCase();

            double effect = 0;
            Person p = u.location.person();
            double liking = p.getRelation(u.person).getLiking();//0 to 100
            double infiltration = u.location.settlement.infiltration * 100;//Also 0 to 100
            effect = (liking + infiltration) * map.param.ability_unit_falseAccusationEffect;

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " accuses " + inv.victim.getName() + " of being in league with shadow.",
                u.location.map.world.wordStore.lookup("ABILITY_UNIT_PLEAD_CASE"));

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.location.person() == null) { return false; }
            if (u is Unit_Investigator == false) { return false; }
            Unit_Investigator u2 = (Unit_Investigator)u;
            return u2.victim != null;
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
            return "Accuses an agent your investigator has false evidence against of being in league with the shadow. The effect of this will increase with noble's liking and with infiltration."
                + "\n[Requires a noble and your investigator to have produced false evidence]";
        }

        public override string getName()
        {
            return "False Accusation";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_mask;
        }
    }
}