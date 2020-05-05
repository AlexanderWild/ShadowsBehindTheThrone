using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class Task_SocialiseAtCourt : Task
    {
        public int dur;

        public override string getShort()
        {
            return "Socialising at court " + dur  + "/" + World.staticMap.param.unit_socialiseAtCourtTime;
        }
        public override string getLong()
        {
            return "This agent is befriending the local noble. If this action completes, the agent will gain " + World.staticMap.param.unit_socialiseAtCourtGain + " liking," +
                " to a maximum of " + World.staticMap.param.unit_socialiseAtCourtMax + ".";
        }

        public override void turnTick(Unit unit)
        {
            if (unit.person == null) { unit.task = null; return; }
            if (unit.location.settlement == null) { unit.task = null;return; }
            if (unit.location.settlement.title == null) { unit.task = null; return; }
            if (unit.location.settlement.title.heldBy == null) { unit.task = null; return; }
            
            dur += 1;
            if (dur >= unit.location.map.param.unit_investigateTime)
            {
                RelObj rel = unit.location.settlement.title.heldBy.getRelation(unit.person);
                double delta = Math.Min(World.staticMap.param.unit_socialiseAtCourtMax - rel.getLiking(), World.staticMap.param.unit_socialiseAtCourtGain);
                if (delta < 0) { delta = 0; }
                rel.addLiking(delta, "Socialised at court", unit.location.map.turn);
                if (unit.isEnthralled())
                {
                    unit.location.map.world.prefabStore.popImgMsg(unit.getName() + " finishes socialising at court. " + unit.location.settlement.title.heldBy.getFullName() + "'s liking "
                        + "for them increases by " + ((int)delta) + " and is now " + ((int)rel.getLiking()),unit.location.map.world.wordStore.lookup("ABILITY_UNIT_SOCIALISE_AT_COURT"));
                }
                unit.task = null;
            }
        }
    }
}