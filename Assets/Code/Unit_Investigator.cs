using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Unit_Investigator : Unit
    {
        int sinceHome = 0;

        int wanderDur = 8;

        
        public Unit_Investigator(Location loc,Society soc) : base(loc,soc)
        {
            maxHp = 3;
        }

        public override void turnTickInner(Map map)
        {
        }

        public override void turnTickAI(Map map)
        {
            if (this.location.soc == society)
            {
                sinceHome = 0;
            }
            else
            {
                sinceHome += 1;
            }

            //Scan local units
            foreach (Unit u in location.units)
            {
                if (u.isEnthralled())
                {
                    if (this.person != null && u.person != null)
                    {
                        this.person.getRelation(u.person).suspicion = Math.Min(1, this.person.getRelation(u.person).suspicion + map.param.unit_suspicionFromProximity);
                        map.addMessage(this.getName() + " has gained suspicion of " + u.getName(), MsgEvent.LEVEL_RED, false);
                    }
                }
            }


            if (task != null)
            {
                task.turnTick(this);
                return;
            }

            if (location.evidence.Count > 0)
            {
                task = new Task_Investigate();
            }
            else if (sinceHome > wanderDur)
            {
                task = new Task_GoToSocialGroup(society);
            }
            else
            {
                task = new Task_Wander();
            }


            task.turnTick(this);
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
