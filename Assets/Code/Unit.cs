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
        public Location location;
        public GraphicalUnit outer;
        public Society society;
        public bool dontDisplayBorder = false;

        public Unit(Location loc,Society soc)
        {
            this.location = loc;
            this.society = soc;
            loc.units.Add(this);
        }

        public bool checkForDisband(Map map)
        {
            if (map.socialGroups.Contains(society) == false)
            {
                if (person != null)
                {
                    person.isDead = true;
                    if (person.state == Person.personState.enthralledAgent)
                    {
                        map.world.prefabStore.popMsg(this.getName() + " disbands as the society they depended on, " + this.society.getName() + ", is no more.");
                    }
                
                }
                map.remove(this);
                bool positive = person == null || person.state != Person.personState.enthralledAgent;
                map.addMessage(this.getName() + " disbands as their society is gone", MsgEvent.LEVEL_GREEN, positive);
                return true;
            }
            return false;
        }
        public virtual string getName()
        {
            return person.getFullName();
        }
        public abstract void turnTick(Map map);

        public abstract Sprite getSprite(World world);

        public abstract string getTitleM();
        public abstract string getTitleF();
        public abstract string getDesc();
    }
}
