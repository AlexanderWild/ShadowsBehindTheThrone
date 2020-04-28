using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class Task_Investigate : Task
    {
        public int dur;

        public override string getShort()
        {
            return "Investigating Evidence " + dur  + "/" + World.staticMap.param.unit_investigateTime;
        }
        public override string getLong()
        {
            return getShort();
        }

        public override void turnTick(Unit unit)
        {
            if (unit.location.evidence.Count > 0)
            {
                dur += 1;
                if (dur >= unit.location.map.param.unit_investigateTime)
                {
                    unit.location.evidence.RemoveAt(0);
                    unit.task = null;
                }
            }
            else
            {
                //Evidence gone. Probably eaten by someone else. Return to idle to retask next turn
                unit.task = null;
            }
        }
    }
}