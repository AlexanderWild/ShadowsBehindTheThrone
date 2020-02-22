using UnityEngine;

using System;

namespace Assets.Code
{
    public class Ab_Soc_BoycottVote: Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            Society soc = (Society)hex.location.soc;
            foreach (Person p in soc.people){
                p.getRelation(map.overmind.enthralled).addLiking(-map.param.ability_boycottVoteCost,"Boycotted vote",map.turn);
            }
            soc.voteSession = null;
            
            map.world.prefabStore.popImgMsg(
                "Your enthralled noble uses political motions to cancel the ongoing vote session, angering the nobles somewhat, but temporarily (or perhaps permanently"
                + ") stopping the voting issue.",
                map.world.wordStore.lookup("SOC_BOYCOTT_VOTE"));
        }

        public override bool castable(Map map, Hex hex)
        {
            if (hex.location == null) { return false; }
            if (hex.location.soc == null) { return false; }
            if (map.overmind.enthralled == null) { return false; }
            if (hex.location.soc != map.overmind.enthralled.society) { return false; }
            


            Society soc = (Society)hex.location.soc;
            if (soc.voteSession == null) { return false; }

            double sumLiking = 0;
            foreach (Person p in soc.people)
            {
                if (p == map.overmind.enthralled) { continue; }
                sumLiking += p.getRelation(map.overmind.enthralled).getLiking();
            }

            return sumLiking > 0;
        }

        public override string specialCost()
        {
            return "Cost: -" + World.staticMap.param.ability_boycottVoteCost + " liking (all)";
        }
        public override int getCost()
        {
            return 0;
        }

        public override string getDesc()
        {
            return "Uses the enthralled noble's social support to get a vote cancelled. The same issue may, sadly, be re-proposed in the next cycle by the next vote-proposer."
                + "\n[Requires society which is voting, and for your enthralled to have more support from nobles than dislike]";
        }

        public override string getName()
        {
            return "Boycott Vote";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_mask;
        }
    }
}