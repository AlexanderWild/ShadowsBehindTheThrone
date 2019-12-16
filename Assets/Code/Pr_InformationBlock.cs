using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Pr_InformationBlock : Property_Prototype
    {
        
        public Pr_InformationBlock(Map map,string name) : base(map,name)
        {
            this.informationAvailabilityMult = 0.5;
            this.baseCharge = map.param.ability_informationBlackoutDuration;
            this.decaysOverTime = true;
            this.stackStyle = stackStyleEnum.ADD_CHARGE;
        }

        public override void turnTick(Location location)
        {
        }

        public override Sprite getSprite(World world)
        {
            return world.textureStore.unit_lookingGlass;
        }

        internal override string getDescription()
        {
            return "Halves the information going through the selected location. This reduces the rate at which nobles gain suspicion from evidence, and reduces the amount they fear other social groups.";
        }
    }
}
