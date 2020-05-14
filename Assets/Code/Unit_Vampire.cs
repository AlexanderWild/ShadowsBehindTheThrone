using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Unit_Vampire : Unit
    {
        int sinceHome = 0;

        int wanderDur = 8;

        
        public Unit_Vampire(Location loc,SocialGroup soc) : base(loc,soc)
        {
            maxHp = 3;
            hp = 3;
            abilities.Add(new Abu_Base_SocialiseAtCourt());
            abilities.Add(new Abu_Base_PleadCase());
            abilities.Add(new Abu_Base_Recruit());
            abilities.Add(new Abu_Base_RecruitOutlaws());
            abilities.Add(new Abu_Base_Disrupt());
            abilities.Add(new Abu_Base_FalseEvidence());
            abilities.Add(new Abu_Base_SpreadShadow());
        }

        public override void turnTickInner(Map map)
        {
        }

        public override void turnTickAI(Map map)
        {
            if (this.location.soc == society)
            {
                sinceHome = 0;
                /*
                if (person != null && location.person() != null && location.person().state != Person.personState.broken){
                    foreach (RelObj rel in person.relations.Values)
                    {
                        double them = location.person().getRelation(rel.them).suspicion;
                        double me = rel.suspicion;
                        if (me > them)
                        {
                            map.addMessage(this.getName() + " warns " + location.person().getFullName() + " about " + rel.them.getFullName(),MsgEvent.LEVEL_ORANGE,false);
                            location.person().getRelation(rel.them).suspicion = me;
                        }
                    }
                }
                */
            }
            else
            {
                sinceHome += 1;
            }

            //Scan local units
            if (map.param.unit_investigatorsSeeEnthralled == 1)
            {
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
            return world.textureStore.unit_ghoul;
        }

        public override string getTitleM()
        {
            return "Vampire";
        }

        public override string getTitleF()
        {
            return "Vampire";
        }

        public override string getDesc()
        {
            return "Vampires are charming and aristrocratic, but depend on a constant supply of blood to survive, which leaves a trail of evidence.";
        }
    }
}
