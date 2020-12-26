using UnityEngine;

using System.Collections.Generic;
using System;

namespace Assets.Code
{
    public class VoteIssue_Crisis_WarThreatens : VoteIssue
    {
        public static int NO_RESPONSE = 0;
        public static int AGENT_TO_KNIGHT = 1;
        public static int AGENT_TO_BASIC = 2;

        public string shortDescString;
        public string longDescString;

        public VoteIssue_Crisis_WarThreatens(Society soc,Person proposer,string sDesc,string lDesc) : base(soc,proposer)
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

            if (option.index == AGENT_TO_KNIGHT)
            {
                double localU = Unit_Investigator.getSwitchUtility(p,(Unit_Investigator)option.unit, Unit_Investigator.unitState.knight);
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

            society.crisisWarLong = null;
            society.crisisWarShort = null;
            society.lastEvidenceResponse = society.map.turn;

            if (option.index == AGENT_TO_KNIGHT)
            {
                World.self.prefabStore.popMsgAgent(option.unit, option.unit, "Trying to promote to knight");
                Unit_Investigator inv = (Unit_Investigator)option.unit;
                inv.state = Unit_Investigator.unitState.knight;
            }
            if (option.index == AGENT_TO_BASIC)
            {
                World.self.prefabStore.popMsgAgent(option.unit, option.unit, "Trying to promote to basic");
                Unit_Investigator inv = (Unit_Investigator)option.unit;
                inv.state = Unit_Investigator.unitState.basic;
            }
        }

        public override bool stillValid(Map map)
        {
            return true;
        }
    }
}