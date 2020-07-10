using UnityEngine;


namespace Assets.Code
{
    public class Abu_Inv_ProduceFalseEvidence: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            Unit_Investigator inv = (Unit_Investigator)u;

            foreach (Unit u2 in u.location.units)
            {
                if (u2 != u)
                {
                    inv.victim = u2;
                }
            }

            inv.victimUses = map.param.ability_unit_falseAccusationCharges;

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " gathers personal items of " + inv.victim.getName() + ", and can now falsely accuse them to nobles (" + inv.victimUses + " uses until they will need more fake evidence).",
                u.location.map.world.wordStore.lookup("ABILITY_UNIT_PRODUCE_FALSE_EVIDENCE"));

        }
        public override bool castable(Map map, Unit u)
        {
            if (u is Unit_Investigator == false) { return false; }
            return u.location.units.Count == 2;
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
            return "Secretly gather items which can be used to accuse another agent, using your false accusation ability."
                + "\n[Requires your investigator to be alone in a location with the victim unit]";
        }

        public override string getName()
        {
            return "Produce False Evidence";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_mask;
        }
    }
}