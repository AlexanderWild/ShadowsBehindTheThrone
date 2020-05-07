using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class Task_SpreadShadow : Task
    {
        public int dur;

        public override string getShort()
        {
            return "Spreading Shadow " + dur  + "/" + World.staticMap.param.unit_spreadShadowTime;
        }
        public override string getLong()
        {
            return "This agent is spreading your shadow over the local noble. When it completes their enshadowment percentage will increase.";
        }

        public override void turnTick(Unit unit)
        {
            if (unit.person == null) { unit.task = null;return; }
            if (unit.location.person() == null) { unit.task = null;  return; }
            if (unit.location.person().getRelation(unit.person).getLiking() < unit.location.map.param.unit_spreadShadowMinLiking/2)
            {

                unit.location.map.world.prefabStore.popMsg(unit.getName() + " can no longer spread shadow over " + unit.location.person().getFullName() + " as their liking is too low.");
                unit.task = null;
                return;
            }


            dur += 1;
            if (dur >= unit.location.map.param.unit_spreadShadowTime)
            {

                Evidence e = new Evidence(unit.location.map.turn);
                e.pointsTo = unit;
                e.weight = unit.location.map.param.unit_spreadShadowEvidence;
                unit.location.evidence.Add(e);

                unit.location.person().shadow = Math.Min(1, unit.location.person().shadow + unit.location.map.param.unit_spreadShadowAmount);

                unit.location.map.world.prefabStore.popImgMsg(unit.getName() + " enshadowing " + unit.location.person().getFullName() +
                    ", they now are at " + ((int)(100*unit.location.person().shadow)) + "% shadow", unit.location.map.world.wordStore.lookup("ABILITY_UNIT_SPREAD_SHADOW"));
                unit.task = null;
            }
        }
    }
}