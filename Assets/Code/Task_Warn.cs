using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class Task_ShareSuspicions : Task
    {
        public int dur;

        public override string getShort()
        {
            return "Sharing Suspicions " + dur  + "/" + World.staticMap.param.unit_shareSuspicionTime;
        }
        public override string getLong()
        {
            return "This agent is sharing their suspicions with a landed noble, raising their suspicions.";
        }

        public override void turnTick(Unit unit)
        {
            dur += 1;
            if (dur >= unit.location.map.param.unit_shareSuspicionTime)
            {
                if (unit.person != null && unit.location.person() != null && unit.location.person().state != Person.personState.broken)
                {
                    foreach (RelObj rel in unit.person.relations.Values)
                    {
                        double them = unit.location.person().getRelation(rel.them).suspicion;
                        double me = rel.suspicion;
                        double gain = (me - them) * 1;

                        RelObj toInv = unit.location.person().getRelation(unit.person);
                        double relLiking = toInv.getLiking();
                        relLiking += 50;
                        if (relLiking < 0) { relLiking = 0; }
                        relLiking /= 100;
                        if (relLiking > 1) { relLiking = 1; }//0 to 1

                        gain *= relLiking;

                        if (me > them)
                        {
                            Person themP = World.staticMap.persons[rel.them];
                            unit.location.map.addMessage(unit.getName() + " warns " + unit.location.person().getFullName() + " about " + themP.getFullName(), MsgEvent.LEVEL_ORANGE, false,unit.location.hex);
                            unit.location.person().getRelation(rel.them).suspicion += gain;
                        }
                    }
                }
                

                if (unit is Unit_Investigator && unit.location.soc != null && unit.location.soc is Society)
                {
                    Unit_Investigator inv = (Unit_Investigator)unit;
                    Society soc = (Society)inv.location.soc;
                    bool submitted = false;
                    foreach (Evidence ev in inv.evidenceCarried)
                    {
                        if (soc.evidenceSubmitted.Contains(ev) == false)
                        {
                            submitted = true;
                            soc.evidenceSubmitted.Add(ev);
                            soc.lastEvidenceSubmission = unit.location.map.turn;
                            ev.turnSubmitted = unit.location.map.turn;

                            unit.location.addAgentDreadAroundThisLocation();
                            //double deltaFear = World.staticMap.param.threat_evidencePresented;
                            //if (soc.isDarkEmpire == false)
                            //{
                            //    soc.dread_agents_evidenceFound += deltaFear;
                            //}
                        }
                    }
                    if (submitted)
                    {
                        unit.location.map.addMessage(unit.getName() + " presents evidence to " + soc.getName(), MsgEvent.LEVEL_ORANGE, false,unit.location.hex);
                    }
                }
                unit.task = null;
            }
        }
    }
}