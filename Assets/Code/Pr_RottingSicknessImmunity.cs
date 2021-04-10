using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Pr_RottingSicknessImmunity : Property_Prototype
    {
        
        public Pr_RottingSicknessImmunity(Map map,string name) : base(map,name)
        {
            this.name = "Rotting Sickness Immunity";
            this.baseCharge = map.param.unit_doctor_rottingSickness_ImmunityDur;
            this.decaysOverTime = true;
            this.stackStyle = stackStyleEnum.TO_MAX_CHARGE;
            this.isImmunity = true;
        }

        public override Sprite getSprite(World world)
        {
            return world.textureStore.property_rottingSicknessImmunity;
        }

        internal override string getDescription()
        {
            return "This location is immune to the Rotting Sickness. The disease cannot spread here by normal means (but you may still be able to transmit it here yourself).";
        }
    }
}
