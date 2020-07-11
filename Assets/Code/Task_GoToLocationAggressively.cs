using UnityEngine;

namespace Assets.Code
{
    public class Task_GoToLocationAgressively : Task
    {
        public Location target;

        public Task_GoToLocationAgressively(Location loc)
        {
            this.target = loc;
        }

        public override string getShort()
        {
            return "Marching on " + target.getName();
        }
        public override string getLong()
        {
            return getShort();
        }

        public override void turnTick(Unit unit)
        {
            if (unit.location == target) { unit.task = null;return; }
            

            Location[] locations = unit.location.map.getPathTo(unit.location, target);
            if (locations == null || locations.Length < 2) { unit.task = null; return; }
            unit.location.map.adjacentMoveTo(unit, locations[1]);

            if (unit.location == target) { unit.task = null; return; }
        }
    }
}