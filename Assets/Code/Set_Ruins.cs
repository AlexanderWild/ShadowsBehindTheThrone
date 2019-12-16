using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Set_Ruins : Settlement
    {
        public Set_Ruins(Location loc) : base(loc)
        {
            this.name = "Ruins of " + loc.shortName;
            this.isHuman = false;
        }

        public override Sprite getSprite()
        {
            return location.map.world.textureStore.loc_minor_ruins;
        }
    }
}
