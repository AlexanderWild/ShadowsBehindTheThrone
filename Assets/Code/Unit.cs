using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public abstract class Unit
    {
        public Person person;
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
        public List<Ability> abilities = new List<Ability>();
        public List<Ability> powers = new List<Ability>();

        public List<Unit> hostility = new List<Unit>();

        public Unit(Location loc,SocialGroup soc)
        {
            this.location = loc;
            this.society = soc;
            loc.units.Add(this);
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
                if (task != null) { task.turnTick(this); }
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

        public virtual bool hostileTo (Unit other)
        {
            if (this == other) { return false; }
            if (this.society.hostileTo(other))
            {
                return true;
            }
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
                        map.addMessage(this.getName() + " switches society to " + this.society.getName(), MsgEvent.LEVEL_GRAY, false);
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
                person.die(v);
            }
            disband(map, null);
            bool positive = person == null || person.state != Person.personState.enthralledAgent;
            if (society.isDark()) { positive = false; }
            map.addMessage(this.getName() + " dies! " + v, MsgEvent.LEVEL_GREEN, positive);

            if (this.isEnthralled())
            {
                map.world.prefabStore.popMsg(this.getName() + " dies!\n" + v);
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
                map.addMessage(msg, MsgEvent.LEVEL_GREEN, positive);

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
        public virtual void turnTickInner(Map map) { }
        public abstract void turnTickAI(Map map);

        public abstract Sprite getSprite(World world);

        public abstract string getTitleM();
        public abstract string getTitleF();
        public abstract string getDesc();

    }
}
