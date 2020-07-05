using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Unit_Witchhunter : Unit
    {
        public int sinceHome = 0;

        public int wanderDur = 8;

        
        public Unit_Witchhunter(Location loc,Society soc) : base(loc,soc)
        {
            maxHp = 12;
            hp = 12;
            abilities.Add(new Abu_Base_Recruit());
        }

        public override void turnTickInner(Map map)
        {
        }

        public override void turnTickAI(Map map)
        {
            if (this.hp < this.maxHp/2)
            {
                if (this.location.soc == society)
                {
                    if (this.task == null)
                    {
                        this.task = new Task_Resupply();
                    }
                }
                else
                {
                    this.task = new Task_GoToSocialGroup(society);
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
            return world.textureStore.unit_witchHunter;
        }

        public override string getTitleM()
        {
            return "Witch Hunter";
        }

        public override string getTitleF()
        {
            return "Witch Hunter";
        }

        public override string getDesc()
        {
            return "Witch Hunters can track agents, and will aggressively attack them if they are declared hostile by their society.";
        }
    }
}
