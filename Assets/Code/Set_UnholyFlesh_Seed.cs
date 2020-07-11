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


        }

        public override Sprite getSprite()
        {
            return location.map.world.textureStore.loc_flesh;
        }
    }
}
