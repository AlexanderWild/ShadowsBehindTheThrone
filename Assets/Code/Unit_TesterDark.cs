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

            if (this.movesTaken != 0) { return; }
            if (this.task is Task_GoToLocation) { return; }//Already moving or fleeing


            bool shouldFlee = false;
            if (this.location.soc != null && this.location.soc.hostileTo(this))
            {
                shouldFlee = true;
            }
            if (!shouldFlee)
            {
                foreach (Unit u in location.units)
                {
                    if (u.hostileTo(this)) { shouldFlee = true; break; }
                }
            }
            if (!shouldFlee)
            {
                foreach (Location l2 in location.getNeighbours())
                {
                    if (shouldFlee) { break; }
                    foreach (Unit u in l2.units)
                    {
                        if (u.hostileTo(this)) { shouldFlee = true; break; }
                    }
                }
            }
            if (shouldFlee)
            {
                flee(map);
            }
            if (this.task != null) { return; }


            this.movesTaken += 1;
            if (this.location.person() != null)
            {
                if (this.location.settlement.infiltration < World.staticMap.param.ability_unit_spreadShadowInfiltrationReq)
                {
                    bool lockedDown = false;
                    foreach (Property p in location.properties)
                    {
                        if (p.proto is Pr_Lockdown)
                        {
                            lockedDown = true;
                            break;
                        }
                    }
                    if (location.settlement.security >= 5) { lockedDown = true; }
                    if (!lockedDown)
                    {
                        this.task = new Task_Infiltrate();
                        return;
                    }
                }
                else if (this.location.person().shadow < 1)
                {
                    this.task = new Task_SpreadShadow();
                    return;
                }
            }

            //We're not starting a new task, so this location is bad. Onwards to greener pastures
            Location target = null;
            double bestDist = -1;
            int c = 0;
            Location safeHarbor = null;
            foreach (Location loc in map.locations)
            {
                if (loc == this.location) { continue; }
                if (loc.soc == null) { continue; }
                if (loc.soc.hostileTo(this)) { continue; }
                if (loc.person() != null && loc.soc is Society && loc.person().shadow < 1)
                {
                    bool good = true;
                    foreach (Unit u in loc.units)
                    {
                        if (u.hostileTo(this)) { good = false;break; }
                    }
                    if (loc.settlement.security >= 5) { good = false; }
                    if (good)
                    {
                        foreach (Property pr in loc.properties)
                        {
                            if (pr.proto is Pr_Lockdown) { good = false; break; }
                            if (pr.proto is Pr_MajorSecurityBoost) { good = false; break; }
                        }
                    }

                    if (good)
                    {
                        double dist = Math.Abs(loc.hex.x - this.location.hex.x) + Math.Abs(loc.hex.y - this.location.hex.y);
                        //dist *= Eleven.random.NextDouble();
                        double score = (loc.person().prestige + 5)/ (dist + 1);

                        if (loc.map.overmind.lightbringerLocations.Contains(loc))
                        {
                            score *= 25;
                        }
                        //score *= Eleven.random.NextDouble() * Eleven.random.NextDouble();
                        //if (dist < bestDist || bestDist == -1)
                        //{
                        //    bestDist = dist;
                        //    target = loc;
                        //}
                        if (score > bestDist || bestDist == -1)
                        {
                            bestDist = score;
                            target = loc;
                        }
                    }
                }

                if (loc.soc is Society)
                {
                    c += 1;
                    if (Eleven.random.Next(c) == 0)
                    {
                        safeHarbor = loc;
                    }
                }
            }
            if (target != null)
            {
                task = new Task_GoToLocation(target);
            }
            else
            {
                //We're unable to find anywhere to infiltrate. Probably banned from everywhere else. Do we have safe harbor? Can we change our name?
                if (location.soc is Society && location.soc.hostileTo(this) == false)
                {
                    //In a safe harbor, swap out now
                    task = new Task_ChangeIdentity();
                }
                else if (safeHarbor != null)
                {
                    task = new Task_GoToLocation(safeHarbor);
                }
                else
                {
                    die(map, "Was no further use"); ;
                }
            }
        }

        public void flee(Map map)
        {
            double bestDist = 0;
            Location target = null;
            foreach (Location l2 in map.locations)
            {
                bool safe = true;
                if (l2.soc == null  || l2.soc.hostileTo(this)) {continue; }
                foreach (Unit u in l2.units)
                {
                    if (u.hostileTo(this)) { safe = false;break; }
                }
                if (!safe) { continue; }
                foreach (Location l3 in l2.getNeighbours())
                {
                    if (!safe) { break; }
                    foreach (Unit u in l3.units)
                    {
                        if (u.hostileTo(this)) { safe = false;break; }
                    }
                }
                if (!safe) { continue; }

                //Evaluated as safe. Flee option
                double dist = map.getDist(this.location, l2);
                if (dist < bestDist || target == null)
                {
                    target = l2;
                    bestDist = dist;
                }
            }
            task = new Task_GoToLocation(target);
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
