using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Set_Ruins : Settlement
    {
        public int stochasticAge;

        public Set_Ruins(Location loc) : base(loc)
        {
            this.name = "Ruins of " + loc.shortName;
            this.isHuman = false;
        }

        public override void turnTick()
        {
            base.turnTick();
            if (Eleven.random.Next(2) == 0)
            {
                stochasticAge += 1;
                if (stochasticAge > 10)
                {
                    this.location.settlement = null;
                    this.location.map.addMessage(this.name + " finally crumble into nothing.", MsgEvent.LEVEL_GRAY);
                    World.log("Clean ruin ending");
                }
            }
        }
        public override Sprite getSprite()
        {
            return location.map.world.textureStore.loc_minor_ruins;
        }
    }
}
