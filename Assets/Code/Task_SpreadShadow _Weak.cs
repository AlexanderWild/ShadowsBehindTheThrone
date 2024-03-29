﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class Task_SpreadShadow_Weak : Task
    {
        public int dur;

        public override string getShort()
        {
            return "Weakly Spreading Shadow " + dur  + "/" + World.staticMap.param.unit_spreadShadowWeakTime;
        }
        public override string getLong()
        {
            return "This agent is spreading your shadow over the local noble. When it completes their enshadowment percentage will increase. Without your direct influence, their ability is weak and slow.";
        }

        public override void turnTick(Unit unit)
        {
            if (unit.person == null) { unit.task = null;return; }
            if (unit.location.person() == null) { unit.task = null;  return; }
            if (unit.location.settlement == null) { unit.task = null;return; }
            if (unit.location.settlement.infiltration < unit.location.map.param.ability_unit_spreadShadowInfiltrationReq)
            {

                //unit.location.map.world.prefabStore.popMsg(unit.getName() + " can no longer spread shadow over " + unit.location.person().getFullName() + " as the infiltration level has dropped too low.");
                unit.task = null;
                return;
            }
            //if (unit.location.person().getRelation(unit.person).getLiking() < unit.location.map.param.ability_unit_spreadShadowMinLiking/2)
            //{

            //    unit.location.map.world.prefabStore.popMsg(unit.getName() + " can no longer spread shadow over " + unit.location.person().getFullName() + " as their liking is too low.");
            //    unit.task = null;
            //    return;
            //}


            dur += 1;
            if (dur >= unit.location.map.param.unit_spreadShadowWeakTime)
            {

                Evidence e = new Evidence(unit.location.map.turn);
                e.pointsTo = unit;
                e.weight = unit.location.map.param.unit_majorEvidence;
                unit.location.evidence.Add(e);

                unit.location.person().shadow = Math.Min(1, unit.location.person().shadow + unit.location.map.param.unit_spreadShadowWeakAmount);

                //unit.location.map.world.prefabStore.popImgMsg(unit.getName() + " finishes enshadowing " + unit.location.person().getFullName() +
                //    ", they now are at " + ((int)(100*unit.location.person().shadow)) + "% shadow", unit.location.map.world.wordStore.lookup("ABILITY_UNIT_SPREAD_SHADOW"));
                unit.task = null;
            }
        }
    }
}