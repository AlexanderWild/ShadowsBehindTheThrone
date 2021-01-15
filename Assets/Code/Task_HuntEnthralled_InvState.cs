using UnityEngine;

namespace Assets.Code
{
    public class Task_HuntEnthralled_InvState : Task
    {
        public Unit target;
        public int turnsLeft;
        public Unit_Investigator inv;

        public Task_HuntEnthralled_InvState(Unit_Investigator inv,Unit prey)
        {
            this.target = prey;
            this.inv = inv;
            turnsLeft = World.staticMap.param.unit_inv_trackDuration;
        }

        public override string getShort()
        {
            return "On the trail of " + target.getName() + "\nTurns left: " + turnsLeft;
        }

        public override string getLong()
        {
            return "This agent is on the trail of " + target.getName() + ", and will continue to chase them for " + inv.paladinDuration + " turns. " +
                "They will not enter hostile territory, but otherwise pursue their target until they are disrupted or the trail goes cold.";
        }

        public override void turnTick(Unit unit)
        {
            if (turnsLeft <= 0) { unit.task = null;return; }
            turnsLeft -= 1;

            Location[] path = unit.location.map.getPathTo(unit.location, target.location, unit, true);
            if (path == null || path.Length < 2) { return; }
            unit.location.map.adjacentMoveTo(unit, path[1]);
        }
        public override bool isBusy()
        {
            return false;
        }
    }
}