using UnityEngine;

namespace Assets.Code
{
    public class Task_GoToSocialGroup : Task
    {
        public SocialGroup target;

        public Task_GoToSocialGroup(SocialGroup sg)
        {
            this.target = sg;
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
            if (unit.location.soc == target) { unit.task = null;return; }
            if (target.isGone()) { unit.task = null;return; }
            

            Location[] locations = unit.location.map.getPathTo(unit.location, target,unit,true);
            if (locations == null || locations.Length < 2) { unit.task = null; return; }
            unit.location.map.adjacentMoveTo(unit, locations[1]);

            if (unit.location.soc == target) { unit.task = null; return; }
        }
        public override bool isBusy()
        {
            return false;
        }
    }
}