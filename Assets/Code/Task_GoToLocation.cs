using UnityEngine;

namespace Assets.Code
{
    public class Task_GoToLocation : Task
    {
        public Location target;

        public Task_GoToLocation(Location location)
        {
            this.target = location;
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

            unit.location.map.moveTowards(unit, target);

            if (unit.location == target) { unit.task = null; return; }
        }
    }
}