using UnityEngine;

using System.Collections.Generic;
using System;

namespace Assets.Code
{
    public class VoteIssue_Crisis_EvidenceDiscovered : VoteIssue
    {
        public List<Evidence> foundEvidence;

        public static int DEFEND_PROVINCE = 0;
        public static int NATIONWIDE_SECURITY = 1;
        public static int NO_RESPONSE = 2;
        public static int INVESTIGATOR_HOSTILITY = 3;
        public static int EXPELL_ALL_FOREIGN_AGENTS = 4;
        public static int LOCKDOWN_PROVINCE = 5;
        public static int AGENT_TO_INVESTIGATOR = 6;
        public static int AGENT_TO_BASIC = 7;

        public VoteIssue_Crisis_EvidenceDiscovered(Society soc,Person proposer,List<Evidence> evidence) : base(soc,proposer)
        {
            foundEvidence = evidence;
        }

        public override string ToString()
        {
            return "Crisis: Evidence of Dark Agents has been discovered!";
        }

        public override string getLargeDesc()
        {
            string reply = "Evidence that dark agents are at work has been discovered. Security can be boosted a lot in specific areas, a little nation-wide, or (if sufficiently aware and desperate) more extreme measures can be implemented.";
            return reply;
        }

        public override double computeUtility(Person p, VoteOption option, List<ReasonMsg> msgs)
        {
            double u = option.getBaseUtility(p);

            double concernLevel = p.threat_agents.threat;//curr 0 to 200
            concernLevel += (100*p.awareness) + (100*p.map.worldPanic);//0 to 400
            concernLevel /= 4;
            concernLevel = Math.Min(concernLevel,(p.map.worldPanic*100) + 20);

            concernLevel *= (1 - p.shadow);
            if (p.state == Person.personState.enthralled || p.state == Person.personState.broken) { concernLevel = 0; }

            //Define the range at which this manner of response is appropriate
            double responseLevelMin = 0;
            double responseLevelMax = 0;

            if (option.index == DEFEND_PROVINCE)
            {
                responseLevelMin = 10;
                responseLevelMax = 40;

                int evidenceFound = 0;
                foreach (Evidence ev in foundEvidence)
                {
                    if (ev.locationFound.province.index == option.province)
                    {
                        evidenceFound += 1;
                    }
                }

                double localU =  World.staticMap.param.utility_defendEvidenceProvince*evidenceFound * (1 - p.shadow);
                msgs.Add(new ReasonMsg("Amount of evidence found in " + society.map.provinces[option.province].name + " province", localU));
                u += localU;

                localU = 0;
                foreach (Person person in society.people)
                {
                    if (person.getLocation() != null && person.getLocation().province.index == option.province)
                    {
                        localU += p.getRelation(person.index).getLiking() * World.staticMap.param.utility_agentDefendProvinceLikingMult;
                    }
                }
                string add = "";
                msgs.Add(new ReasonMsg("Liking for nobles in province" + add, localU));
                u += localU;

                if (p.getLocation().province.index != option.province)
                {
                    localU = p.getSelfInterest() * p.threat_agents.threat * World.staticMap.param.utility_selfInterestFromThreat;
                    if (localU != 0)
                    {
                        msgs.Add(new ReasonMsg("Does not help me personally", localU));
                        u += localU;
                    }
                }
                else
                {
                    localU = -1*p.getSelfInterest() * p.threat_agents.threat * World.staticMap.param.utility_selfInterestFromThreat;
                    if (localU != 0)
                    {
                        msgs.Add(new ReasonMsg("Helps me personally", localU));
                        u += localU;
                    }
                }
            }
            if (option.index == NATIONWIDE_SECURITY)
            {
                responseLevelMin = 10;
                responseLevelMax = 40;

                double localU = World.staticMap.param.utility_evidenceResonseBaseline * (concernLevel / 100);
                msgs.Add(new ReasonMsg("Base Desirability", localU));
                u += localU;

                localU = p.getSelfInterest() * p.threat_agents.threat * World.staticMap.param.utility_selfInterestFromThreat/2;
                if (localU != 0)
                {
                    msgs.Add(new ReasonMsg("Doesn't maximise my provinces' defences", localU));
                    u += localU;
                }
            }
            if (option.index == NO_RESPONSE)
            {
                responseLevelMin = 0;
                responseLevelMax = 10;

                if (p.getGreatestThreat() != null && p.getGreatestThreat() != p.threat_agents)
                {
                    double localU = World.staticMap.param.utility_greatestThreatDelta*0.5;
                    msgs.Add(new ReasonMsg("Not our greatest threat", localU));
                    u += localU;
                }
                else
                {
                    double localU = -World.staticMap.param.utility_greatestThreatDelta;
                    msgs.Add(new ReasonMsg("Dark Agents are our greatest threat", localU));
                    u += localU;
                }
            }

            if (option.index == EXPELL_ALL_FOREIGN_AGENTS)
            {
                responseLevelMin = 75;
                responseLevelMax = 100;

                double n = 0;
                foreach (Unit unit in society.map.units)
                {
                    if (unit.society == society) { continue; }
                    if (society.enemies.Contains(unit) == false)
                    {
                        n += 1;
                    }
                }
                if (n > 0)
                {
                    msgs.Add(new ReasonMsg("So many potential enemies!", concernLevel));
                    u += concernLevel;
                }
            }
            if (option.index == LOCKDOWN_PROVINCE)
            {
                responseLevelMin = 40;
                responseLevelMax = 100;

                int evidenceFound = 0;
                foreach (Evidence ev in foundEvidence)
                {
                    if (ev.locationFound.province.index == option.province)
                    {
                        evidenceFound += 1;
                    }
                }

                double localU = World.staticMap.param.utility_defendEvidenceProvince * evidenceFound * (1 - p.shadow);
                msgs.Add(new ReasonMsg("Amount of evidence found in " + society.map.provinces[option.province].name + " province", localU));
                u += localU;

                localU = 0;
                foreach (Person person in society.people)
                {
                    if (person.getLocation() != null && person.getLocation().province.index == option.province)
                    {
                        localU += p.getRelation(person.index).getLiking() * World.staticMap.param.utility_agentDefendProvinceLikingMult;
                    }
                }
                string add = "" +(int)(localU);
                msgs.Add(new ReasonMsg("Liking for nobles in province" + add, localU));
                u += localU;

                if (p.getLocation().province.index != option.province)
                {
                    localU = p.getSelfInterest() * p.threat_agents.threat * World.staticMap.param.utility_selfInterestFromThreat;
                    if (localU != 0)
                    {
                        msgs.Add(new ReasonMsg("Does not help me personally", localU));
                        u += localU;
                    }
                }
                else
                {
                    localU = -1 * p.getSelfInterest() * p.threat_agents.threat * World.staticMap.param.utility_selfInterestFromThreat;
                    if (localU != 0)
                    {
                        msgs.Add(new ReasonMsg("Helps me personally", localU));
                        u += localU;
                    }
                }
            }
            if (option.index == AGENT_TO_INVESTIGATOR)
            {
                responseLevelMin = 0;
                responseLevelMax = 100;
                Unit_Investigator inv = (Unit_Investigator)option.unit;

                double localU = Unit_Investigator.getSwitchUtility(p,(Unit_Investigator)option.unit, Unit_Investigator.unitState.investigator);
                localU *= p.map.param.utility_swapAgentRolesMult;
                msgs.Add(new ReasonMsg("Balance of agent skills vs balance of threats", localU));
                u += localU;
                bool hasSuspicions = false;
                foreach (RelObj rel in inv.person.relations.Values)
                {
                    if (rel.suspicion > 0 && inv.location.map.persons[rel.them].unit != null && inv.location.map.units.Contains(inv.location.map.persons[rel.them].unit))
                    {
                        hasSuspicions = true;
                    }
                }
                if (hasSuspicions)
                {
                    localU = 12;
                    msgs.Add(new ReasonMsg("Has a suspect", localU));
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

            society.lastEvidenceResponse = society.map.turn;
            foreach (Evidence ev in foundEvidence)
            {
                society.handledEvidence.Add(ev);
            }

            if (option.index == DEFEND_PROVINCE)
            {
                World.log(society.getName() + " implements crisis legislation, increasing security to " + society.map.provinces[option.province].name);
                foreach (Location loc in society.map.locations)
                {
                    if (loc.province.index == option.province && loc.soc == society)
                    {
                        Property.addProperty(society.map, loc, "Major Security Boost");
                    }
                }
                society.map.addMessage(society.getName() + " raises " + society.map.provinces[option.province].name + " security level", MsgEvent.LEVEL_ORANGE, false);
            }
            if (option.index == NATIONWIDE_SECURITY)
            {
                World.log(society.getName() + " implements crisis legislation, increasing security nationwide");
                foreach (Location loc in society.map.locations)
                {
                    if (loc.province.index == option.province && loc.soc == society)
                    {
                        Property.addProperty(society.map, loc, "Minor Security Boost");
                    }
                }
                society.map.addMessage(society.getName() + " raises its security level nationwide", MsgEvent.LEVEL_ORANGE, false);
            }
            if (option.index == NO_RESPONSE)
            {
                World.log(society.getName() + " implements crisis legislation, does nothing");
            }
            if (option.index == EXPELL_ALL_FOREIGN_AGENTS)
            {
                bool agentsExpelled = false;
                foreach (Unit u in society.map.units)
                {
                    if (u.society != society && society.enemies.Contains(u) == false)
                    {
                        society.enemies.Add(u);
                        if (u.isEnthralled())
                        {
                            agentsExpelled = true;
                        }
                    }
                }
                if (agentsExpelled)
                {
                    society.map.world.prefabStore.popMsg(society.getName() + " expells all foreign agents, in response to evidence discovered." +
                        " All your existing agents will now be attacked on sight if they enter or are in its lands. New agents will still be acceptable.");
                }
                World.log(society.getName() + " implements crisis legislation, expelling all foreign agents");
                society.map.addMessage(society.getName() + " outlaws all foreign agents", MsgEvent.LEVEL_RED, false);
            }
            if (option.index == INVESTIGATOR_HOSTILITY)
            {
                foreach (Evidence ev in foundEvidence)
                {
                    if (ev.discoveredBy.society == society && ev.discoveredBy != null && ev.pointsTo != null && (ev.discoveredBy.hostileTo(ev.pointsTo) == false))
                    {
                        ev.discoveredBy.hostility.Add(ev.pointsTo);
                        society.map.addMessage(ev.discoveredBy.getName() + " permitted to attack " + ev.pointsTo.getName(), MsgEvent.LEVEL_RED, !ev.pointsTo.isEnthralled());
                        if (ev.pointsTo.isEnthralled())
                        {
                            society.map.world.prefabStore.popMsg("The nobles of " + society.getName() + " have given permission their agent, " + ev.discoveredBy.getName()
                                + " to attack " + ev.pointsTo.getName() + " on sight, if they encounter them during investigations.");
                        }
                    }
                }
            }
            if (option.index == LOCKDOWN_PROVINCE)
            {
                World.log(society.getName() + " implements crisis legislation, fully locking down " + society.map.provinces[option.province].name);
                Unit enthralledVic = null;
                foreach (Location loc in society.map.locations)
                {
                    if (loc.province.index == option.province && loc.soc == society)
                    {
                        Property.addProperty(society.map, loc, "Lockdown");
                        foreach (Unit u in loc.units)
                        {
                            if (u.isEnthralled())
                            {
                                enthralledVic = u;
                            }
                        }
                    }
                }
                society.map.addMessage(society.getName() + " locks down " + society.map.provinces[option.province].name, MsgEvent.LEVEL_ORANGE, false);
                if (enthralledVic != null)
                {
                    society.map.world.prefabStore.popMsg(society.getName() + " has imposed a complete lockdown in the province " + society.map.provinces[option.province].name +
                        " which impacts your agent " + enthralledVic.getName() + "'s ability to operate until the lockdown is over.");
                }
            }

            if (option.index == AGENT_TO_INVESTIGATOR)
            {
                Unit_Investigator inv = (Unit_Investigator)option.unit;
                inv.changeState(Unit_Investigator.unitState.investigator);
                World.self.prefabStore.popMsgAgent(option.unit, option.unit, option.unit.getName() + " has been assigned the role of Investigator, by the nobles of " + option.unit.society.getName() +
                    " in response to external threats. Investigators are experts at combatting your agents. They can recognise evidence and spot your agents if they are in the same location.");
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