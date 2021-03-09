using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Pr_Blizzard : Property_Prototype
    {
        
        public Pr_Blizzard(Map map,string name) : base(map,name)
        {
            this.baseCharge = map.param.ability_informationBlackoutDuration;
            this.decaysOverTime = true;
            this.stackStyle = stackStyleEnum.ADD_CHARGE;
        }

        public override Sprite getSprite(World world)
        {
            return world.textureStore.property_blizzard;
        }

        internal override string getDescription()
        {
            return "This location is experiencing a heavy blizzard. Any other location which is also experiencing a blizzard will be cut off, preventing units from travelling between locations.";
        }
    }
}
