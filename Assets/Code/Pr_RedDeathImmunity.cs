using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Pr_RedDeathImmunity : Property_Prototype
    {
        
        public Pr_RedDeathImmunity(Map map,string name) : base(map,name)
        {
            this.name = "Red Death Immunity";
            this.baseCharge = map.param.unit_rd_redDeathPlagueImmunityDur;
            this.decaysOverTime = true;
            this.stackStyle = stackStyleEnum.TO_MAX_CHARGE;
            this.isImmunity = true;
        }

        public override Sprite getSprite(World world)
        {
            return world.textureStore.property_redPlagueImmunity;
        }

        internal override string getDescription()
        {
            return "Red death immune";
        }
    }
}
