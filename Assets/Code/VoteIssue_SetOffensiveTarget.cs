using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class VoteIssue_SetOffensiveTarget : VoteIssue
    {

        public VoteIssue_SetOffensiveTarget(Society soc,Person proposer) : base(soc,proposer)
        {
        }


        public override string ToString()
        {
            string reply = "Set Offensive Target";
            // if (society.offensiveTarget == null)
            // {
            //     reply += "(now None)";
            // }
            // else
            // {
            //     reply += "(now " + society.offensiveTarget.getName() + ")";
            // }
            return reply;
        }
        public override string getLargeDesc()
        {
            string reply = "A society sets an offensive target as a prelude to war. War may usually only be declared against the offensive target, and only when the society is in the Offensive military stance.";
            reply += "\n\nNobles will prefer to set offensive targets which they regard as high-threat but low military strength.";
            return reply;
        }

        public override double computeUtility(Person voter, VoteOption option,List<ReasonMsg> msgs)
        {
            double u = option.getBaseUtility(voter);


            double ourStrength = society.currentMilitary;
            double theirStrength = option.group.currentMilitary;
            double localU = 0;


            //1 if we're 100% of the balance, -1 if they are
            double relativeStrength = (ourStrength - theirStrength) / (ourStrength + theirStrength);

            //Don't just always attack the weakest, it causes blobbing. Check military to avoid suicide charges, but attack should be modulated by threat
            //As such, this modifier only applies when its negative (aversion to attacking stronger targets)
            localU = society.map.param.utility_militaryTargetRelStrengthOffensive*relativeStrength;
            if (localU < 0)
            {
                msgs.Add(new ReasonMsg("Relative Strength of Current Militaries", localU));
                u += localU;
            }


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


            //If we already have a military target
            if (society.offensiveTarget != null)
            {
                theirStrength = society.offensiveTarget.currentMilitary;
                localU = 0;

                //1 if we're 100% of the balance, -1 if they are
                relativeStrength = (ourStrength - theirStrength) / (ourStrength + theirStrength);
                if (relativeStrength > 0) { relativeStrength = 0; }

                //Add current target threat
                foreach (ThreatItem threat in voter.threatEvaluations)
                {
                    if (threat.group == society.offensiveTarget)
                    {
                        localU -= threat.threat;
                        break;
                    }
                }

                localU -= society.map.param.utility_militaryTargetRelStrengthOffensive * relativeStrength;
                msgs.Add(new ReasonMsg("Desirability of current target (" + society.offensiveTarget.getName() + ")", localU));
                u += localU;
            }


            return u;
        }

        public override void implement(VoteOption option)
        {
            base.implement(option);
            society.offensiveTarget = option.group;
        }
        public override bool stillValid(Map map)
        {
            return true;
        }
    }
}
