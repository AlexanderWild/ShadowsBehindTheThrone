using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class Task_Infiltrate_Weak : Task
    {
        public int dur;

        public override string getShort()
        {
            return "Slowly Infiltrate " + dur  + "/" + (World.staticMap.param.unit_infiltrateWeakTime);//+2 for weakened
        }
        public override string getLong()
        {
            return "This agent is infiltrating the location, increasing your infiltration value after the task completes. Without your direct support, their skills are inferior to usual.";
        }

        public static double getEffectiveness(Unit unit)
        {
            double value = World.staticMap.param.unit_infiltrateAmount / (World.staticMap.param.unit_divisorOffset + unit.location.settlement.getSecurity(new List<ReasonMsg>()));
            double liking = 0;
            if (unit.location.person() != null && unit.person != null)
            {
                liking = unit.location.person().getRelation(unit.person).getLiking();
            }
            liking /= 100;
            if (liking > 0)
            {
                liking *= 2;
                value *= (liking + 1);//Triple effect on max liking
            }
            else
            {
                if (liking < -1) { liking = -1; }
                value *= (1 + (liking * 0.9));//10% effect on min liking
            }

            value /= 2;//Weaken it

            if (value > 1) { value = 1; }
            return value;
        }

        public override void turnTick(Unit unit)
        {
            if (unit.location.soc == null || (unit.location.soc is Society == false)) { unit.task = null;return; }
            if (unit.location.settlement == null) { unit.task = null;  return; }
            

            dur += 1;
            if (dur >= unit.location.map.param.unit_infiltrateWeakTime)
            {
                double value = getEffectiveness(unit);

                unit.location.settlement.infiltration += value;
                if (unit.location.settlement.infiltration > 1) { unit.location.settlement.infiltration = 1; }

                //unit.location.map.world.prefabStore.popImgMsg(unit.getName() + " finishes increasing your infiltration level in " + unit.location.getName() +
                //    ". Infiltration increased by " + (int)(100*value) + "%, and is now at " + (int)(100*unit.location.settlement.infiltration),
                //    unit.location.map.world.wordStore.lookup("ABILITY_UNIT_INFILTRATE"));
                unit.task = null;

                Evidence e = new Evidence(unit.location.map.turn);
                e.pointsTo = unit;
                e.weight = unit.location.map.param.unit_majorEvidence;
                unit.location.evidence.Add(e);
            }
        }
    }
}