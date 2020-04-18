using UnityEngine;

namespace Assets.Code
{
    public class Act_Investigate :Action
    {
        public int turns;
        public override string getShort() { return "Investigate " + turns + "/" + World.staticMap.param.action_investigateTurns; }
        public override string getLong() {
            return "This character is trying to discover hidden enthralled in their neighbouring locations. When this action completes, any enthralled noble neighbouring this location" +
            " will gain evidence.";
        }
        public override void turnTick(Person person)
        {
            if (person.title_land == null) { person.action = null;return; }

            turns += 1;
            if (turns >= World.staticMap.param.action_investigateTurns)
            {
                foreach (Location loc in person.title_land.settlement.location.getNeighbours())
                {
                    if (loc.person() != null)
                    {
                        if (loc.person().state == Person.personState.enthralled && loc.person().evidence != 1)
                        {
                            loc.person().evidence += person.map.param.action_investigateEvidence;
                            if (loc.person().evidence > 1) { loc.person().evidence = 1; }
                            person.map.world.prefabStore.popMsg(person.getFullName() + ", who has an awareness of the dark, has completed an investigation, which revealed your enthralled, adding evidence to them.");
                        }
                    }
                }
                person.action = null;
            }
        }
    }
}