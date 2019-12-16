using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Code
{
    public class VoteIssue_MilitaryStance: VoteIssue
    {

        public VoteIssue_MilitaryStance(Society soc,Person proposer) : base(soc,proposer)
        {
        }

        public override string ToString()
        {
            return "Set Military Stance";
        }


        public override string getLargeDesc()
        {
            string reply = "Three military stances exist: Offensive, Defensive and Introverted.";
            reply += "\nOffensive Stance allows for declaration of war against the offensive target a society has set.";
            reply += "\nDefensive Stance provides additional defence against a threat, should they declare war.";
            reply += "\nIntroverted Stance allows socities to defend against internal threats. It allows execution of nobles suspected of association with dark powers, and slightly reduces the risk of civil war.";
            return reply;
        }

        public override double computeUtility(Person voter, VoteOption option,List<ReasonMsg> msgs)
        {
            double u = option.getBaseUtility(voter);


            double ourStr = 1 + (voter.society.currentMilitary + (voter.society.maxMilitary / 2));
            double offStr = 1;
            double defStr = 1;
            if (voter.society.offensiveTarget != null)
            {
                offStr = voter.society.offensiveTarget.currentMilitary + (voter.society.offensiveTarget.maxMilitary/2);
            }
            if (voter.society.defensiveTarget != null)
            {
                defStr = voter.society.defensiveTarget.currentMilitary + (voter.society.defensiveTarget.maxMilitary / 2);
            }

            double defUtility = 0;
            if (voter.society.defensiveTarget != null)
            {
                //Negative if we are stronger than the one we fear
                defUtility += (defStr - ourStr) / (ourStr + defStr)*voter.map.param.utility_militaryTargetRelStrengthDefensive;
            }
            double offUtility = 0;
            double offUtilityStr = 0;
            double offUtilityPersonality = 0;
            if (voter.society.offensiveTarget != null)
            {
                //Negative if the offensive target is stronger
                offUtilityStr += (ourStr - offStr) / (ourStr + offStr) * voter.map.param.utility_militaryTargetRelStrengthOffensive;
                offUtilityPersonality += voter.politics_militarism * voter.map.param.utility_militarism;
                offUtility += offUtilityStr;
                offUtility += offUtilityPersonality;
            }
            double introUtility = 0;
            double introUtilityStability = -(society.data_societalStability - 1);//0 if stability is 1, increasing to 1 if civil war is imminent, to 2 if every single person is a traitor
            introUtilityStability *= voter.map.param.utility_introversionFromInstability;
            double introFromInnerThreat = voter.threat_enshadowedNobles.threat*voter.map.param.utility_introversionFromSuspicion;
            introUtility += introUtilityStability;
            introUtility += introFromInnerThreat;
            introUtility += 10;

            //Option 0 is DEFENSIVE
            //Option 1 is OFFENSIVE
            //Option 2 is INTROSPECTIVE
            if (voter.society.posture == Society.militaryPosture.defensive && option.index != 0)
            {
                u -= defUtility;
                msgs.Add(new ReasonMsg("Switching away from defensive", -defUtility));
            }
            if (voter.society.posture == Society.militaryPosture.offensive && option.index != 1)
            {
                u -= offUtility;
                msgs.Add(new ReasonMsg("Switching away from offensive", -offUtility));
            }
            if (voter.society.posture == Society.militaryPosture.introverted && option.index != 2)
            {
                u -= introUtility;
                msgs.Add(new ReasonMsg("Switching away from introversion", -introUtility));
            }

            if (option.index == 0 && society.posture != Society.militaryPosture.defensive)
            {
                u += defUtility;
                msgs.Add(new ReasonMsg("Our relative strength against defensive target", defUtility));
            }
            if (option.index == 1 && society.posture != Society.militaryPosture.offensive)
            {
                u += offUtility;
                msgs.Add(new ReasonMsg("Our relative strength against offensive target", offUtilityStr));
                msgs.Add(new ReasonMsg("Militarism personality", offUtilityPersonality));
            }
            if (option.index == 2 && society.posture != Society.militaryPosture.introverted)
            {
                u += introUtility;
                msgs.Add(new ReasonMsg("Instability internally",introUtilityStability));
                msgs.Add(new ReasonMsg("Suspicion of nobles' darkness", introFromInnerThreat));
            }


            return u;
        }

        public override void implement(VoteOption option)
        {
            base.implement(option);
            if (option.index == 0)
            {
                society.posture = Society.militaryPosture.defensive;
            }
            else if (option.index == 1)
            {
                society.posture = Society.militaryPosture.offensive;
            }
            else if (option.index == 2)
            {
                society.posture = Society.militaryPosture.introverted;
            }
        }
        public override bool stillValid(Map map)
        {
            return true;
        }
    }
}
