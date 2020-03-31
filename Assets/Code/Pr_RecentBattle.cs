using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Pr_RecentHumanBattle : Property_Prototype
    {
        
        public Pr_RecentHumanBattle(Map map,string name) : base(map,name)
        {
            this.name = "Recent Human Battle";
            this.baseCharge = map.param.war_battleDeadDur;
            this.decaysOverTime = true;
            this.stackStyle = stackStyleEnum.TO_MAX_CHARGE;
        }

        public override Sprite getSprite(World world)
        {
            return world.textureStore.unit_battleIcon;
        }

        internal override string getDescription()
        {
            return "This location is the site of a recent battle between human forces. The dead litter the ground, picked over by birds of carrion and wild dogs."
                + " The violence of the death, the anger, the fear, the pain, the cruel elation, is still felt in the air.";
        }
    }
}
