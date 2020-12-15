using UnityEngine;


namespace Assets.Code
{
    public class Abu_Vamp_Infiltrate: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            u.task = new Task_Vamp_Infiltrate();

            Unit_Vampire vamp = (Unit_Vampire)u;
            vamp.blood -= World.staticMap.param.ability_unit_bloodCostInfiltrate;

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " begins infiltrating "+ u.location.person().getFullName() + ". If successful your infiltration level will increase by " + 
                (int)(100* Task_Vamp_Infiltrate.getEffectiveness(u)) + "%." +
                " Security will reduce the amount of infiltration gained. No evidence will be left behind."
                + " If the noble likes your vampire this infiltration will be more effective, but will reduce if they dislike them.",
                u.location.map.world.wordStore.lookup("ABILITY_UNIT_INFILTRATE"));

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.location.person() == null) { return false; }
            if (u.location.soc == null) { return false; }
            if (u.location.soc is Society == false) { return false; }
            return true;
        }

        public override void cast(Map map, Hex hex) { }
        public override bool castable(Map map, Hex hex)
        {
            return false;
        }

        public override string specialCost()
        {
            return World.staticMap.param.ability_unit_bloodCostInfiltrate + " blood";
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
            return "Begins a " + World.staticMap.param.unit_infiltrateTime + " turn task which will result in your infiltration level increasing." +
                " Effectiveness reduces based on security level, and increased by noble's liking."
                + "\n[Requires a society-held location]";
        }

        public override string getName()
        {
            return "Infiltrate";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_vampire;
        }
    }
}