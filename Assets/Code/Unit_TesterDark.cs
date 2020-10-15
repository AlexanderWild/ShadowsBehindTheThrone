using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Unit_TesterDark : Unit
    {

        
        public Unit_TesterDark(Location loc,SocialGroup soc) : base(loc,soc)
        {
            maxHp = 5;
            hp = 5;
        }

        public override void turnTickInner(Map map)
        {
            if (this.task != null) { return; }

            if (this.location.person() != null)
            {
                if (this.location.settlement.infiltration < World.staticMap.param.ability_unit_spreadShadowInfiltrationReq)
                {
                    this.task = new Task_Infiltrate();
                    return;
                }
                if (this.location.person().shadow < 1)
                {
                    this.task = new Task_SpreadShadow();
                    return;
                }
            }

            //We're not starting a new task, so this location is bad. Onwards to greener pastures
            Location target = null;
            double bestDist = -1;
            foreach (Location loc in map.locations)
            {
                if (loc == this.location) { continue; }
                if (loc.soc == null) { continue; }
                if (loc.soc.hostileTo(this)) { continue; }
                if (loc.person() != null && loc.soc is Society && loc.person().shadow < 1)
                {
                    bool good = true;
                    //foreach (Unit u in loc.units)
                    //{
                    //    if (u is Unit_TesterDark) { good = false;break; }
                    //}
                    if (good)
                    {
                        double dist = Math.Abs(loc.hex.x - this.location.hex.x) + Math.Abs(loc.hex.y - this.location.hex.y);
                        //dist *= Eleven.random.NextDouble();
                        if (dist < bestDist || bestDist == -1)
                        {
                            bestDist = dist;
                            target = loc;
                        }
                    }
                }
            }
            if (target != null)
            {
                task = new Task_GoToLocation(target);
            }
        }

        public override bool checkForDisband(Map map)
        {
            return false;
        }
        public override void turnTickAI(Map map)
        {
        }

        public override bool definesBackground()
        {
            return true;
        }

        public override Sprite getPortraitBackground()
        {
            return World.staticMap.world.textureStore.person_back_vampire;
        }

        public override Sprite getSprite(World world)
        {
            return world.textureStore.unit_enthralled;
        }

        public override bool hasSpecialInfo()
        {
            return true;
        }

        public override Color specialInfoColour()
        {
            return new Color(0.8f, 0, 0);
        }

        public override string specialInfo()
        {
            return "Autonomous";
        }

        public override string specialInfoLong()
        {
            return "Automatic testing agent";
        }

        public override string getTitleM()
        {
            return "TestDark";
        }

        public override string getTitleF()
        {
            return "TestDark";
        }

        public override string getDesc()
        {
            return "Automatic Testing Element.";
        }
    }
}
