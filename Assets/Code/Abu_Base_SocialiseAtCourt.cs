using UnityEngine;


namespace Assets.Code
{
    public class Abu_Base_SocialiseAtCourt: AbilityUnit
    {

        public override void castInner(Map map, Unit u)
        {
            u.task = new Task_SocialiseAtCourt();

            u.location.map.world.prefabStore.popImgMsg(u.getName() + " begins socialising at court. This will take " + map.param.unit_socialiseAtCourtTime + " turns, after which their relationship" +
                " with the local noble will improve by up to " + map.param.unit_socialiseAtCourtGain + ", with lower gains the higher the noble's prestige.", u.location.map.world.wordStore.lookup("ABILITY_UNIT_SOCIALISE_AT_COURT"));

        }
        public override bool castable(Map map, Unit u)
        {
            if (u.location.settlement == null) { return false; }
            if (u.location.settlement.title == null) { return false; }
            if (u.location.settlement.title.heldBy == null) { return false; }
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

        public override string specialCost()
        {
            return "No Evidence";
        }
        public override string getDesc()
        {
            return "Begins a " + World.staticMap.param.unit_socialiseAtCourtTime + " turn task which will improve noble's liking for the agent by up to " 
                + World.staticMap.param.unit_socialiseAtCourtGain + ", with lower gains the higher the noble's prestige."
                + "\n[Requires a noble]";
        }

        public override string getName()
        {
            return "Socialise at Court";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_mask;
        }
    }
}