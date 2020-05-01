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

            //1 if we're 100% of the balance, -1 if they are

            double ourStrength = society.currentMilitary;
            double theirStrength = target.currentMilitary;
            if (ourStrength + theirStrength == 0) { ourStrength += 0.001; }
            double relativeStrength = (ourStrength - theirStrength) / (ourStrength + theirStrength + 1);
            double localU = 0;

            double relMilU = society.map.param.utility_militaryTargetRelStrengthOffensive * relativeStrength;

            //Peace
            if (option.index == 0)
            {
                if (ourStrength < 1)
                {
                    localU = -1000;
                    msgs.Add(new ReasonMsg("We are too weak", localU));
                    u += localU;
                    return u;
                }

                if (relMilU < 0)
                {
                    msgs.Add(new ReasonMsg("Relative strength of current militaries", -relMilU));
                    u += relMilU;
                }

                localU = voter.politics_militarism * -100;
                msgs.Add(new ReasonMsg("Militarism of " + voter.getFullName(), localU));
                u += localU;
            }
            else //Pro-war
            {

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
                    localU = society.map.param.utility_militaryTargetCompleteProvince;
                    msgs.Add(new ReasonMsg("Has territory from my province", localU));
                    u += localU;
                }

                //We want to own more land. Invade weak neighbours
                bool shouldExpand = true;
                int nDukes = 0;
                foreach (Title t in society.titles)
                {
                    if (t is Title_ProvinceRuler) { nDukes += 1; }
                }
                if (nDukes >= society.map.param.society_maxDukes) { shouldExpand = false; }
                if (shouldExpand && option.group is Society && option.group.currentMilitary * 1.5 < this.society.currentMilitary && option.group.getNeighbours().Contains(this.society))
                {
                    localU = society.map.param.utility_militaryTargetExpansion;
                    msgs.Add(new ReasonMsg("Expand our holdings", localU));
                    u += localU;
                }

                localU = voter.politics_militarism * 100;
                msgs.Add(new ReasonMsg("Militarism of " + voter.getFullName(), localU));
                u += localU;

                foreach (ThreatItem threat in voter.threatEvaluations)
                {
                    if (threat.group == option.group)
                    {
                        localU = threat.threat*4;
                        msgs.Add(new ReasonMsg("Perceived Threat", localU));
                        u += localU;
                        break;
                    }
                }
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
