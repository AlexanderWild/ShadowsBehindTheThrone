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

            bool proposerPassed = false;
            foreach (Person p in option.votesFor)
            {
                if (option.votesFor.Contains(p))
                {
                    proposerPassed = true;
                }
            }
            if (proposerPassed)
            {
                proposer.prestige += World.staticMap.param.society_prestigeFromVotingSuccess;
            }
            else
            {
                proposer.prestige += World.staticMap.param.society_prestigeFromVotingFailure;
                if (proposer.prestige < 0) { proposer.prestige = 0; }
            }
        }

        public void changeLikingForVotes(VoteOption option,VoteIssue issue)
        {
            if (issue.options.Count < 2) { return; }//No reason to hate someone for voting for the only permitted option

            //Everyone affected/concerned about the vote now changes their opinion of all the voters for the winning option
            //depending on how much they care and how much they were affected
            foreach (Person p in society.people)
            {
                double deltaRel = getLikingDelta(p, option,issue);
                if (issue.proposer == p && (option.votesFor.Contains(p) == false))
                {
                    foreach (Person voter in option.votesFor)
                    {
                        p.getRelation(voter).addLiking(society.map.param.society_dislikeFromFailedProposal, "Did not vote on my proposed measure " + this.ToString(), 
                            society.map.turn,RelObj.STACK_NONE);
                    }
                }
                foreach (Person voter in option.votesFor)
                {
                    p.getRelation(voter).addLiking(deltaRel,"Vote on issue " + this.ToString(),society.map.turn, RelObj.STACK_REPLACE);
                }
            }
        }

        public double getLikingDelta(Person p,VoteOption option,VoteIssue issue)
        {
            if (issue.options.Count < 2) { return 0; }


            //Special case voting
            if (issue is VoteIssue_AssignTitle)
            {
                bool votedForSelf = false;
                foreach (VoteOption opt in issue.options)
                {
                    if (opt.person == p && opt.votesFor.Contains(p))
                    {
                        //I voted for myself
                        votedForSelf = true;
                    }
                }
                if (votedForSelf)
                {
                    if (option.person != p)
                    {
                        //Didn't vote for me! Loathesome!
                        return p.map.param.person_dislikeFromNotBeingVotedFor;
                    }
                }
            }

            double utility = computeUtility(p, option, new List<ReasonMsg>());
            double deltaRel = utility;
            if (deltaRel > 0)
            {
                deltaRel *= p.map.param.person_votingRelChangePerUtilityPositive;
            }
            else
            {
                deltaRel *= p.map.param.person_votingRelChangePerUtilityNegative;
            }
            if (deltaRel > p.map.param.person_maxLikingGainFromVoteAccordance)
            {
                deltaRel = p.map.param.person_maxLikingGainFromVoteAccordance;
            }
            if (deltaRel < p.map.param.person_maxLikingLossFromVoteDiscord)
            {
                deltaRel = p.map.param.person_maxLikingLossFromVoteDiscord;
            }

            if (deltaRel < 0)
            {
                //You can't hate someone for voting the same way you did, that's just nuts.
                if (option.votesFor.Contains(p))
                {
                    deltaRel = 0;
                }
            }
            return deltaRel;
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
