using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Set_WormNest : Settlement
    {
        public Set_WormNest(Location loc) : base(loc)
        {
            this.isHuman = false;
            name = "Worm Nest";
            
            militaryCapAdd += 5;
            militaryRegenAdd = 0.25;
            this.defensiveStrengthMax = 0;

           
        }

        public override void turnTick()
        {
            base.turnTick();

            this.location.hex.purity = 0.5f;
        }


        public override Sprite getSprite()
        {
            return location.map.world.textureStore.loc_worm_nest;
        }
    }
}
