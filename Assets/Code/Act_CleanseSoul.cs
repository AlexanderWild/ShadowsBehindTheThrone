using UnityEngine;

namespace Assets.Code
{
    public class Act_CleanseSoul :Action
    {
        public int turns;
        public override string getShort() { return "Cleanse Soul " + turns + "/" + World.staticMap.param.action_cleanseSoulTurns; }
        public override string getLong() {
            return "This character is cleansing their soul, removing shadow. They will remove " + ((int)(100*World.staticMap.param.action_cleanseSoulAmount)) + "% shadow if they succeed.";
        }
        public override void turnTick(Person person)
        {
            turns += 1;
            if (person.state == Person.personState.enthralled || person.state == Person.personState.broken)
            {
                person.action = null;
                return;
            }

            if (turns >= World.staticMap.param.action_cleanseSoulTurns)
            {
                person.shadow -= World.staticMap.param.action_cleanseSoulAmount;
                if (person.shadow < 0) { person.shadow = 0; }
                person.map.addMessage(person.getFullName() + " has cleansed their soul of some shadow", MsgEvent.LEVEL_YELLOW, false);
                person.action = null;
            }
        }
    }
}