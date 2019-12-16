using UnityEngine;

using System;

namespace Assets.Code
{
    public class Ab_Over_CancelVote : Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            Society soc = (Society)hex.location.soc;
            soc.voteSession = null;

            map.world.prefabStore.popImgMsg(
                "You cancel the voting session of " + soc.getName() + ". This current session is cancelled, and no result will occur, nor will nobles likings for each other change."
                + " Of course the next vote proposer may still simply re-table the same motion",
                map.world.wordStore.lookup("ABILITY_CANCEL_VOTE"));
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.soc == null) { return false; }
            if (hex.location.soc is Society == false) { return false; }

            Society soc = (Society)hex.location.soc;
            if (soc.voteSession == null) { return false; }
            return true;
        }
        
        public override int getCost()
        {
            return World.staticMap.param.ability_cancelVoteCost;
        }

        public override string getDesc()
        {
            return "Cancels an ongoing issue in a society, preventing its effects from occuring (including liking changes). The same issue may, sadly, be re-proposed in the next cycle by the next vote-proposer."
                + "\n[Requires society which is voting]";
        }

        public override string getName()
        {
            return "Cancel Vote";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_convert;
        }
    }
}