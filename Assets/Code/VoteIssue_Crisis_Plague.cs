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
        public static int QUARANTINE = 3;
        public static int TREATMENT = 4;

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
            if (option.index == QUARANTINE)
            {
                //First part is your personal utility. How much am I, personally, affected?
                double localU = p.threat_plague.threat;
                bool amNeighbour = false;
                bool hasDisease = false;
                foreach (Property p2 in p.getLocation().properties)
                {
                    if (p2.proto.isDisease) { hasDisease = true;break; }
                }
                if (hasDisease)
                {
                    //This option doesn't help me. It's already here. If I'm corrupt, I'll naturally be angry at not being given help
                    localU = p.getSelfInterest() * p.threat_plague.threat * World.staticMap.param.utility_selfInterestFromThreat;
                    if (localU != 0)
                    {
                        msgs.Add(new ReasonMsg("Does not help me personally", localU));
                        u += localU;
                    }
                }
                else {
                    //I do not have the disease. Best to vote to keep it that way.
                    foreach (Location l2 in p.getLocation().getNeighbours())
                    {
                        foreach (Property p2 in l2.properties) { if (p2.proto.isDisease) { amNeighbour = true; break; } }
                    }
                    if (amNeighbour)
                    {
                        localU *= 1.5;
                    }
                    else
                    {
                        localU *= 0.66;
                    }
                    localU *= p.getSelfInterest()  * -1 * World.staticMap.param.utility_selfInterestFromThreat;//Note this param is negative as it is petulance
                    if (localU != 0)
                    {
                        msgs.Add(new ReasonMsg("Desire to keep self safe from disease", localU));
                        u += localU;
                    }
                }

                //Second part is the benefit to the nation
                int nNeighbours = 0;
                double nSocLocs = 0;
                foreach (Location l2 in p.map.locations)
                {
                    if (l2.soc == p.society) { nSocLocs += 1; }
                    else { continue; }

                    hasDisease = false;
                    foreach (Property p2 in l2.properties) { if (p2.proto.isDisease) { hasDisease = true; break; } }
                    if (hasDisease)
                    {
                        foreach (Location l3 in l2.getNeighbours())
                        {
                            if (l3.soc != p.society) { continue; }
                            bool l3HasDisease = false;
                            foreach (Property p2 in l3.properties) { if (p2.proto.isDisease) { l3HasDisease = true; break; } }
                            if (!l3HasDisease)
                            {
                                //Location 3 doesn not have the disease, but l2 does. Therefore it is a spread vector
                                //Since we're taking a risk for each link, we can double-count L3s.
                                nNeighbours += 1;
                            }
                        }
                    }
                }
                localU = nNeighbours;
                if (nSocLocs > 0) { localU /= nSocLocs; }//Normalise for society size
                localU *= p.threat_plague.threat;
                localU *= p.map.param.utility_plagueResponseMultPerRiskItem;
                localU *= Math.Max(0, Math.Min(1, 1 - p.getSelfInterest()));//Clamp that between [0,1]
                if (localU != 0)
                {
                    msgs.Add(new ReasonMsg("Benefit to nation: Number of locations at risk", localU));
                    u += localU;
                }
            }
            if (option.index == TREATMENT)
            {
                //First part is your personal utility. How much am I, personally, affected?
                double localU = p.threat_plague.threat;
                bool hasDisease = false;
                foreach (Property p2 in p.getLocation().properties)
                {
                    if (p2.proto.isDisease) { hasDisease = true; break; }
                }
                if (hasDisease)
                {
                    //I have the disease. Gotta invest in that sweet cure
                    localU *= p.getSelfInterest() * -1 * World.staticMap.param.utility_selfInterestFromThreat;//Note this param is negative as it is petulance
                    if (localU != 0)
                    {
                        msgs.Add(new ReasonMsg("Desire to save self from disease", localU));
                        u += localU;
                    }
                }
                else
                {
                    //This option doesn't help me. It's already here. If I'm corrupt, I'll naturally be angry at not being given help
                    localU = p.getSelfInterest() * p.threat_plague.threat * World.staticMap.param.utility_selfInterestFromThreat;
                    if (localU != 0)
                    {
                        msgs.Add(new ReasonMsg("Does not help me personally", localU));
                        u += localU;
                    }
                }

                //Second part is the benefit to the nation
                int nDiseased = 0;
                double nSocLocs = 0;
                foreach (Location l2 in p.map.locations)
                {
                    if (l2.soc == p.society) { nSocLocs += 1; }
                    else { continue; }

                    foreach (Property p2 in l2.properties) { if (p2.proto.isDisease) { nDiseased += 1; break; } }
                }
                localU = nDiseased;
                if (nSocLocs > 0) { localU /= nSocLocs; }//Normalise for society size
                localU *= p.threat_plague.threat;
                localU *= p.map.param.utility_plagueResponseMultPerRiskItem;
                localU *= Math.Max(0, Math.Min(1, 1 - p.getSelfInterest()));//Clamp that between [0,1]
                if (localU != 0)
                {
                    msgs.Add(new ReasonMsg("Benefit to nation: Number of locations infected", localU));
                    u += localU;
                }
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
            if (option.index == QUARANTINE)
            {
                foreach (Location loc in society.map.locations)
                {
                    if (loc.soc == society)
                    {
                        bool hasDisease = false;
                        foreach (Property pr in loc.properties)
                        {
                            if (pr.proto.isDisease) { hasDisease = true;break; }
                        }
                        if (hasDisease)
                        {
                            Property.addProperty(loc.map, loc, "Quarantine");
                        }
                    }
                }
            }
            if (option.index == TREATMENT)
            {
                foreach (Location loc in society.map.locations)
                {
                    if (loc.soc == society)
                    {
                        bool hasDisease = false;
                        foreach (Property pr in loc.properties)
                        {
                            if (pr.proto.isDisease) { hasDisease = true; break; }
                        }
                        if (hasDisease)
                        {
                            Property.addProperty(loc.map, loc, "Medical Aid");
                        }
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