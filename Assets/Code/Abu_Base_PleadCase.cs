using UnityEngine;


namespace Assets.Code
{
    public class Abu_Base_PleadCase: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            u.task = new Task_PleadCase();

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " begins presenting the case for your enthralled nobles and agents' innocent, or at least their lesser guilt." +
                " This will take " + map.param.unit_pleadCaseTime + " turns, after which " + u.location.person().getFullName() + "'s suspicion towards all enthralled or broken nobles and units will halve.",
                u.location.map.world.wordStore.lookup("ABILITY_UNIT_PLEAD_CASE"));

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.location.person() == null) { return false; }
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
            return "Begins a " + World.staticMap.param.unit_pleadCaseTime + " turn task which will halve the suspicion of the local nobles towards all your enthralled or broken agents and nobles."
                + "\n[Requires a noble]";
        }

        public override string getName()
        {
            return "Plead Case";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_mask;
        }
    }
}