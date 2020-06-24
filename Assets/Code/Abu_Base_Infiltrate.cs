using UnityEngine;


namespace Assets.Code
{
    public class Abu_Base_Infiltrate: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            u.task = new Task_Infiltrate();

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " beings infiltrating "+ u.location.person().getFullName() + ". If successful your infiltration level will increase by " + 
                (int)(100*Task_Infiltrate.getEffectiveness(u)) + "%." +
                " Security will reduce the amount of infiltration gained, and " + (int)(100*World.staticMap.param.unit_infiltrateEvidence) + "% evidence will be left behind."
                + " If the noble likes your agent this infiltration will be more effective, but will reduce if they dislike them.",
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
            return map.world.textureStore.icon_mask;
        }
    }
}