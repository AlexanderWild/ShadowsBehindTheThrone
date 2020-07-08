using UnityEngine;

using System;

namespace Assets.Code
{
    public class Ab_Over_DelayVote: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            Society soc = (Society)hex.location.soc;
            soc.voteSession.timeRemaining += map.param.ability_delayVoteTurnsAdded;

            map.world.prefabStore.popImgMsg(
                "You delay the voting process of " + soc.getName() + ". The motion will go on for longer, slowing their ability to respond and giving you more time to alter the voting intentions of the nobles.",
                map.world.wordStore.lookup("ABILITY_DELAY_VOTE"));
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

        public override int getCooldown()
        {
            return World.staticMap.param.ability_delayVoteCooldown;
        }

        public override int getCost()
        {
            return World.staticMap.param.ability_delayVoteCost;
        }

        public override string getDesc()
        {
            return "Delays a voting session, adding " + World.staticMap.param.ability_delayVoteTurnsAdded + " turns until compeletion of the voting session."
                + "\n[Requires society which is voting]";
        }

        public override string getName()
        {
            return "Delay Vote";
        }
 
        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_convert;
        }
    }
}