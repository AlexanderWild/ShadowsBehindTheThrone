using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public abstract class VoteIssue
    {
        public Person proposer;
        public Society society;
        public List<VoteOption> options = new List<VoteOption>();

        public VoteIssue(Society society,Person proposer)
        {
            this.society = society;
            this.proposer = proposer;
        }

        public abstract double computeUtility(Person p,VoteOption option, List<ReasonMsg> msgs);
        public abstract bool stillValid(Map map);

        public virtual void implement(VoteOption option)
        {
            bool positive = true;
            int priority = (society.hasEnthralled()) ? 1 : 3;
            string msg = society.getName() + ": chose " + option.info(this) + " for " + this.ToString();

            World.staticMap.addMessage(msg, priority, positive);
        }

        public void changeLikingForVotes(VoteOption option)
        {
            //Everyone affected/concerned about the vote now changes their opinion of all the voters for the winning option
            //depending on how much they care and how much they were affected
            foreach (Person p in society.people)
            {
                double utility = computeUtility(p, option, new List<ReasonMsg>());

                double deltaRel = utility;
                if (deltaRel > 0)
                {
                    deltaRel *= p.map.param.society_votingRelChangePerUtilityPositive;
                }
                else
                {
                    deltaRel *= p.map.param.society_votingRelChangePerUtilityNegative;
                }
                if (deltaRel > p.map.param.person_maxLikingGainFromVoteAccordance)
                {
                    deltaRel = p.map.param.person_maxLikingGainFromVoteAccordance;
                }
                if (deltaRel < p.map.param.person_maxLikingLossFromVoteDiscord)
                {
                    deltaRel = p.map.param.person_maxLikingLossFromVoteDiscord;
                }
                foreach (Person voter in option.votesFor)
                {
                    p.getRelation(voter).addLiking(deltaRel,"Vote on issue " + this.ToString(),society.map.turn);
                }
            }
        }

        public virtual string getLargeDesc()
        {
            return "Voting on issue: " + this.ToString();
        }

        public virtual string getIndexInfo(int index)
        {
            return "Index: " + index;
        }
    }
}
