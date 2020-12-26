using UnityEngine;

namespace Assets.Code
{
    public class Task_SupportMilitary : Task
    {
        public Task_SupportMilitary()
        {
        }

        public override string getShort()
        {
            return "Supporting Military Units";
        }
        public override string getLong()
        {
            return "This agent is supporting military units. Any units in the same location will deal more damage when attacking or attacked.";
        }

        public override void turnTick(Unit unit)
        {
            if (unit.society.isAtWar() == false) { unit.task = null;return; }


            foreach (Unit u2 in unit.location.units)
            {
                if (unit == u2) { continue; }
                if (u2.society == unit.society && u2.isMilitary)
                {
                    return;//No need to do anything other than idle
                }
            }

            double closest = 0;
            Unit closestAlly = null;
            foreach (Unit u2 in unit.location.map.units)
            {
                if (u2.society == unit.society && u2.isMilitary)
                {
                    double dist = unit.location.map.getDist(u2.location, unit.location);
                    dist += 0.00001 * Eleven.random.NextDouble();
                    if (dist < closest || closestAlly == null)
                    {
                        closestAlly = u2;
                        closest = dist;
                    }
                }
            }
            if (closestAlly != null)
            {
                unit.location.map.moveTowards(unit, closestAlly.location);
            }
        }
        public override bool isBusy()
        {
            return false;
        }
    }
}