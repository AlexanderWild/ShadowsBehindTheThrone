using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class Task_Disrupted : Task
    {
        public int dur;

        public override string getShort()
        {
            return "Disrupted " + dur  + "/" + World.staticMap.param.unit_disruptDuration;
        }
        public override string getLong()
        {
            return "This agent is disrupted. They cannot take actions until they re-organise and re-orient themselves.";
        }

        public override void turnTick(Unit unit)
        {
            if (unit.location.person() == null) { unit.task = null;  return; }
            
            dur += 1;
            if (dur >= unit.location.map.param.unit_disruptDuration)
            {
                unit.task = null;
            }
        }
    }
}