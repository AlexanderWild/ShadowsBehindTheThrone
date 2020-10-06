using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Unit_ArmyOfTheDead : Unit
    {
        public Location home;
        
        public Unit_ArmyOfTheDead(Location loc,SocialGroup soc) : base(loc,soc)
        {
            hp = 1;
            maxHp = World.staticMap.param.unit_armyOfDeadMaxHP;
            home = loc;
            isMilitary = true;
        }

        public override void turnTickInner(Map map)
        {
            //if (home.soc != this.society || home.settlement == null || home.settlement.attachedUnit != this)
            //{
            //    this.disband(map, "Disbanded due to loss of home");
            //    if (home.settlement.attachedUnit == this)
            //    {
            //        home.settlement.attachedUnit = null;
            //    }
            //    return;
            //}

            if (location.soc != null && this.society.getRel(location.soc).state == DipRel.dipState.war)
            {
                location.map.takeLocationFromOther(society, location.soc, location);
            }
        }

        public override void turnTickAI(Map map)
        {
            if (task == null)
            {
                if (society.isAtWar())
                {
                    warAI();
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
            World.log("UNDEAD WAR AI");
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
                            if (this.society.getRel(u2.society).state == DipRel.dipState.war)
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

        public override Sprite getSprite(World world)
        {
            return world.textureStore.unit_armyOfDead;
        }

        public override string getName()
        {
            return "Army of the Dead";
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
            return "A mobile extension of the flesh, able to rise up and attack the neighbouring human settlements.";
        }

        public override string getTitleM()
        {
            throw new NotImplementedException();
        }

        public override string getTitleF()
        {
            throw new NotImplementedException();
        }
    }
}
