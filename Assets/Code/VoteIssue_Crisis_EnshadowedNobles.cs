using UnityEngine;

using System.Collections.Generic;
using System;

namespace Assets.Code
{
    public class VoteIssue_Crisis_EnshadowedNobles : VoteIssue
    {
        public List<Evidence> foundEvidence;

        public static int NO_RESPONSE = 0;

        public static int WITCH_HUNT = 5;
        public static int AGENT_TO_INQUISITOR = 6;
        public static int AGENT_TO_BASIC = 7;

        public VoteIssue_Crisis_EnshadowedNobles(Society soc,Person proposer,List<Evidence> evidence) : base(soc,proposer)
        {
            foundEvidence = evidence;
        }

        public override string ToString()
        {
            return "Crisis: Evidence of Dark Nobles has been discovered!";
        }

        public override string getLargeDesc()
        {
            string reply = "Evidence that some nobles within this society may be under the influence of dark powers has been detected. The society can now vote on responses to this issue.";
            return reply;
        }

        public override double computeUtility(Person p, VoteOption option, List<ReasonMsg> msgs)
        {
            double u = option.getBaseUtility(p);

            double concernLevel = p.threat_enshadowedNobles.threat;//curr 0 to 200
            concernLevel += (100*p.awareness) + (100*p.map.worldPanic);//0 to 400
            concernLevel /= 4;
            concernLevel = Math.Min(concernLevel,(p.map.worldPanic*100) + 20);

            concernLevel *= (1 - p.shadow);
            if (p.state == Person.personState.enthralled || p.state == Person.personState.broken) { concernLevel = 0; }

            //Define the range at which this manner of response is appropriate
            double responseLevelMin = 0;
            double responseLevelMax = 0;

            if (option.index == NO_RESPONSE)
            {
                responseLevelMin = 0;
                responseLevelMax = 10;

                if (p.getGreatestThreat() != null && p.getGreatestThreat() != p.threat_enshadowedNobles)
                {
                    double localU = World.staticMap.param.utility_greatestThreatDelta * 0.5;
                    msgs.Add(new ReasonMsg("Not our greatest threat", localU));
                    u += localU;
                }
                else
                {
                    double localU = -World.staticMap.param.utility_greatestThreatDelta;
                    msgs.Add(new ReasonMsg("Enshadowed nobles are our greatest threat", localU));
                    u += localU;
                }
            }

            if (option.index == WITCH_HUNT)
            {
                responseLevelMin = 80;
                responseLevelMax = 100;

                double localU = -World.staticMap.param.utility_killSuspectRelucatance;
                msgs.Add(new ReasonMsg("Base Reluctance to Execute Noble", localU));
                u += localU;


                bool isWarlike = false;
                bool isHonorable = false;
                bool isPacifist = false;
                foreach (Trait t in p.traits)
                {
                    if (t is Trait_Political_Warlike) { isWarlike = true; break; }
                    if (t is Trait_Political_Honorable) { isHonorable = true; break; }
                    if (t is Trait_Political_Pacifist) { isPacifist = true; break; }
                }
                if (isHonorable)
                {
                    localU = p.map.param.utility_honorableHatesWitchHunt;
                    msgs.Add(new ReasonMsg("Honorable Politician opposes executions without trials", localU));
                    u += localU;
                }
                if (isWarlike)
                {
                    localU = p.map.param.utility_warlikeLikesWitchHunt;
                    msgs.Add(new ReasonMsg("Warlike Politician supports violence", localU));
                    u += localU;
                }
                if (isPacifist)
                {
                    localU = -p.map.param.utility_warlikeLikesWitchHunt;
                    msgs.Add(new ReasonMsg("Pacifist Politician supports violence", localU));
                    u += localU;
                }

                double maxSuspicion = 0;
                foreach (Person p2 in p.society.people)
                {
                    maxSuspicion = Math.Max(maxSuspicion,p.getRelation(p2.index).suspicion);
                }
                localU = maxSuspicion;
                if (localU != 0)
                {
                    msgs.Add(new ReasonMsg("Suspicion towards other nobles", localU));
                    u += localU;
                }
            }

            if (option.index == AGENT_TO_INQUISITOR)
            {
                responseLevelMin = 0;
                responseLevelMax = 100;
                Unit_Investigator inv = (Unit_Investigator)option.unit;

                double localU = Unit_Investigator.getSwitchUtility(p,(Unit_Investigator)option.unit, Unit_Investigator.unitState.inquisitor);
                localU *= p.map.param.utility_swapAgentRolesMult;
                msgs.Add(new ReasonMsg("Balance of agent skills vs balance of threats", localU));
                u += localU;

                bool isCorrupt = false;
                foreach (Trait t in p.traits)
                {
                    if (t is Trait_Political_Corrupt) { isCorrupt = true;break; }
                }
                if (isCorrupt)
                {
                    localU = p.map.param.utility_corruptHatesInquisitor;
                    msgs.Add(new ReasonMsg("Inquisitor may expose my political corruption", localU));
                    u += localU;
                }
            }
            if (option.index == AGENT_TO_BASIC)
            {
                responseLevelMin = 0;
                responseLevelMax = 100;

                double localU = Unit_Investigator.getSwitchUtility(p, (Unit_Investigator)option.unit, Unit_Investigator.unitState.basic);
                localU *= p.map.param.utility_swapAgentRolesMult;
                msgs.Add(new ReasonMsg("Balance of agent skills vs balance of threats", localU));
                u += localU;
            }

            if (concernLevel > responseLevelMax)
            {
                double delta = responseLevelMax - concernLevel;
                delta *= society.map.param.utility_extremeismScaling;
                double localU = delta;
                msgs.Add(new ReasonMsg("Too weak response for current concern level (" + (int)(concernLevel) + "%)", localU));
                u += localU;
            }
            if (concernLevel < responseLevelMin)
            {
                double delta = concernLevel - responseLevelMin;
                delta *= society.map.param.utility_extremeismScaling;
                double localU = delta;
                msgs.Add(new ReasonMsg("Too extreme for current concern level (" + (int)(concernLevel) +"%)", localU));
                u += localU;
            }
            return u;
        }

        public override void implement(VoteOption option)
        {
            base.implement(option);

            society.lastNobleCrisis = society.map.turn;
            society.crisisNobles = false;
            foreach (Evidence ev in foundEvidence)
            {
                society.handledEvidence.Add(ev);
            }

            if (option.index == WITCH_HUNT)
            {
                society.crisisWitchHunt = true;
            }
            if (option.index == AGENT_TO_INQUISITOR)
            {
                Unit_Investigator inv = (Unit_Investigator)option.unit;
                inv.changeState(Unit_Investigator.unitState.inquisitor);
                World.self.prefabStore.popMsgAgent(option.unit, option.unit, option.unit.getName() + " has been assigned the role of Inquisitor, by the nobles of " + option.unit.society.getName() +
                    " in response to external threats. Inquisitors are experts at combatting your finding enthralled, enshadowed, broken or corrupt nobles.");
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