using UnityEngine;

namespace Assets.Code
{
    public class Task_HuntEnthralled : Task
    {
        public Unit target;
        public int turnsLeft;

        public Task_HuntEnthralled(Unit prey)
        {
            this.target = prey;
            turnsLeft = World.staticMap.param.unit_paladin_trackDuration;
        }

        public override string getShort()
        {
            return "Hunting " + target.getName() + "\nTurns left: " + turnsLeft;
        }

        public override string getLong()
        {
            return "This agent is hunting " + target.getName() + ", and will continue to do so for " + turnsLeft + " turns. They will not enter hostile territory, but otherwise will seek out their prey tirelessly, until the trail runs cold.";
        }

        public override void turnTick(Unit unit)
        {
            turnsLeft -= 1;
            if (turnsLeft <= 0) { unit.task = null;return; }

            Location[] path = unit.location.map.getPathTo(unit.location, target.location, unit, true);
            if (path == null || path.Length < 2) { unit.task = null; return; }
            unit.location.map.adjacentMoveTo(unit, path[1]);
        }
    }
}