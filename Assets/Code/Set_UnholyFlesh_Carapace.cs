using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Code
{
    public class Set_UnholyFlesh_Carapace : Settlement
    {
        public Set_UnholyFlesh_Carapace(Location loc) : base(loc)
        {
            this.isHuman = false;
            name = "Unholy Carapace";


            defensiveStrengthMax = 20;
        }

        public override Sprite getCustomTerrain()
        {
            return World.staticMap.world.textureStore.hex_special_flesh;
        }

        public override void turnTick()
        {
            base.turnTick();

            foreach (Hex h in location.territory)
            {
                h.flora = null;
            }
        }

        public override Sprite getSprite()
        {
            return location.map.world.textureStore.loc_flesh;
        }
    }
}
