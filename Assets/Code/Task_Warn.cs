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
                            unit.location.map.addMessage(unit.getName() + " warns " + unit.location.person().getFullName() + " about " + themP.getFullName(), MsgEvent.LEVEL_ORANGE, false);
                            unit.location.person().getRelation(rel.them).suspicion += gain;
                        }
                    }
                }
                
                unit.task = null;
            }
        }
    }
}