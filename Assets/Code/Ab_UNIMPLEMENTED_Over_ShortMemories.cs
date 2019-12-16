using UnityEngine;

namespace Assets.Code
{
    public class Ab_UNIMPLEMENTED_Over_ShortMemories : Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            Society soc = (Society)hex.location.soc;
            soc.temporaryThreat += soc.permanentThreat;
            soc.permanentThreat = 0;

            map.world.prefabStore.popImgMsg(
                "You change " + soc.getName() + "'s permanent threat into temporary. Temporary threat will decay over time, but a portion will be converted to permanent threat.",
                map.world.wordStore.lookup("ABILITY_SHORT_MEMORIES"));
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.soc == null) { return false; }
            if (hex.location.soc.permanentThreat <= 0) { return false; }
            return true;
        }
        
        public override int getCost()
        {
            return World.staticMap.param.ability_cancelVoteCost;
        }

        public override string getDesc()
        {
            return "Permanent threat is caused by having temporary threat over a period of time. This ability converts all permanent threat back into temporary, so it can decay."
                + "\n[Requires society with permanent threat]";
        }

        public override string getName()
        {
            return "Short Memories";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_convert;
        }
    }
}