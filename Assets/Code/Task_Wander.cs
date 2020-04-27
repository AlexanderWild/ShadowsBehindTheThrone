using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class Task_Wander : Task
    {
        LinkedList<Location> visitBuffer = new LinkedList<Location>();
        int bufferSize = 16;
        int wanderDur = 8;
        int wanderTimer = 0;

        public override string getShort()
        {
            return "Wandering";
        }
        public override string getLong()
        {
            return getShort();
        }

        public override void turnTick(Unit unit)
        {
            List<Location> neighbours = unit.location.getNeighbours();

            int c = 0;
            Location chosen = null;
            foreach (Location loc in neighbours)
            {
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
            unit.location.map.instaMoveTo(unit, chosen);
            if (visitBuffer.Count > bufferSize)
            {
                visitBuffer.RemoveFirst();
            }

            wanderTimer += 1;
            if (wanderTimer > wanderDur)
            {
                //Done wandering, go home to ensure you don't wander too far for too long
                unit.task = new Task_GoToLocation(unit.society.getCapital());
            }
        }
    }
}