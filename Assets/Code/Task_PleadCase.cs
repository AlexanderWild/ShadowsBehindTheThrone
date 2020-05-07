using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class Task_PleadCase : Task
    {
        public int dur;

        public override string getShort()
        {
            return "Pleading Case " + dur  + "/" + World.staticMap.param.unit_pleadCaseTime;
        }
        public override string getLong()
        {
            return "This agent is arguing that your enthralled agents are innocent. When this task completes, the local noble will lose half their suspicion for all broken and enthralled nobles and agents.";
        }

        public override void turnTick(Unit unit)
        {
            if (unit.location.person() == null) { unit.task = null;  return; }
            
            dur += 1;
            if (dur >= unit.location.map.param.unit_pleadCaseTime)
            {
                int n = 0;
                string str = "";
                foreach (RelObj rel in unit.location.person().relations.Values)
                {
                    if (rel.suspicion > 0 && (rel.them.state == Person.personState.enthralled || rel.them.state == Person.personState.enthralledAgent || rel.them.state == Person.personState.broken))
                    {
                        rel.suspicion /= 2;
                        n += 1;
                        str += rel.them.getFullName() + "; ";
                    }
                }
                string add = "";
                if (n == 0)
                {
                    add = "No suspicion existed to remove.";
                }
                else
                {
                    add = n + " suspicions reduced: " + str.Substring(0, str.Length - 2);
                }
                unit.location.map.world.prefabStore.popImgMsg(unit.getName() + " finishes pleading the case of those under your shadow to " + unit.location.person().getFullName() +
                    ". " + add, unit.location.map.world.wordStore.lookup("ABILITY_UNIT_PLEAD_CASE"));
                unit.task = null;
            }
        }
    }
}