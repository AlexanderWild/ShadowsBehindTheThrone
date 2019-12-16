using UnityEngine;

using System;

namespace Assets.Code
{
    public class Ab_Soc_Vote : Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            Society soc = map.overmind.enthralled.society;
            if (soc.voteSession != null)
            {
                soc.voteSession.assignVoters();
                map.world.ui.addBlocker(map.world.prefabStore.getScrollSet(soc.voteSession, soc.voteSession.issue.options).gameObject);
                map.overmind.hasTakenAction = false;
            }
        }

        public override bool castable(Map map, Hex hex)
        {
            if (map.overmind.enthralled == null) { return false; }
            if (map.overmind.enthralled.society.voteSession == null) { return false; }

            return true;
        }

        public override string specialCost()
        {
            return "";
        }
        public override int getCost()
        {
            return 0;
        }

        public override string getDesc()
        {
            return "Vote on an issue currently in session. The winning vote is the one with the most total prestige (Adding up the prestiges of the nobles voting for that option)"
                + "\n[Requires an enthralled noble and a vote in their society]";
        }

        public override string getName()
        {
            return "Vote";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_mask;
        }
    }
}