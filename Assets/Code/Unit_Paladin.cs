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

        public override bool checkForDisband(Map map)
        {
            return false;
        }
        public override void turnTickAI(Map map)
        {
            if (task == null)
            {
                double bestScore = 0.7;//Default chance to find the target
                Unit target = null;
                foreach (Unit u in map.units)
                {
                    if (u.isEnthralled())
                    {
                        double score = 1;
                        if (u.location.soc != null && u.location.soc is Society) { score += 0.5; }
                        if (u.location.soc != null && u.location.soc.hostileTo(this)) { score = 0; }
                        if (u.location.person() != null)
                        {
                            score *=  1 + u.location.person().awareness;
                        }
                        score *= Eleven.random.NextDouble();
                        if (score > bestScore)
                        {
                            bestScore = score;
                            target = u;
                        }
                    }
                }
                if (target != null)
                {
                    Task_HuntEnthralled taskI = new Task_HuntEnthralled(target);
                    taskI.turnsLeft = World.staticMap.param.unit_paladin_trackPoints;
                    task = taskI;
                }
            }
            if (task != null)
            {
                task.turnTick(this);
                return;
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
            return true;
        }

        public override Color specialInfoColour()
        {
            return new Color(1, 0.9f, 0.5f);
        }

        public override string specialInfo()
        {
            return "Witchhunter";
        }

        public override string specialInfoLong()
        {
            return "This unit will try to hunt down and kill enthralled, tracking them across the world. They will require time to find a trail, which they will then follow for many turns."
                + "\nThey are more likely to find a trail if the enthralled is in a society, and more likely if the local noble has awareness of the dark.";
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
