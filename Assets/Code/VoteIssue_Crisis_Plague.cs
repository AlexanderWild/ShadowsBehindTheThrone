using UnityEngine;

using System.Collections.Generic;
using System;

namespace Assets.Code
{
    public class VoteIssue_Crisis_Plague : VoteIssue
    {
        public static int NO_RESPONSE = 0;
        public static int AGENT_TO_MEDIC = 1;
        public static int AGENT_TO_BASIC = 2;

        public string shortDescString;
        public string longDescString;

        public VoteIssue_Crisis_Plague(Society soc,Person proposer,string sDesc,string lDesc) : base(soc,proposer)
        {
            shortDescString = sDesc;
            longDescString = lDesc;
        }

        public override string ToString()
        {
            return shortDescString;
        }

        public override string getLargeDesc()
        {
            return longDescString;
        }

        public override double computeUtility(Person p, VoteOption option, List<ReasonMsg> msgs)
        {
            double u = option.getBaseUtility(p);


            if (option.index == NO_RESPONSE)
            {
                msgs.Add(new ReasonMsg("Neutral Option", 0));
            }

            if (option.index == AGENT_TO_MEDIC)
            {
                double localU = Unit_Investigator.getSwitchUtility(p,(Unit_Investigator)option.unit, Unit_Investigator.unitState.medic);
                localU *= p.map.param.utility_swapAgentRolesMult;
                msgs.Add(new ReasonMsg("Balance of agent skills vs balance of threats", localU));
                u += localU;
            }
            if (option.index == AGENT_TO_BASIC)
            {
                double localU = Unit_Investigator.getSwitchUtility(p, (Unit_Investigator)option.unit, Unit_Investigator.unitState.basic);
                localU *= p.map.param.utility_swapAgentRolesMult;
                msgs.Add(new ReasonMsg("Balance of agent skills vs balance of threats", localU));
                u += localU;
            }

            return u;
        }

        public override void implement(VoteOption option)
        {
            base.implement(option);

            society.crisisPlague = null;
            society.crisisPlagueLong = null;
            society.lastEvidenceResponse = society.map.turn;

            if (option.index == AGENT_TO_MEDIC)
            {
                Unit_Investigator inv = (Unit_Investigator)option.unit;
                inv.changeState(Unit_Investigator.unitState.medic);
                if (inv.person.isWatched())
                {
                    World.self.prefabStore.popMsgAgent(option.unit, option.unit, option.unit.getName() + " has been assigned the role of Apothecarian, by the nobles of " + option.unit.society.getName() +
                    " in response to external threats. Apothecarians excell in treating plagues, but cannot do anything else.");
                }
            }
            if (option.index == AGENT_TO_BASIC)
            {
                Unit_Investigator inv = (Unit_Investigator)option.unit;
                inv.changeState(Unit_Investigator.unitState.basic);
                if (inv.person.isWatched())
                {
                    World.self.prefabStore.popMsgAgent(option.unit, option.unit, option.unit.getName() + " has been assigned the role of Agent, by the nobles of " + option.unit.society.getName() +
                    " in response to external threats. Standard agents are general-purpose units. They can investigate your agents' evidence, and can assist in wars. They can be promoted "
                    + " to specalists by vote.");
                }
            }
        }

        public override bool stillValid(Map map)
        {
            return true;
        }
    }
}