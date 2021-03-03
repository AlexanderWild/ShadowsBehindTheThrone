using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Set_CityRuins : Settlement
    {
        public double infrastructure;
        public double initialInfrastructure;

        public Set_CityRuins(Location loc) : base(loc)
        {
            this.name = "Ruins of " + loc.shortName;
            this.isHuman = false;
        }

        public override void turnTick()
        {
            base.turnTick();
            if (infrastructure > 1)
            {
                infrastructure -= 1;
            }
            else
            {
                this.location.settlement = null;
                this.location.map.addMessage(this.name + " finally crumble into nothing.", MsgEvent.LEVEL_GRAY,true,location.hex);
            }
        }

        public override Sprite getSprite()
        {
            return location.map.world.textureStore.loc_minor_ruins;
        }
    }
}
