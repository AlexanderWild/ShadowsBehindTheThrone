using UnityEngine;


namespace Assets.Code
{
    public class Abu_Vamp_Insanity: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            u.task = new Task_Vamp_Insanity();

            Unit_Vampire vamp = (Unit_Vampire)u;
            vamp.blood -= World.staticMap.param.ability_unit_bloodCostInsanity;

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " beings imposing their dark will on the mind of "+ u.location.person().getFullName() + ". Each turn, they will lose sanity" +
                " until their mind snaps.",
                u.location.map.world.wordStore.lookup("ABILITY_VAMP_INSANITY"));

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.location.person() == null) { return false; }
            if (u.location.soc == null) { return false; }
            if (u.location.soc is Society == false) { return false; }
            if (u.location.person().sanity <= 0) { return false; }
            if (u.location.settlement.infiltration < World.staticMap.param.ability_unit_insanityInfiltrationReq) { return false; }
            return true;
        }

        public override void cast(Map map, Hex hex) { }
        public override bool castable(Map map, Hex hex)
        {
            return false;
        }

        public override string specialCost()
        {
            return World.staticMap.param.ability_unit_bloodCostInsanity + " blood\nNo Evidence";
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
            return "Begins a " + World.staticMap.param.unit_infiltrateTime + " task which will cause the local noble to lose 1 sanity each turn, as long as the vampire remains unmoving."
                + "\n[Requires a location with a non-insane noble with infiltration > " + (int)(100* World.staticMap.param.ability_unit_insanityInfiltrationReq) + "%]";
        }

        public override string getName()
        {
            return "Drive to Madness";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_vampire;
        }
    }
}