using UnityEngine;

namespace Assets.Code
{
    public class Task_HuntEnthralled_PaladinState : Task
    {
        public Unit target;
        public int turnsLeft;
        public Unit_Investigator inv;

        public Task_HuntEnthralled_PaladinState(Unit_Investigator inv,Unit prey)
        {
            this.target = prey;
            this.inv = inv;
            turnsLeft = World.staticMap.param.unit_paladin_trackDuration;
        }

        public override string getShort()
        {
            return "Hunting " + target.getName() + "\nTurns left: " + inv.paladinDuration;
        }

        public override string getLong()
        {
            return "This agent is hunting " + target.getName() + ", and will continue to do so for " + inv.paladinDuration + " turns. " +
                "They will not enter hostile territory, but otherwise will seek out their prey tirelessly, until they run out of supplies and must return home.";
        }

        public override void turnTick(Unit unit)
        {
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