using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public abstract class Unit
    {
        public int personID = -1;
        public Location parentLocation;
        public Location location;
        public GraphicalUnit outer;
        public SocialGroup society;
        public Task task;
        public bool dontDisplayBorder = false;
        public bool isMilitary = false;
        public int hp;
        public int maxHp = 5;
        public int movesTaken = 0;
        public int lastTurnActionTaken;
        public bool isDead = false;
        public bool automated = false;
        public List<Ability> abilities = new List<Ability>();
        public List<Ability> powers = new List<Ability>();
        public bool flaggedAsEnthralledHostile = false;
        public List<Unit> hostility = new List<Unit>();

        public EventContext.State eventState = new EventContext.State();

        public Unit(Location loc, SocialGroup soc)
        {
            this.location = loc;
            this.society = soc;
            loc.units.Add(this);
        }

        public Person person {
            get { if (personID == -1) { return null; } return location.map.persons[personID]; }
            set { personID = value.index; } 
        }

        public virtual void turnTick(Map map)
        {
            if (task == null || (task is Task_Disrupted == false))
            {
                movesTaken = 0;
            }
            if (task != null && this.isEnthralled() && (task is Task_Disrupted == false))
            {
                bool lockedDown = false;
                foreach (Property pr in location.properties)
                {
                    if (pr.proto is Pr_Lockdown)
                    {
                        lockedDown = true;
                        break;
                    }
                }
                if (lockedDown)
                {
                    map.world.prefabStore.popMsg(this.getName() + " has had their task aborted, as the location they are in,  " + this.location.getName() + " is under security lockdown.");
                    task = null;
                }
            }
            if (hp == 0)
            {
                if (map.units.Contains(this)) { die(map, "Death of unknown cause."); }
                return;
            }
            if (checkForDisband(map)) { return; }
            if (checkForLocDamage(map)) { return; }
            turnTickInner(map);
            if (isEnthralled() == false)
            {
                turnTickAI(map);
            }
            else
            {
                if (automated)
                {
                    AI_WeakReleased(map);
                }
                if (task != null) { task.turnTick(this); }
            }

            if (person != null && person.title_land != null)
            {
                if (Application.isEditor)
                {
                    World.log("Err data " + this.getName() + " " + person.getFullName());
                    throw new Exception("Editor-Only Exception: Caught a person who belonged to a unit and a nation");
                }

                //Can't remove a person from their nation, but can remove a nation from a person
                person.title_land.heldBy = null;
                person.title_land = null;
            }
        }

        public virtual bool definesBackground()
        {
            return false;
        }
        public virtual bool definesForeground()
        {
            return false;
        }

        public virtual Sprite getPortraitBackground()
        {
            return null;
        }
        public virtual Sprite getPortraitForeground()
        {
            return null;
        }

        public virtual bool hasSpecialInfo()
        {
            return false;
        }
        public virtual string specialInfoLong()
        {
            return "This unit has no special mechanics.";
        }
        public virtual string specialInfo()
        {
            return "";
        }
        public virtual Color specialInfoColour()
        {
            return Color.white;
        }

        public virtual bool hostileTo (Unit other,bool allowRecursion=true)
        {
            if (this == other) { return false; }
            if (this.society.hostileTo(other))
            {
                return true;
            }
            if (this.isEnthralled() && other.flaggedAsEnthralledHostile) { return true; }
            if (allowRecursion && other.hostileTo(this,false)){ return true; }
            return hostility.Contains(other);
        }

        public virtual bool checkForDisband(Map map)
        {
            if (parentLocation != null)
            {
                if (parentLocation.soc == null)
                {
                    if (person.state == Person.personState.enthralledAgent)
                    {
                        map.world.prefabStore.popMsg(this.getName() + " disbands as the location they depended on, " + parentLocation.getName() + ", is lost.");
                    }
                    disband(map, this.getName() + " disbands as their home is lost.");
                    return true;
                }
                else
                {
                    if (this.society != parentLocation.soc)
                    {
                        this.society = parentLocation.soc;
                        map.addMessage(this.getName() + " switches society to " + this.society.getName(), MsgEvent.LEVEL_GRAY, false,location.hex);
                    }
                }
                return false;
            }

            if (map.socialGroups.Contains(society) == false)
            {
                if (person != null)
                {
                    if (person.state == Person.personState.enthralledAgent)
                    {
                        map.world.prefabStore.popMsg(this.getName() + " disbands as the society they depended on, " + this.society.getName() + ", is no more.");
                    }
                    disband(map, this.getName() + " disbands as their society is gone");
                    return true;

                }
            }
            return false;
        }

        public void die(Map map, string v)
        {
            isDead = true;

            if (GraphicalMap.selectedSelectable == this)
            {
                GraphicalMap.selectedSelectable = null;
            }
            if (person != null)
            {
                person.die(v,false);
            }
            disband(map, null);
            bool positive = person == null || person.state != Person.personState.enthralledAgent;
            if (society.isDark()) { positive = false; }
            map.addMessage(this.getName() + " dies! " + v, MsgEvent.LEVEL_GREEN, positive,location.hex);

            if (this.person != null && person.isWatched())
            {
                //map.world.prefabStore.popMsg(this.getName() + " dies!\n" + v);
                map.world.prefabStore.popMsgAgentDeath(this, this.getName() + " dies!\n" + v);
            }
        }

        public virtual void disband(Map map,string msg)
        {
            if (person != null)
            {
                person.isDead = true;
            }
            map.remove(this);
            location.units.Remove(this);
            if (location.units.Contains(this)) { throw new Exception("Double-presence in list " + this.getName()); }
            if (msg != null)
            {
                bool positive = person == null || person.state != Person.personState.enthralledAgent;
                map.addMessage(msg, MsgEvent.LEVEL_GREEN, positive,location.hex);

                if (this.isEnthralled())
                {
                    map.world.prefabStore.popMsg(this.getName() + " is gone.\n" + msg);
                }
            }
            foreach (Location loc in map.locations)
            {
                if (loc.units.Contains(this))
                {
                    throw new Exception("My loc " + this.location.getName() + " but am at " + loc.getName());
                }
            }
        }

        public virtual bool isEnthralled()
        {
            if (person == null) { return false; }
            return person.state == Person.personState.enthralledAgent;
        }

        public virtual string getName()
        {
            if (person != null)
            {
                return person.getFullName();
            }
            else
            {
                return "Agent";
            }
        }
        public virtual string getTaskShort()
        {
            if (task != null)
            {
                return task.getShort();
            }
            return "No current task";
        }
        public virtual string getTaskDesc()
        {
            if (task != null)
            {
                return task.getLong();
            }
            return "No current task";
        }

        public virtual bool checkForLocDamage(Map map)
        {
            if (location.soc != null && location.soc.hostileTo(this))
            {
                if (location.hex.cloud != null && location.hex.cloud is Cloud_Fog) { return false; }//Can't take damage while in the fog
                this.hp -= 1;
                map.world.prefabStore.particleCombat(location.hex, location.hex);
                if (this.isEnthralled())
                {
                    map.world.prefabStore.popMsg(this.getName() + " takes damage from being in the territory of " + location.soc.getName());
                }
                if (hp <= 0)
                {
                    die(map, "Killed by " + location.soc.getName());
                    return true;
                }
            }
            return false;
        }

        public void flee(Map map)
        {
            double bestDist = 0;
            Location target = null;
            foreach (Location l2 in map.locations)
            {
                bool safe = true;
                if (l2.soc == null || l2.soc.hostileTo(this)) { continue; }
                foreach (Unit u in l2.units)
                {
                    if (u.hostileTo(this)) { safe = false; break; }
                }
                if (!safe) { continue; }
                foreach (Location l3 in l2.getNeighbours())
                {
                    if (!safe) { break; }
                    foreach (Unit u in l3.units)
                    {
                        if (u.hostileTo(this)) { safe = false; break; }
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


        public void AI_WeakReleased(Map map)
        {

            if (this.movesTaken != 0) { return; }
            if (this.task is Task_GoToLocation) { return; }//Already moving or fleeing

            if (this is Unit_Vampire vamp)
            {
                if (vamp.blood < vamp.maxBlood*0.3 && location.person() != null)
                {
                    //Drink
                    vamp.blood = vamp.maxBlood;

                    double amount = map.param.unit_minorEvidence;
                    Evidence e2 = new Evidence(map.turn);
                    e2.pointsTo = vamp;
                    e2.weight = amount;
                    vamp.location.evidence.Add(e2);
                    return;
                }
            }


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
                        this.task = new Task_Infiltrate_Weak();
                        return;
                    }
                }
                else if (this.location.person().shadow < 1)
                {
                    this.task = new Task_SpreadShadow_Weak();
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
                        if (u.hostileTo(this)) { good = false; break; }
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
                        double score = (loc.person().prestige + 5) / (dist + 1);

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

        public virtual void turnTickInner(Map map) { }
        public abstract void turnTickAI(Map map);

        public abstract Sprite getSprite(World world);

        public abstract string getTitleM();
        public abstract string getTitleF();
        public abstract string getDesc();

    }
}
