using UnityEngine;

namespace Assets.Code
{
    public class Act_Disrupted :Action
    {
        public int turns;
        public override string getShort() { return "Disrupted " + turns + "/" + World.staticMap.param.ability_disruptActionDuration; }
        public override string getLong() {
            return "This character has been disrupted by your power, and will take a number of turns to recover before they can continue their efforts.";
        }
        public override void turnTick(Person person)
        {
            if (person.title_land == null) { person.action = null;return; }

            turns += 1;
            if (turns >= World.staticMap.param.ability_disruptActionDuration)
            {
                person.action = null;
            }
        }
    }
}