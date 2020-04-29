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

            unit.location.map.moveTowards(unit, target);

            if (unit.location.soc == target) { unit.task = null; return; }
        }
    }
}