using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class Task_InvestigateNobleBasic : Task
    {
        public int dur;

        public override string getShort()
        {
            return "Investigating Noble " + dur  + "/" + World.staticMap.param.unit_investigateNobleTime;
        }
        public override string getLong()
        {
            return "This agent is investigating this noble to determine if there exists shadow or enthrallment in this society. If this tasks complete the nobles will be aware that shadows exists, but not which noble is dark.";
        }

        public override void turnTick(Unit unit)
        {
            if (unit.location.person() == null)
            {
                unit.task = null;
                return;
            }
            
            dur += 1;
            if (dur >= unit.location.map.param.unit_investigateNobleTime)
            {
                Person person = unit.location.person();


                //person.evidence += unit.location.map.param.unit_investigateNobleEvidenceGain;
                //if (person.evidence > 1)
                //{
                //    person.evidence = 1;
                //}

                if (person.state == Person.personState.enthralled || person.state == Person.personState.broken)
                {
                    //unit.location.map.world.prefabStore.popMsg(unit.getName() + " has investigated " + person.getFullName() + ", as they suspected they were under the influence of dark powers." +
                    //    " Their investigation has caused " + person.getFullName() + " to gain " + ((int)(unit.location.map.param.unit_investigateNobleEvidenceGain * 100))
                    //    + "% evidence, and they are now at " + ((int)(person.evidence * 100)) + "%.");
                    unit.location.map.world.prefabStore.popMsgAgent(unit,unit,unit.getName() + " has investigated " + person.getFullName() + ", as they suspected they were under the influence of dark powers." +
                        " Their investigation has concluded that there exists dark influence in this society. They have warned the nobles, who may try to determine which noble is the guilty party.");

                    foreach (Person p in person.society.people)
                    {
                        p.threat_enshadowedNobles.temporaryDread += unit.location.map.param.threat_dreadFromNobleInvestigation;
                    }

                    person.society.crisisNobles = true;
                }

                unit.task = null;
            }
        }
    }
}