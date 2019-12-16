using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class VoteIssue_Vassalise : VoteIssue
    {
        public SocialGroup target;

        public VoteIssue_Vassalise(Society soc, Society target,Person proposer) : base(soc,proposer)
        {
            this.target = target;
        }

        public override string ToString()
        {
            return "Vassalise under " + target.getName();
        }


        public override string getLargeDesc()
        {
            string reply = "Vote to vassalise under another society for protection.";
            reply += "\nIf passed, this motion will cause this society to be absorbed by another which shares a common defensive target.";
            reply += "\nA last resort measure against a feared threat.";
            return reply;
        }

        public override double computeUtility(Person voter, VoteOption option,List<ReasonMsg> msgs)
        {
            double u = option.getBaseUtility(voter);

            //Option 0 is "Don't vassalise"
            //Multiply values based on this, as they should be symmetric for most parts
            double parityMult = 1;
            if (option.index == 0) { parityMult = -1; }


            double localU = 0;
            localU = voter.map.param.utility_vassaliseReluctance*parityMult;
            msgs.Add(new ReasonMsg("Inherent reluctance", localU));
            u += localU;

            Society socTarget = (Society)target;
            foreach (KillOrder order in socTarget.killOrders)
            {
                if (order.person == voter)
                {
                    localU = -1000 * parityMult;
                    msgs.Add(new ReasonMsg("Kill order against " + voter.getFullName(), localU));
                    u += localU;
                }
            }

            if (voter.society.defensiveTarget == null) { return u; }

            double threatV = 0;
            foreach (ThreatItem threat in voter.threatEvaluations)
            {
                if (threat.group == voter.society.defensiveTarget)
                {
                    threatV = threat.threat;
                    break;
                }
            }

            /*
            double ourStrength = society.currentMilitary;
            double theirStrength = society.defensiveTarget.currentMilitary;
            if (ourStrength + theirStrength == 0) { ourStrength += 0.001; }
            //-1 if we're 100% of the balance, +1 if they are
            double relativeStrength = (theirStrength - ourStrength) / (ourStrength + theirStrength);

            double relMilU = society.map.param.utility_vassaliseMilMult*relativeStrength * parityMult;
            msgs.Add(new ReasonMsg("Military strength vs defensive target", relMilU));
            u += relMilU;
            */

            double relThreatU = society.map.param.utility_vassaliseThreatMult * threatV * parityMult;
            msgs.Add(new ReasonMsg("Threat from defensive target", relThreatU));
            u += relThreatU;


            return u;
        }

        public override void implement(VoteOption option)
        {
            base.implement(option);
            if (option.index == 1)
            {
                bool canVassalise = false;
                Society receiever = (Society)target;
                List<Location> trans = new List<Location>();
                foreach (Location loc in society.map.locations)
                {
                    if (loc.soc == society)
                    {
                        trans.Add(loc);
                    }
                    if (loc.soc == target && loc.settlement != null)
                    {
                        canVassalise = true;
                    }
                }
                if (canVassalise)
                {
                    World.log(society.getName() + " VASSALISES UNDER " + target.getName());
                    society.map.turnMessages.Add(new MsgEvent(society.getName() + " vassalises under " + target.getName() + ", transferring all lands and nobles.",MsgEvent.LEVEL_RED,true));
                    foreach (Location loc in trans)
                    {
                        receiever.map.takeLocationFromOther(receiever, society, loc);
                    }
                }
            }
        }

        public override bool stillValid(Map map)
        {
            return true;
        }
    }
}
