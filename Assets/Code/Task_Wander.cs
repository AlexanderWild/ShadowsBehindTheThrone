using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class Task_Wander : Task
    {
        public LinkedList<Location> visitBuffer = new LinkedList<Location>();
        public int bufferSize = 16;
        public int wanderDur = 8;
        public int wanderTimer = 0;

        public override string getShort()
        {
            return "Wandering";
        }
        public override string getLong()
        {
            return "This unit is patrolling their nation and surrounding lands, looking for evidence. If they find any, they will begin to investigate it.";
        }

        public override void turnTick(Unit unit)
        {

            if (unit.location.evidence.Count > 0)
            {
                unit.task = new Task_Investigate();
                return;
            }
            List<Location> neighbours = unit.location.getNeighbours();

            int c = 0;
            Location chosen = null;
            foreach (Location loc in neighbours)
            {
                if (loc.evidence.Count > 0)
                {
                    chosen = loc;
                    break;
                }
                if (visitBuffer.Contains(loc) == false)
                {
                    c += 1;
                    if (Eleven.random.Next(c) == 0)
                    {
                        chosen = loc;
                    }
                }
            }
            if (chosen != null)
            {
                visitBuffer.AddLast(chosen);
            }
            else
            {
                int q = Eleven.random.Next(neighbours.Count);
                chosen = neighbours[q];
            }
            unit.location.map.adjacentMoveTo(unit, chosen);
            if (visitBuffer.Count > bufferSize)
            {
                visitBuffer.RemoveFirst();
            }

            //Start investigating the evidence you just moved to
            if (unit.location.evidence.Count > 0)
            {
                unit.task = new Task_Investigate();
                return;
            }

            wanderTimer += 1;
            if (wanderTimer > wanderDur)
            {
                //Done wandering, go home to ensure you don't wander too far for too long
                unit.task = new Task_GoToSocialGroup(unit.society);
            }
        }
    }
}