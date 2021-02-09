using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Pr_Fog_Source : Property_Prototype
    {
        
        public Pr_Fog_Source(Map map,string name) : base(map,name)
        {
            this.name = "Well of Fog";
            this.decaysOverTime = false;
            this.stackStyle = stackStyleEnum.TO_MAX_CHARGE;
        }

        public override void turnTick(Property p, Location location)
        {
            base.turnTick(p, location);

            bool maintained = false;
            if (location.person() == null) { maintained = false; }
            else
            {
                if (location.person().state == Person.personState.enthralled) { maintained = true; }
                if (location.person().state == Person.personState.broken) { maintained = true; }
                if (location.person().shadow >= 0.5) { maintained = true; }
            }

            if (!maintained) {p.charge = 0; }

            foreach (Hex h in location.territory)
            {
                if (h.cloud == null)
                {
                    h.cloud = new Cloud_Fog();
                }else if (h.cloud is Cloud_Fog)
                {
                    ((Cloud_Fog)h.cloud).age = 0;
                }
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
            return "This area is generating fog constantly, pouring out into neighbouring locations. Will exist as long as the location's noble is under the influence of the shadow.";
        }
    }
}
