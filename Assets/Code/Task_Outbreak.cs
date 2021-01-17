using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class Task_Outbreak : Task
    {
        public int dur;

        public List<string> diseasesToSpread = new List<string>();

        public override string getShort()
        {
            return "Causing Outbreak (" + dur + " turns)";
        }
        public override string getLong()
        {
            return "This agent is causing an outbreak in this location. This task ends when the turns spend exceed the location's security level.";
        }


        public override void turnTick(Unit unit)
        {
            if (unit.location.soc == null || (unit.location.soc is Society == false)) { unit.task = null;return; }
            if (unit.location.settlement == null) { unit.task = null;  return; }
            

            dur += 1;
            if (dur >= unit.location.settlement.getSecurity(new List<ReasonMsg>()))
            {
                foreach (string str in diseasesToSpread)
                {
                    Property.addProperty(unit.location.map, unit.location, str);
                }
                unit.location.map.world.prefabStore.popImgMsg(unit.getName() + " has circumvented security, and spread disease to " + unit.location.getName(),
                    unit.location.map.world.wordStore.lookup("ABILITY_RED_OUTBREAK"),5);
                unit.task = null;

                Society soc = (Society)unit.location.soc;
                soc.crisisPlague = "Plagues are spreading";
                soc.crisisPlagueLong = "Diseases are spreading throughout our lands, carried by some unknown ghastly apparition";

                Evidence e = new Evidence(unit.location.map.turn);
                e.pointsTo = unit;
                e.weight = unit.location.map.param.unit_minorEvidence;
                unit.location.evidence.Add(e);
            }
        }
    }
}