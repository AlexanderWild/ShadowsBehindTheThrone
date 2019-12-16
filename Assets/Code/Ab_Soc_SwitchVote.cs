using UnityEngine;

using System;

namespace Assets.Code
{
    public class Ab_Soc_SwitchVote : Ability
    {
        public override void cast(Map map, Hex hex)
        {
            base.cast(map, hex);

            Person other = hex.location.person();
            castInner(map, other);
        }
        public override void castInner(Map map, Person other)
        {
            Society soc = map.overmind.enthralled.society;
            soc.voteSession.assignVoters();
            VoteOption enthOpt = null;
            VoteOption otherOpt = null;
            foreach (VoteOption opt in soc.voteSession.issue.options)
            {
                if (opt.votesFor.Contains(map.overmind.enthralled))
                {
                    enthOpt = opt;
                }
                if (opt.votesFor.Contains(other))
                {
                    otherOpt = opt;
                }
            }
            if (enthOpt == null) { throw new Exception("Enthralled wasn't voting for some reason"); }

            if (soc.voteSession.issue.computeUtility(other, enthOpt, new System.Collections.Generic.List<ReasonMsg>()) < -50)
            {
                map.world.prefabStore.popMsg(other.getFullName() + " refuses to change their vote, as they are too opposed to voting for " + enthOpt.info() + ".");
                return;
            }

            other.forcedVoteSession = soc.voteSession;
            other.forcedVoteOption = enthOpt;
            other.getRelation(map.overmind.enthralled).addLiking(-World.staticMap.param.ability_switchVoteLikingCost, "Asked to change vote", map.turn);


            map.world.prefabStore.popImgMsg(
                other.getFullName() + " is now voting for " + enthOpt.info(soc.voteSession.issue),
                map.world.wordStore.lookup("SOC_CHANGE_VOTE"));
        }

        public override bool castable(Map map, Person person)
        {
            if (map.overmind.enthralled == null) { return false; }
            if (person.getRelation(map.overmind.enthralled).getLiking() < World.staticMap.param.ability_switchVoteLikingCost) { return false; }

            if (map.overmind.enthralled.society == null) { throw new Exception("Enthralled didn't have a society?"); }
            Society soc = map.overmind.enthralled.society;
            if (soc.voteSession == null) { return false; }
            if (soc.voteSession.issue == null) { throw new Exception("Vote session didn't have an issue? Why?"); }

            //Check the enthralled was actually voting one way or the other
            foreach (VoteOption opt in soc.voteSession.issue.options)
            {
                if (opt.votesFor.Contains(map.overmind.enthralled))
                {
                    return true;
                }
            }
            return false;
        }
        public override bool castable(Map map, Hex hex)
        {
            if (map.overmind.enthralled == null) { return false; }
            if (hex.location == null) { return false; }
            if (hex.location.person() == null) { return false; }
            if (hex.location.person() == map.overmind.enthralled) { return false; }
            if (hex.location.soc != map.overmind.enthralled.society) { return false; }
            Person p = hex.location.person();
            return castable(map, p);
        }

        public override string specialCost()
        {
            return "Cost: -" + World.staticMap.param.ability_switchVoteLikingCost + " liking";
        }
        public override int getCost()
        {
            return 0;
        }

        public override string getDesc()
        {
            return "Requests another noble to change their vote to match that of your enthralled's current voting position."
                + "\n[Requires a noble in your enthralled's society with a positive attitude towards you enthralled (They will refuse if they are too opposed to your voting position)]";
        }

        public override string getName()
        {
            return "Change Vote";
        }

        public override Sprite getSprite(Map map)
        {
            return map.world.textureStore.icon_mask;
        }
    }
}