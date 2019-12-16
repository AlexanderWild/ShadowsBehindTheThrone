using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Pr_MilitaryAidOutgoing : Property_Prototype
    {
        
        public Pr_MilitaryAidOutgoing(Map map,string name) : base(map,name)
        {
            this.baseCharge = map.param.ability_militaryAidDur;
            this.milCapAdd = -map.param.ability_militaryAidAmount;
            this.decaysOverTime = true;
            this.stackStyle = stackStyleEnum.NONE;
        }

        public override void turnTick(Location location)
        {
        }

        public override Sprite getSprite(World world)
        {
            return world.textureStore.unit_supply;
        }

        internal override string getDescription()
        {
            return "This location is sending military aid, decreasing its military cap by " + (int)this.milCapAdd + "."
                + "\nThis will reduce this nation in war, and reduce the ability of this location to rebel against their sovreign.";
        }
    }
}
