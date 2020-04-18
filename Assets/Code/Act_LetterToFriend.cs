using UnityEngine;
using System;

namespace Assets.Code
{
    public class Act_LetterToFriend :Action
    {
        public int turns;
        public override string getShort() { return "Warn Friend " + turns + "/" + World.staticMap.param.action_letterWritingTurns; }
        public override string getLong() {
            return "This character is writing a letter which they will send to a neighbouring friend (positive liking), to warn them and increase their awareness of the dark.";
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
                    if (loc.person() != null)
                    {
                        RelObj rel = person.getRelation(loc.person());
                        if (rel.getLiking() < 10 && loc.person().awareness < person.awareness)
                        {
                            c += 1;
                            if (Eleven.random.Next(c) == 0) { chosenTarget = loc.person(); }
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