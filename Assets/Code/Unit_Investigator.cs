using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Unit_Investigator : Unit
    {
        LinkedList<Location> visitBuffer = new LinkedList<Location>();
        int bufferSize = 16;
        int wanderDur = 8;
        int sinceHome = 0;

        public Unit_Investigator(Location loc,Society soc) : base(loc,soc)
        {
        }

        public override void turnTick(Map map)
        {
            checkForDisband(map);


            /*
            int distHome = map.getStepDist(location, society);
            if (distHome > maxWanderRange)
            {
                map.instaMoveTo(this,map.getPathTo(location, society.getCapital())[0]);
                return;
            }
            /*
            if (this.location == society.getCapital())
            {
                sinceHome = 0;
            }
            else
            {
                sinceHome += 1;
            }
            if (sinceHome > wanderDur)
            {
                map.instaMoveTo(this, map.getPathTo(location, society.getCapital())[0]);
                return;
            }
            */

            World.log("Turn taking " + this.getName());
            List<Location> neighbours = location.getNeighbours();

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
            map.instaMoveTo(this, chosen);
            if (visitBuffer.Count > bufferSize)
            {
                visitBuffer.RemoveFirst();
            }
        }

        public override Sprite getSprite(World world)
        {
            return world.textureStore.unit_lookingGlass;
        }

        public override string getTitleM()
        {
            return "Investigator";
        }

        public override string getTitleF()
        {
            return "Investigator";
        }

        public override string getDesc()
        {
            return "LORUM IPSUM";
        }
    }
}
