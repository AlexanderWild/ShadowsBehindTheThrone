using UnityEngine;

namespace Assets.Code
{
    public class Act_Research :Action
    {
        public int turns;
        public override string getShort() { return "Researching"; }
        public override string getLong() {
            return "This character is learning about the nature of the darkness, via the scholarly resources available to them through the location they control." +
                " They will gain awareness until they reach 100%, but risk losing sanity every turn.";
        }
        public override void turnTick(Person person)
        {
            if (person.title_land == null) { person.action = null;return; }
            if (person.title_land.settlement is Set_University == false) { person.action = null; return; }
            if (person.awareness >= 1) { person.awareness = 1; person.action = null; }

            person.awareness += World.staticMap.param.action_research_expectedAwarenessPerTurn*Eleven.random.NextDouble() * person.map.param.awareness_master_speed;
            if (person.awareness >= 1) { person.awareness = 1;person.action = null; }
            if (person.sanity > 0 && Eleven.random.NextDouble() < World.staticMap.param.action_research_pSanityHit)
            {
                if (person.sanity > 0) { person.sanity -= 1; }
                if (person.sanity <= 0) {
                    person.sanity = 0;
                    World.staticMap.addMessage(person.getFullName() + " has gone insane from what they learnt", MsgEvent.LEVEL_DARK_GREEN, true);
                }
            }
        }
    }
}