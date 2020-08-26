using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Unit_Paladin : Unit
    {
        public Location home;
        public int trailTurns;

        public Unit_Paladin(Location loc,SocialGroup soc) : base(loc,soc)
        {
            maxHp = 7;
            hp = 7;
            home = loc;
            isMilitary = true;
        }

        public override void turnTickInner(Map map)
        {
        }

        public override void turnTickAI(Map map)
        {
            if (trailTurns > 0) { trailTurns -= 1; }

            if (task is Task_HuntEnthralled) {
                Task_HuntEnthralled trackingTask = (Task_HuntEnthralled)task;

                Location[] path = map.getPathTo(this.location, trackingTask.target.location,this,true);
                if (path == null)
                {
                    task = null;
                    trailTurns = 0;
                }
                else
                {

                }
            }

            if (task != null)
            {
                task.turnTick(this);
                return;
            }
        }

        public void warAI()
        {
            Society soc = (Society)this.society;
            if (soc.posture == Society.militaryPosture.offensive)
            {
                HashSet<Location> closed = new HashSet<Location>();
                HashSet<Location> open = new HashSet<Location>();
                HashSet<Location> open2 = new HashSet<Location>();

                closed.Add(location);
                open.Add(location);

                int c = 0;
                Location target = null;
                for (int tries = 0; tries < 128; tries++)
                {
                    open2.Clear();
                    foreach (Location loc in open)
                    {
                        foreach (Location l2 in loc.getNeighbours())
                        {
                            if (!closed.Contains(l2))
                            {
                                closed.Add(l2);
                                open2.Add(l2);
                            }
                            if (l2.soc != null && l2.soc.getRel(this.society).state == DipRel.dipState.war)
                            {
                                c += 1;
                                if (Eleven.random.Next(c) == 0)
                                {
                                    target = l2;
                                }
                            }
                            foreach (Unit u2 in l2.units)
                            {
                                if (this.hostileTo(u2))
                                {
                                    c += 1;
                                    if (Eleven.random.Next(c) == 0)
                                    {
                                        target = l2;
                                    }
                                }
                            }
                        }
                    }
                    if (target != null) { break; }//Now found the closest unit set
                    open = open2;
                    open2 = new HashSet<Location>();
                }

                if (target != null)
                {
                    task = new Task_GoToLocationAgressively(target);
                }
            }
        }

        public override Sprite getSprite(World world)
        {
            return world.textureStore.unit_paladin;
        }

        public override string getName()
        {
            return "Paladin";
        }

        public override bool hasSpecialInfo()
        {
            return false;
        }

        public override Color specialInfoColour()
        {
            return base.specialInfoColour();
        }


        public override string getDesc()
        {
            return "Paladins hunt down enthralled agents, tracking them by information given by aware individuals and by rumours.";
        }

        public override string getTitleM()
        {
            return "Paladin";
        }

        public override string getTitleF()
        {
            return "Paladin";
        }
    }
}
