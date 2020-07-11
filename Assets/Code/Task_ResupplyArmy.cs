using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class Task_ResupplyArmy : Task
    {
        public int dur;

        public override string getShort()
        {
            return "Resupplying";
        }
        public override string getLong()
        {
            return "This army is recruiting and training new soldiers.";
        }

        public override void turnTick(Unit unit)
        {
            if (unit.hp == unit.maxHp) { unit.task = null; return; }
            if (unit.society != unit.location.soc) { unit.task = null; return; }
            if (unit.location.settlement == null) { unit.task = null; return; }
            unit.hp += (int)(unit.location.settlement.militaryRegenAdd);
            if (unit.hp > unit.maxHp) { unit.hp = unit.maxHp; }
        }
    }
}