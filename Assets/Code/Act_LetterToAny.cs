using UnityEngine;
using UnityEditor;
using System;

namespace Assets.Code
{
    public class Act_LetterToAny :Action
    {
        public int turns;
        public override string getShort() { return "Warn Others " + turns + "/" + World.staticMap.param.action_letterWritingTurns; }
        public override string getLong() {
            return "This character is writing a letter which they will send to a nearby noble, to warn them and increase their awareness of the dark.";
        }
        public override void turnTick(Person person)
        {
            if (person.title_land == null) { person.action = null;return; }

            turns += 1;
            if (turns >= World.staticMap.param.action_letterWritingTurns)
            {
                int c = 0;
                Person chosenTarget = null;
                foreach (Location loc in person.getLocation().getNeighbours())
                {
                    foreach (Location l2 in loc.getNeighbours())
                    {
                        if (l2.person() != null && l2.person() != person)
                        {
                            if (l2.person().awareness < person.awareness)
                            {
                                c += 1;
                                if (Eleven.random.Next(c) == 0) { chosenTarget = l2.person(); }
                            }
                        }
                    }
                }
                if (chosenTarget != null)
                {
                    double delta = person.map.param.awareness_letterWritingAwarenessGain * chosenTarget.getAwarenessMult() * person.map.param.awareness_master_speed;
                    delta = Math.Min(delta, 1 - chosenTarget.awareness);
                    delta = Math.Min(delta, person.awareness - chosenTarget.awareness);//Can't exceed your own awareness
                    chosenTarget.awareness += delta;
                    person.map.addMessage(person.getFullName() + " writes to " + chosenTarget.getFullName() + " to warn them. " + (int)(delta * 100) + " awareness gained", MsgEvent.LEVEL_ORANGE, false);
                    
                }
                person.action = null;
            }
        }
    }
}