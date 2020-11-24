using UnityEngine;

namespace Assets.Code
{
    public class Task_GoToClue : Task
    {
        public Location target;

        public Task_GoToClue(Location loc)
        {
            this.target = loc;
        }

        public override string getShort()
        {
            return "Investigating Evidence in " + target.getName();
        }
        public override string getLong()
        {
            return "This agent has been told of the existence of evidence within a society's borders, and is travelling to investigate it.";
        }

        public override void turnTick(Unit unit)
        {
            if (unit.location == target) { unit.task = null;return; }
            

            Location[] locations = unit.location.map.getPathTo(unit.location, target,unit,true);
            if (locations == null || locations.Length < 2) { unit.task = null; return; }
            unit.location.map.adjacentMoveTo(unit, locations[1]);

            if (unit.location == target) { unit.task = null; return; }
        }
        public override bool isBusy()
        {
            return false;
        }
    }
}