using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class VoteIssue_DeclareWar : VoteIssue
    {
        public SocialGroup target;

        public VoteIssue_DeclareWar(Society soc,SocialGroup target,Person proposer) : base(soc,proposer)
        {
            this.target = target;
        }

        public override string ToString()
        {
            if (society.offensiveTarget.getName() != null)
            {
                return "Declare War on " + society.offensiveTarget.getName();
            }
            else
            {
                return "Declare War";
            }
        }
        public override string getLargeDesc()
        {
            string reply = "Vote to declare war on " + target.getName() +".";
            return reply;
        }

        public override string getIndexInfo(int index)
        {
            if (index == 0)
            {
                return "Maintain Peace";
            }
            else
            {
                return "Declare War";
            }
        }

        public override double computeUtility(Person voter, VoteOption option,List<ReasonMsg> msgs)
        {
            double u = option.getBaseUtility(voter);

            //double parityMult = 1;
            //if (option.index == 0) { parityMult = -1; }
            //return 100 * parityMult;

            //Option 0 is "Don't declare war"
            //Multiply values based on this, as they should be symmetric for most parts
            double parityMult = 1;
            if (option.index == 0) { parityMult = -1; }

            double ourStrength = society.currentMilitary;
            double theirStrength = target.currentMilitary;
            if (ourStrength + theirStrength == 0) { ourStrength += 0.001; }
            double localU = 0;

            if (ourStrength < 1)
            {
                localU = -1000 * parityMult;
                msgs.Add(new ReasonMsg("We are too weak", localU));
                u += localU;
                return u;
            }

            //1 if we're 100% of the balance, -1 if they are
            double relativeStrength = (ourStrength - theirStrength) / (ourStrength + theirStrength + 1);

            double relMilU = society.map.param.utility_militaryTargetRelStrengthOffensive * relativeStrength * parityMult;
            msgs.Add(new ReasonMsg("Relative strength of current militaries", relMilU));
            u += relMilU;


            //We want to expand into territory we already partially own
            bool hasOurTerritory = false;
            foreach (Location loc in target.lastTurnLocs)
            {
                if (loc.province == voter.getLocation().province)
                {
                    hasOurTerritory = true;
                    break;
                }
            }
            if (hasOurTerritory)
            {
                localU = society.map.param.utility_militaryTargetCompleteProvince * parityMult;
                msgs.Add(new ReasonMsg("Has territory from my province", localU));
                u += localU;
            }

            /*
            if (relMilU > 0 && target is Society)
            {
                //0 if stability is full, getting more negative as stability decreases
                double stabilityU = (this.society.data_societalStability-1);
                stabilityU = relMilU * stabilityU;//More negative the worse stability is, reaches the complement of relMilU when civil war is imminent
                msgs.Add(new ReasonMsg("Our societal stability concerns", stabilityU));
                u += stabilityU;
            }
            */

            localU = voter.politics_militarism * parityMult * 50;
            msgs.Add(new ReasonMsg("Militarism of " + voter.getFullName(), localU));
            u += localU;


            foreach (ThreatItem threat in voter.threatEvaluations)
            {
                if (threat.group == option.group)
                {
                    localU = threat.threat;
                    msgs.Add(new ReasonMsg("Perceived Threat", localU));
                    u += localU;
                    break;
                }
            }

            /*
            if (this.society.defensiveTarget != null && this.society.defensiveTarget != this.society.offensiveTarget)
            {

                theirStrength = society.defensiveTarget.currentMilitary;
                //0 if we're 100% of the balance, -1 if they are
                relativeStrength = (((ourStrength - theirStrength) / (ourStrength + theirStrength))-1)/2;

                localU = society.map.param.utility_militaryTargetRelStrength * relativeStrength * parityMult;
                msgs.Add(new ReasonMsg("Defensive Target's Capabilities (risk of sneak attack)", localU));
                u += localU;
            }
            */

            return u;
        }

        public override void implement(VoteOption option)
        {
            base.implement(option);
            if (option.index == 1 && society.posture == Society.militaryPosture.offensive)
            {
                society.map.declareWar(society, target);
            }
        }
        public override bool stillValid(Map map)
        {
            if (society.posture != Society.militaryPosture.offensive) { return false; }
            if (society.offensiveTarget == null) { return false; }
            if (society.offensiveTarget != target) { return false; }
            if (society.map.socialGroups.Contains(target) == false) { return false; }
            return true;
        }
    }
}
