using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class Task_Vamp_Insanity : Task
    {
        public int dur;

        public override string getShort()
        {
            return "Weaving Insanity";
        }
        public override string getLong()
        {
            return "This vampire is driving the local noble to madness.";
        }

        public override void turnTick(Unit unit)
        {
            if (unit.location.soc == null || (unit.location.soc is Society == false)) { unit.task = null;return; }
            if (unit.location.settlement == null) { unit.task = null;  return; }
            if (unit.location.person() == null) { unit.task = null; return; }

            Unit_Vampire vampire = (Unit_Vampire)unit;

            unit.location.person().sanity -= 1;

            if (unit.location.person().sanity < 1)
            {
                unit.location.person().sanity = 0;
                unit.location.map.world.prefabStore.popImgMsg(unit.getName() + " breaks the mind of " + unit.location.person().getFullName() +
         ". They are now driven to madness.",
         unit.location.map.world.wordStore.lookup("ABILITY_VAMP_INSANITY"));
                unit.task = null;
            }
        }
    }
}