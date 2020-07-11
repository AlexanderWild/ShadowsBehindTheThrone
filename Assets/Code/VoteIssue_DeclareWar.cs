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

            double ourStrength = society.currentMilitary + (society.maxMilitary*0.5);
            double theirStrength = target.currentMilitary + (target.maxMilitary*0.5);
            if (ourStrength + theirStrength == 0) { ourStrength += 0.001; }
            double relativeStrength = (ourStrength - theirStrength) / (ourStrength + theirStrength + 1);
            double localU = 0;

            double relMilU = society.map.param.utility_militaryTargetRelStrengthOffensive * relativeStrength;

            bool shouldExpand = true;
            int nDukes = 0;
            foreach (Title t in society.titles)
            {
                if (t is Title_ProvinceRuler) { nDukes += 1; }
            }
            if (nDukes >= society.map.param.society_maxDukes) { shouldExpand = false; }

            bool hasOurTerritory = false;
            foreach (Location loc in target.lastTurnLocs)
            {
                foreach (Location l2 in society.lastTurnLocs)
                {
                    if (loc.province == l2.province)
                    {
                        hasOurTerritory = true;
                    }
                }
            }

            //Peace
            if (option.index == 0)
            {
                if (ourStrength < 1)
                {
                    localU = 1000;
                    msgs.Add(new ReasonMsg("We are too weak", localU));
                    u += localU;
                    return u;
                }

                if (ourStrength*2 < theirStrength)
                {
                    localU = 1000;
                    msgs.Add(new ReasonMsg("We stand no chance", localU));
                    u += localU;
                    return u;
                }

                if (relMilU < 0)
                {
                    msgs.Add(new ReasonMsg("Relative strength of current militaries", -relMilU));
                    u += relMilU;
                }

                bool tinyComplete = target.lastTurnLocs.Count <= 3 && hasOurTerritory;//You can take over tiny nations to complete your provinces
                if (!tinyComplete && society.isDarkEmpire == false && shouldExpand == false && target is Society)
                {

                    localU = society.map.param.utility_militaryOverexpansionHalt;
                    msgs.Add(new ReasonMsg("We have as much as we can control", localU));
                    u += localU;
                }

                localU = voter.politics_militarism * -100;
                msgs.Add(new ReasonMsg("Militarism of " + voter.getFullName(), localU));
                u += localU;
            }
            else //Pro-war
            {

                //We want to expand into territory we already partially own
                bool hasMyTerritory = false;
                foreach (Location loc in target.lastTurnLocs)
                {
                    if (loc.province == voter.getLocation().province)
                    {
                        hasMyTerritory = true;
                        break;
                    }
                }
                if (hasMyTerritory)
                {
                    localU = society.map.param.utility_militaryTargetCompleteProvince;
                    msgs.Add(new ReasonMsg("Has territory from my province", localU));
                    u += localU;
                }

                //We want to own more land. Invade weak neighbours
                if (shouldExpand && target is Society && target.currentMilitary * 1.5 < this.society.currentMilitary && target.getNeighbours().Contains(this.society))
                {
                    //Society soc = (Society)target;
                    //if (this.society.getCapital() != null && soc.getCapital() != null)
                    //{
                    //    if (soc.getCapital().hex.getHabilitability() > 0.5)
                    //    {
                            localU = society.map.param.utility_militaryTargetExpansion * Math.Max(0, voter.politics_militarism);
                            msgs.Add(new ReasonMsg("Expand our holdings", localU));
                            u += localU;
                    //    }
                    //}
                }

                localU = voter.politics_militarism * 100;
                msgs.Add(new ReasonMsg("Militarism of " + voter.getFullName(), localU));
                u += localU;

                foreach (ThreatItem threat in voter.threatEvaluations)
                {
                    if (threat.group == target)
                    {
                        localU = threat.threat*society.map.param.utility_fromThreat;
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
