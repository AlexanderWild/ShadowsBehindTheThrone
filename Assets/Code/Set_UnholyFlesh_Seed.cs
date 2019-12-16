using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Set_UnholyFlesh_Seed : Settlement
    {
        public Set_UnholyFlesh_Seed(Location loc) : base(loc)
        {
            this.isHuman = false;
            name = "Unholy Seed";


            militaryCapAdd += 2;
            militaryRegenAdd = 0.1;
        }

        public override Sprite getSprite()
        {
            return location.map.world.textureStore.loc_flesh;
        }
    }
}
