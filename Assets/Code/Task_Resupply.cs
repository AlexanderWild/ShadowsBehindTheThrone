﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class Task_Resupply : Task
    {
        public int dur;

        public override string getShort()
        {
            return "Resupplying";
        }
        public override string getLong()
        {
            return "This agent is healing their wounds and recruiting new followers.";
        }

        public override void turnTick(Unit unit)
        {
            if (unit.hp == unit.maxHp) { unit.task = null;return; }
            if (unit.society != unit.location.soc) { unit.task = null; return; }

            //Only heal 0.5 per turn, so attacking agents actually outpace healing
            if (unit.location.map.turn % 2 == 0)
            {
                unit.hp += 1;
            }

            if (unit.hp > unit.maxHp) { unit.hp = unit.maxHp; unit.task = null; return; }
        }
    }
}