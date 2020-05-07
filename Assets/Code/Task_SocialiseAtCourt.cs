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
            return "This agent is befriending the local noble. If this action completes, the agent will gain up to " + World.staticMap.param.unit_socialiseAtCourtGain + " liking (scaled by noble's prestige).";
        }

        public override void turnTick(Unit unit)
        {
            if (unit.person == null) { unit.task = null; return; }
            if (unit.location.settlement == null) { unit.task = null;return; }
            if (unit.location.settlement.title == null) { unit.task = null; return; }
            if (unit.location.settlement.title.heldBy == null) { unit.task = null; return; }
            if (unit.location.soc is Society == false) { unit.task = null; return; }

            dur += 1;
            if (dur >= unit.location.map.param.unit_socialiseAtCourtTime)
            {
                Society soc = (Society)unit.location.soc;
                double maxPrestige = 0;
                foreach (Person p in soc.people)
                {
                    if (p.prestige > maxPrestige) { maxPrestige = p.prestige; }
                }
                double prestige = unit.location.person().prestige;
                double scale = 0;
                if (maxPrestige > 0)
                {
                    scale = prestige / maxPrestige;
                    scale = 1 - scale;//0 to 1, 1 if prestige is 0, 0 if prestige is MAX
                    scale *= 0.75;
                    scale += 0.25;
                }

                RelObj rel = unit.location.settlement.title.heldBy.getRelation(unit.person);
                double maxGain = World.staticMap.param.unit_socialiseAtCourtGain;
                double delta = maxGain * scale;
                delta = Math.Min(delta, 100 - rel.getLiking());
                if (delta < 0) { delta = 0; }
                int iDelta = (int)(delta);

                rel.addLiking(iDelta, "Socialised at court", unit.location.map.turn);
                if (unit.isEnthralled())
                {
                    unit.location.map.world.prefabStore.popImgMsg(unit.getName() + " finishes socialising at court. " + unit.location.settlement.title.heldBy.getFullName() + "'s liking "
                        + "for them increases by " + iDelta + " and is now " + ((int)rel.getLiking()),unit.location.map.world.wordStore.lookup("ABILITY_UNIT_SOCIALISE_AT_COURT"));
                }
                unit.task = null;
            }
        }
    }
}