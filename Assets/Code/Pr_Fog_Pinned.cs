using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Pr_Fog_Pinned : Property_Prototype
    {
        
        public Pr_Fog_Pinned(Map map,string name) : base(map,name)
        {
            this.name = "Trapped Fog";
            this.decaysOverTime = true;
            this.baseCharge = map.param.ability_fog_trapDuration;
            this.stackStyle = stackStyleEnum.ADD_CHARGE;
        }

        public override void turnTick(Property p, Location location)
        {
            base.turnTick(p, location);

            Hex h = location.hex;
            if (h.cloud == null)
            {
                h.cloud = new Cloud_Fog();
            }
            else if (h.cloud is Cloud_Fog)
            {
                ((Cloud_Fog)h.cloud).age = 0;
            }
        }

        public override void endProperty(Location location, Property p)
        {
        }

        public override Sprite getSprite(World world)
        {
            return world.textureStore.property_redPlague;
        }

        internal override string getDescription()
        {
            return "This area is temporarily under a persistent small amount of fog.";
        }
    }
}
