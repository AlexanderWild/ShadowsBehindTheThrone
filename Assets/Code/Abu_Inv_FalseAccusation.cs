using UnityEngine;


namespace Assets.Code
{
    public class Abu_Inv_FalseAccusation: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            Unit_Investigator inv = (Unit_Investigator)u;

            double effect = 0;
            Person p = u.location.person();
            double liking = p.getRelation(u.person).getLiking();//0 to 100
            double infiltration = u.location.settlement.infiltration * 100;//Also 0 to 100
            effect = (liking + infiltration + 100) * map.param.ability_unit_falseAccusationEffect;
            if (effect < 0) { effect = 0; }
            effect /= 100;
            if (effect > 1) { effect = 1; }

            p.getRelation(inv.victim.person).suspicion += effect;
            if (p.getRelation(inv.victim.person).suspicion > 1) { p.getRelation(inv.victim.person).suspicion = 1; }


            string add = "";
            inv.victimUses -= 1;
            if (inv.victimUses <= 0)
            {
                add += "\nThey have used all the false evidence they produced, and now must collect more from their victim to continue.";
            }

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " accuses " + inv.victim.getName() + " of being in league with shadow, presenting false evidence to " + p.getFullName()
                + "\nTheir suspicion rises by "+  (int)(100*effect) + "% (after applying infiltration and liking bonuses), and is now " + (int)(100*p.getRelation(inv.victim.person).suspicion) + "%." + add,
                u.location.map.world.wordStore.lookup("ABILITY_UNIT_FALSE_ACCUSE"));

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.location.person() == null) { return false; }
            if (u is Unit_Investigator == false) { return false; }
            Unit_Investigator u2 = (Unit_Investigator)u;
            if (u2.victimUses <= 0) { return false; }
            return u2.victim != null && u2.victim.person != null;
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