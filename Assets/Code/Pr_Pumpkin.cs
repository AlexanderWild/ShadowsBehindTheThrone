using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Pr_Pumpkin : Property_Prototype
    {
        
        public Pr_Pumpkin(Map map,string name) : base(map,name)
        {
            this.name = "Pumpkin";
            this.baseCharge = map.param.unit_headless_pumpkinDur;
            this.decaysOverTime = true;
            this.stackStyle = stackStyleEnum.TO_MAX_CHARGE;
        }

        public override Sprite getSprite(World world)
        {
            return world.textureStore.property_pumpkin;
        }

        internal override string getDescription()
        {
            return "A noble recently lost their head here, condemned by their peers. This is a perfect head for the horseman to steal. Move the horseman here to reclaim it.";
        }
    }
}
