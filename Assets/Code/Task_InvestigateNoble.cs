using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class Task_InvestigateNoble : Task
    {
        public int dur;

        public override string getShort()
        {
            return "Investigating Noble " + dur  + "/" + World.staticMap.param.unit_investigateNobleTime;
        }
        public override string getLong()
        {
            return "This agent is trying to bring to light evidence of the local noble's enthrallment if they complete this task, the noble will gain evidence.";
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


                person.evidence += unit.location.map.param.unit_investigateNobleEvidenceGain;
                if (person.evidence > 1)
                {
                    person.evidence = 1;
                }

                if (person.state == Person.personState.enthralled)
                {
                    unit.location.map.world.prefabStore.popMsg(unit.getName() + " has investigated " + person.getFullName() + ", as they suspected they were under the influence of dark powers." +
                        " Their investigation has caused " + person.getFullName() + " to gain " + ((int)(unit.location.map.param.unit_investigateNobleEvidenceGain * 100))
                        + "% evidence, and they are now at " + ((int)(person.evidence * 100)) + "%.");
                }

                person.investigationLastTurn = unit.location.map.turn;

                unit.task = null;
            }
        }
    }
}