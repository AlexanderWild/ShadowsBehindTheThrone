using UnityEngine;

namespace Assets.Code
{
    public class Task_GoToLocation : Task
    {
        public Location target;

        public Task_GoToLocation(Location loc)
        {
            this.target = loc;
        }

        public override string getShort()
        {
            return "Travelling to " + target.getName();
        }
        public override string getLong()
        {
            return getShort();
        }

        public override void turnTick(Unit unit)
        {
            if (unit.location == target) { unit.task = null;return; }
            

            Location[] locations = unit.location.map.getPathTo(unit.location, target,unit,true);
            if (locations == null || locations.Length < 2) { unit.task = null; return; }
            unit.location.map.adjacentMoveTo(unit, locations[1]);

            if (unit.location == target) { unit.task = null; return; }
        }
    }
}