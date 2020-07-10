﻿using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class Task_Investigate : Task
    {
        public int dur;

        public override string getShort()
        {
            return "Investigating Evidence " + dur  + "/" + World.staticMap.param.unit_investigateTime;
        }
        public override string getLong()
        {
            return "This agent is uncovering evidence in this location. The evidence may point towards another agent's guilt. If so, they will gain suspicion towards that unit.";
        }

        public override void turnTick(Unit unit)
        {
            //Enthralled can't just eat clues
            if (unit.isEnthralled())
            {
                unit.task = null;
                return;
            }

            if (unit.location.evidence.Count > 0)
            {
                dur += 1;
                if (dur >= unit.location.map.param.unit_investigateTime)
                {
                    Evidence ev = unit.location.evidence[0];
                    if (ev.pointsTo != null)
                    {
                        if (unit.person != null && ev.pointsTo.person != null)
                        {
                            unit.person.getRelation(ev.pointsTo.person).suspicion = System.Math.Min(1, unit.person.getRelation(ev.pointsTo.person).suspicion + ev.weight);
                        }
                        else
                        {
                            //Can't use suspicion system, go straight to murder
                            //Makes sense since they're probably non-human terrors
                            unit.hostility.Add(ev.pointsTo);
                        }

                        unit.location.map.addMessage(unit.getName() + " has found evidence from " + ev.pointsTo.getName(), MsgEvent.LEVEL_RED, false);
                    }
                    else if (ev.pointsToPerson != null)
                    {
                        if (unit.person != null && ev.pointsToPerson != null)
                        {
                            unit.person.getRelation(ev.pointsToPerson).suspicion = System.Math.Min(1, unit.person.getRelation(ev.pointsToPerson).suspicion + ev.weight);
                        }

                        unit.location.map.addMessage(unit.getName() + " has found evidence from " + ev.pointsToPerson.getFullName(), MsgEvent.LEVEL_RED, false);
                    }

                    unit.location.evidence.Remove(ev);
                    unit.task = null;

                    unit.location.map.overmind.panicFromCluesDiscovered += unit.location.map.param.panic_fromClueFound;
                    if (unit.location.map.overmind.panicFromCluesDiscovered > 1) { unit.location.map.overmind.panicFromCluesDiscovered = 1; }


                    if (unit.location.person() != null)
                    {
                        Person noble = unit.location.person();
                        foreach (RelObj rel in unit.person.relations.Values)
                        {
                            ////Goes negative if they suspect more than we do, reaches 1.0 if we suspect 1.0 and they suspect 0.0
                            // if ((rel.suspicion - noble.getRelation(rel.them).suspicion) > Eleven.random.NextDouble())
                            if ((rel.suspicion > noble.getRelation(rel.them).suspicion * 1.1))
                            {
                                unit.task = new Task_ShareSuspicions();
                                return;
                            }
                        }
                    }
                }
            }
            else
            {
                //Evidence gone. Probably eaten by someone else. Return to idle to retask next turn
                unit.task = null;
            }
        }
    }
}